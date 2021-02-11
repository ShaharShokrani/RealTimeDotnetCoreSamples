using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using RealTimeDotnetCoreSamples.Models;

namespace RealTimeDotnetCoreSamples.Services
{
    public interface IHttpClientFactoryService
    {
        Task<ResultHandler<TRes>> GetHttpResponse<TRes>(HttpRequestModel httpRequest);
    }

    public class HttpClientFactoryService : IHttpClientFactoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientFactoryService> _logger;

        public HttpClientFactoryService(IHttpClientFactory httpClientFactory,
            ILogger<HttpClientFactoryService> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._logger = logger;
        }

        public async Task<ResultHandler<TRes>> GetHttpResponse<TRes>(HttpRequestModel httpRequest)
        {
            try
            {
                if (!httpRequest.IsValid)
                {
                    this._logger.LogError("GetHttpResponse Not Valid", new object[] { httpRequest });
                    return ResultHandler.Fail<TRes>(E_ErrorType.EntityNotValid);
                }

                ResultHandler<Stream> resultHandler = await GetStreamResponseByHttpRequest(httpRequest);                

                if (!resultHandler.Success)
                {
                    this._logger.LogError("GetStreamResponseByHttpRequest Failed", new object[] { httpRequest });
                    return ResultHandler.Fail<TRes>(resultHandler.ErrorType);
                }

                using (Stream stream = resultHandler.Value)
                {
                    TRes result = await JsonSerializer.DeserializeAsync<TRes>(stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }, httpRequest.CancellationToken);
                    return ResultHandler.Ok(result);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("GetHttpResponse Exception", new object[] { httpRequest });
                return ResultHandler.Fail<TRes>(ex);
            }
        }

        private async Task<ResultHandler<Stream>> GetStreamResponseByHttpRequest(HttpRequestModel httpRequestModel)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(httpRequestModel.MethodName, httpRequestModel.QueryString, httpRequestModel.HttpMethod, httpRequestModel.HttpContent);
                HttpClient httpClient = CreateHttpClient(httpRequestModel.ClientName, httpRequestModel.Headers);

                HttpResponseMessage httpResponseMessage = await SendHttpResponseMessage(httpRequestMessage, httpClient, httpRequestModel.CompletionOption, httpRequestModel.CancellationToken);
                httpResponseMessage.EnsureSuccessStatusCode();

                var content = await httpResponseMessage.Content.ReadAsStreamAsync();

                return ResultHandler.Ok(content);
            }
            catch (TaskCanceledException tcex)
            {
                stopwatch.Stop();
                _logger.LogError(tcex, $"GetStreamResponseByHttpRequest, time out was: {stopwatch.Elapsed.TotalSeconds}", new object[] { httpRequestModel });
                return ResultHandler.Fail<Stream>(tcex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStreamResponseByHttpRequest", new object[] { httpRequestModel });
                return ResultHandler.Fail<Stream>(ex);
            }
        }

        private async Task<ResultHandler<string>> GetStringResponseByHttpRequest(HttpRequestModel httpRequestModel)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(httpRequestModel.MethodName, httpRequestModel.QueryString, httpRequestModel.HttpMethod, httpRequestModel.HttpContent);
                HttpClient httpClient = CreateHttpClient(httpRequestModel.ClientName, httpRequestModel.Headers);

                HttpResponseMessage httpResponseMessage = await SendHttpResponseMessage(httpRequestMessage, httpClient, httpRequestModel.CompletionOption, httpRequestModel.CancellationToken);
                httpResponseMessage.EnsureSuccessStatusCode();

                var content = await httpResponseMessage.Content.ReadAsStringAsync();

                return ResultHandler.Ok(content);
            }
            catch (TaskCanceledException tcex)
            {
                stopwatch.Stop();
                _logger.LogError(tcex, $"GetStreamResponseByHttpRequest, time out was: {stopwatch.Elapsed.TotalSeconds}", new object[] { httpRequestModel });
                return ResultHandler.Fail<string>(tcex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStreamResponseByHttpRequest", new object[] { httpRequestModel });
                return ResultHandler.Fail<string>(ex);
            }
        }

        private HttpClient CreateHttpClient(E_HttpClient clientName, IDictionary<string, string> headers)
        {
            HttpClient httpClient = this._httpClientFactory.CreateClient(clientName.ToString());

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (!httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value))
                    {

                    }
                }
            }

            return httpClient;
        }

        private System.Net.Http.HttpRequestMessage CreateHttpRequestMessage(string methodName, IDictionary<string, string> queryString, HttpMethod httpMethod, HttpContent httpContent)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                RequestUri = CreateRequestUri(methodName, queryString),
                Method = httpMethod,
                Content = httpContent,
            };

            return requestMessage;
        }

        private Uri CreateRequestUri(string methodName, IDictionary<string, string> queryString)
        {
            if (queryString != null)
                methodName = QueryHelpers.AddQueryString(methodName, queryString);
            Uri uri = new Uri(methodName, UriKind.Relative);
            return uri;
        }

        private async Task<HttpResponseMessage> SendHttpResponseMessage(HttpRequestMessage httpRequestMessage, HttpClient httpClient, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            try
            {
                using (httpRequestMessage.Content)
                {
                    using (httpRequestMessage)
                    {
                        var response = await httpClient.SendAsync(httpRequestMessage, completionOption, cancellationToken);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendHttpResponseMessage", new object[] { httpRequestMessage, httpClient });
                return null;
            }
        }
    }    
}
