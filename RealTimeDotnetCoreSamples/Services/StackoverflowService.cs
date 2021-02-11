using Microsoft.Extensions.Logging;
using RealTimeDotnetCoreSamples.Models;
using RealTimeDotnetCoreSamples.Models.Stackoverflow;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeDotnetCoreSamples.Services
{
    public class StackoverflowService : IStackoverflowService, IUserService
    {        
        private readonly ILogger<StackoverflowService> _logger;
        private readonly StackoverflowConfig _stackoverflowConfig;
        private readonly IHttpClientFactoryService _httpClientFactoryService;

        public StackoverflowService(ILogger<StackoverflowService> logger,
                                AppSettingsConfig appSettingsConfig,
                                IHttpClientFactoryService httpClientFactoryService)
        {
            this._logger = logger;
            this._stackoverflowConfig = appSettingsConfig.Stackoverflow;
            this._httpClientFactoryService = httpClientFactoryService;
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
                
                nameValueCollection.Add("order", "desc");
                nameValueCollection.Add("sort", "reputation");
                nameValueCollection.Add("site", "stackoverflow");

                HttpRequestModel httpRequestModel = new HttpRequestModel()
                {
                    MethodName = "Users",
                    CancellationToken = new System.Threading.CancellationToken(),
                    ClientName = E_HttpClient.Stackoverflow,
                    CompletionOption = System.Net.Http.HttpCompletionOption.ResponseContentRead,
                    Headers = null,
                    HttpContent = null,
                    HttpMethod = System.Net.Http.HttpMethod.Get,
                    QueryString = nameValueCollection
                };

                var usersResult = await this._httpClientFactoryService.GetHttpResponse<UserResponse>(httpRequestModel);
                if (!usersResult.Success)
                {
                    return ResultHandler.Fail<IEnumerable<UserModel>>(usersResult.ErrorType);
                }

                List<UserModel> result = new List<UserModel>();

                foreach (User user in usersResult.Value.items)
                {
                    result.Add(new UserModel()
                    {
                        Id = user.user_id,
                        Name = user.display_name
                    });
                }

                Thread.Sleep(TimeSpan.FromSeconds(this._stackoverflowConfig.UsersThreadSleep));

                return ResultHandler.Ok<IEnumerable<UserModel>>(result);
            }
            catch (System.Exception ex)
            {
                return ResultHandler.Fail<IEnumerable<UserModel>>(ex);
            }
        }
    }

    public interface IStackoverflowService
    {
    }
}
