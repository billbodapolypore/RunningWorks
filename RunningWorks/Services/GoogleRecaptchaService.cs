using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RunningWorks.Services
{
    public class GoogleRecaptchaService : IRecaptchaService
    {
        // TODO: Move these to config
        private readonly Uri _apiUri = new Uri("https://www.google.com/recaptcha/api/siteverify");
        private readonly string _siteSecret = "6LdJLkAUAAAAAFZlvixYG538KlHX6wkhc2Mbq12L";
        private readonly ILogger<GoogleRecaptchaService> _logger;

        public GoogleRecaptchaService(ILogger<GoogleRecaptchaService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> VerifyResponse(string clientResponse)
        {
            _logger.LogInformation("VerifyResponse Start");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var apiRequest = new ApiRequest() {
                    SiteSecret = _siteSecret,
                    ClientResponse = clientResponse
                };

                var requestJson = JsonConvert.SerializeObject(apiRequest);
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                try
                {
                    _logger.LogInformation("POST to Google Api");
                    var httpResponse = await httpClient.PostAsync(_apiUri, requestContent);

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        // If we have an unsucessful status code log the response 
                        _logger.LogError($"Unsuccessful api request. StatusCode: {httpResponse.StatusCode}; ReasonPhrase: {httpResponse.ReasonPhrase};");
                    }

                    var responseJson = await httpResponse.Content.ReadAsStringAsync();

                    var apiReponse = JsonConvert.DeserializeObject<ApiResponse>(responseJson);


                    if (!apiReponse.Success)
                    {
                        _logger.LogError($"requestJson: {requestJson}; responseJson: {responseJson};");
                    }

                    return apiReponse.Success;
                }
                catch (HttpRequestException ex)
                {
                    // If we have an http exception log the exception
                    _logger.LogError(ex, "Unhandled HttpRequestException");
                }

                // For the best user experience assume the captcha was successful
                return true;
            }
        }

        public class ApiRequest
        {
            [JsonProperty(PropertyName = "secret")]
            public string SiteSecret { get; set; }
            [JsonProperty(PropertyName = "response")]
            public string ClientResponse { get; set; }
        }

        public class ApiResponse
        {
            [JsonProperty(PropertyName = "success")]
            public bool Success { get; set; }
            [JsonProperty(PropertyName = "error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
    }
}
