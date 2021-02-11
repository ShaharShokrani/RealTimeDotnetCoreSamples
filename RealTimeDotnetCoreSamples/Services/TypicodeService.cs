using Microsoft.Extensions.Logging;
using RealTimeDotnetCoreSamples.Models;
using RealTimeDotnetCoreSamples.Models.Typicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RealTimeDotnetCoreSamples.Services
{
    public class TypicodeService : ITypicodeService, IUserService
    {        
        private readonly ILogger<TypicodeService> _logger;
        private readonly TypicodeConfig _typicodeConfig;
        private readonly IHttpClientFactoryService _httpClientFactoryService;

        public TypicodeService(ILogger<TypicodeService> logger,
                                AppSettingsConfig appSettingsConfig,
                                IHttpClientFactoryService httpClientFactoryService)
        {
            this._logger = logger;
            this._typicodeConfig = appSettingsConfig.Typicode;
            this._httpClientFactoryService = httpClientFactoryService;
        }
        
        public async Task<ResultHandler<IEnumerable<Album>>> GetAlbums(AlbumsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return ResultHandler.Fail<IEnumerable<Album>>(E_ErrorType.EntityNotValid);
                }                

                Dictionary<string, string> nameValueCollection = new Dictionary<string, string>();

                if (request.Id.HasValue)
                    nameValueCollection.Add("Id", request.Id.Value.ToString());

                HttpRequestModel httpRequestModel = new HttpRequestModel()
                {
                    MethodName = "Albums",
                    CancellationToken = new System.Threading.CancellationToken(),
                    ClientName = E_HttpClient.Typicode,
                    CompletionOption = System.Net.Http.HttpCompletionOption.ResponseContentRead,
                    Headers = null,
                    HttpContent = null,
                    HttpMethod = System.Net.Http.HttpMethod.Get,
                    QueryString = null
                };

                List<Album> result = new List<Album>();

                for (int i = 0; i < 50; i++)
                {
                    var albumsResponseResult = await this._httpClientFactoryService.GetHttpResponse<Album[]>(httpRequestModel);
                    if (!albumsResponseResult.Success)
                    {
                        return ResultHandler.Fail<IEnumerable<Album>>(albumsResponseResult.ErrorType);
                    }
                    result.AddRange(albumsResponseResult.Value);
                }                

                return ResultHandler.Ok<IEnumerable<Album>>(result);
            }
            catch (System.Exception ex)
            {
                return ResultHandler.Fail<IEnumerable<Album>>(ex);
            }
        }
        public async Task<ResultHandler<IEnumerable<UserModel>>> GetUsers(UsersRequest request)
        {
            try
            {
                if (request == null)
                {
                    return ResultHandler.Fail<IEnumerable<UserModel>>(E_ErrorType.EntityNotValid);
                }                

                Dictionary<string, string> nameValueCollection = new Dictionary<string, string>();

                if (request.Id.HasValue)
                    nameValueCollection.Add("Id", request.Id.Value.ToString());

                HttpRequestModel httpRequestModel = new HttpRequestModel()
                {
                    MethodName = "Users",
                    CancellationToken = new System.Threading.CancellationToken(),
                    ClientName = E_HttpClient.Typicode,
                    CompletionOption = System.Net.Http.HttpCompletionOption.ResponseContentRead,
                    Headers = null,
                    HttpContent = null,
                    HttpMethod = System.Net.Http.HttpMethod.Get,
                    QueryString = nameValueCollection
                };

                var usersResult = await this._httpClientFactoryService.GetHttpResponse<User[]>(httpRequestModel);
                if (!usersResult.Success)
                {
                    return ResultHandler.Fail<IEnumerable<UserModel>>(usersResult.ErrorType);
                }

                List<UserModel> result = new List<UserModel>();

                foreach (User user in usersResult.Value)
                {
                    result.Add(new UserModel()
                    {
                        Id = user.Id,
                        Name = user.Name
                    });
                }

                Thread.Sleep(TimeSpan.FromSeconds(_typicodeConfig.UsersThreadSleep));

                return ResultHandler.Ok<IEnumerable<UserModel>>(result);
            }
            catch (System.Exception ex)
            {
                return ResultHandler.Fail<IEnumerable<UserModel>>(ex);
            }
        }
        public async Task<ResultHandler<string>> GetAlbumYaml(int id)
        {
            try
            {
                Dictionary<string, string> nameValueCollection = new Dictionary<string, string>();
                
                nameValueCollection.Add("Id", id.ToString());

                HttpRequestModel httpRequestModel = new HttpRequestModel()
                {
                    MethodName = "Albums",
                    CancellationToken = new System.Threading.CancellationToken(),
                    ClientName = E_HttpClient.Typicode,
                    CompletionOption = System.Net.Http.HttpCompletionOption.ResponseContentRead,
                    Headers = null,
                    HttpContent = null,
                    HttpMethod = System.Net.Http.HttpMethod.Get,
                    QueryString = null
                };                 

                var albumsResponseResult = await this._httpClientFactoryService.GetHttpResponse<Album[]>(httpRequestModel);
                if (!albumsResponseResult.Success)
                {
                    return ResultHandler.Fail<string>(albumsResponseResult.ErrorType);
                }
                Album album = albumsResponseResult.Value.FirstOrDefault();

                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                if (album == null)
                {
                    return ResultHandler.Fail<string>(E_ErrorType.Supplier);
                }

                var result = serializer.Serialize(album);
                return ResultHandler.Ok<string>(result);
            }
            catch (System.Exception ex)
            {
                return ResultHandler.Fail<string>(ex);
            }
        }
    }

    public interface ITypicodeService
    {
        Task<ResultHandler<IEnumerable<Album>>> GetAlbums(AlbumsRequest request);
        Task<ResultHandler<string>> GetAlbumYaml(int id);
    }
}
