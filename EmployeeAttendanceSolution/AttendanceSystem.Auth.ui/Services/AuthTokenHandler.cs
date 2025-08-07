using Blazored.LocalStorage;
using Blazored.SessionStorage;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.ui.Services
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly ISessionStorageService _sessionStorage;
        private readonly ILocalStorageService _localStorage;

        public AuthTokenHandler(
            ISessionStorageService sessionStorage,
            ILocalStorageService localStorage)
        {
            _sessionStorage = sessionStorage;
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
           
                // First try session storage, then fall back to local storage
                var token = await _sessionStorage.GetItemAsync<string>("authToken")
                           ?? await _localStorage.GetItemAsync<string>("authToken");

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                return await base.SendAsync(request, cancellationToken);
           
        }
    }
}