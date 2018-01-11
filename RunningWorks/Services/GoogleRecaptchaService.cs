using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RunningWorks.Services
{
    public class GoogleRecaptchaService : IRecaptchaService
    {
        // TODO: Move these to config
        private readonly string _apiUri = "https://www.google.com/recaptcha/api/siteverify";
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

                try
                {
                    var fullUri = new Uri($"{_apiUri}?secret={_siteSecret}&response={clientResponse}");
                    var httpResponse = await httpClient.PostAsync(fullUri, null);

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        // If we have an unsucessful status code log the response 
                        _logger.LogError($"Unsuccessful api request. StatusCode: {httpResponse.StatusCode}; ReasonPhrase: {httpResponse.ReasonPhrase};");
                    }

                    var responseJson = await httpResponse.Content.ReadAsStringAsync();
                    var apiReponse = JsonConvert.DeserializeObject<ApiResponse>(responseJson);

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

        public class ApiResponse
        {
            [JsonProperty(PropertyName = "success")]
            public bool Success { get; set; }
            [JsonProperty(PropertyName = "error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
    }
}
