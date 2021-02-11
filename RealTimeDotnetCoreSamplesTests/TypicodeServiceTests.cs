using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RealTimeDotnetCoreSamples.Models.Typicode;
using RealTimeDotnetCoreSamples.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RealTimeDotnetCoreSamplesTests
{
    [TestClass]
    public class TypicodeServiceTests
    {
        private readonly Mock<ILogger<TypicodeService>> _loggerTypicodeServiceMock = new Mock<ILogger<TypicodeService>>();
        private readonly Mock<ILogger<HttpClientFactoryService>> _loggerHttpClientFactoryServiceMock = new Mock<ILogger<HttpClientFactoryService>>();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        
        private ITypicodeService _typicodeService;
        private IHttpClientFactoryService _httpClientFactoryService;
        [TestInitialize]
        public void Setup()
        {
            HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Brotli
            });

            httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "br");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RealTimeDotnetCoreSamples");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            this._httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            this._httpClientFactoryService = new HttpClientFactoryService(_httpClientFactoryMock.Object, _loggerHttpClientFactoryServiceMock.Object);
            this._typicodeService = new TypicodeService(_loggerTypicodeServiceMock.Object, new RealTimeDotnetCoreSamples.Models.AppSettingsConfig(), _httpClientFactoryService);
        }

        [TestMethod]
        public async Task GetAlbumsTest_WhenCalled_ShouldReturnOkResponse()
        {
            try
            {
                //Arrange
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                int id = 1;
                Album expectedAlbum = new Album()
                {
                    id = id
                };

                //Act
                var actualYamlResult = await this._typicodeService.GetAlbumYaml(id);

                string actualYaml = actualYamlResult.Value;

                // Assert            
                var actualAlbum = deserializer.Deserialize<Album>(actualYaml);

                Assert.AreEqual(expectedAlbum.id, actualAlbum.id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
