using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace DemoGraphConsoleApp1
{
    class Program
    {
        private static string appId = "24d5ec42-c341-4d1d-b0a2-52c8e2ac3216";
        private static string[] scopes = { "User.Read", "Calendars.Read"};

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            GraphServiceClient graphClient = new GraphServiceClient(new authProvider (appId, scopes));
            var user = graphClient.Me.Request().GetAsync();
            Console.WriteLine($"User name is {user.Result.DisplayName}");
        }
    }

    class authProvider : IAuthenticationProvider
    {
        private static IPublicClientApplication msalClient;
        private string[] _scopes;
        public authProvider (string appId, string[] scopes)
        {
            msalClient = PublicClientApplicationBuilder
                .Create(appId)
                .WithTenantId("organizations")
                .Build();
            _scopes = scopes;

        }
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", await getAccessToken());
        }

        private async Task<string> getAccessToken()
        {
            var result = await msalClient.AcquireTokenWithDeviceCode(_scopes, callback =>
           {
               Console.WriteLine(callback.Message);
               return Task.FromResult(0);
           }).ExecuteAsync();
            return result.AccessToken;
        }
    }
}
