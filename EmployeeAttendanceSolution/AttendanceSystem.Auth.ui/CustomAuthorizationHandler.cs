    using Blazored.SessionStorage;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;


    public class CustomAuthorizationHandler : DelegatingHandler
    {
        private readonly ISessionStorageService _sessionStorage;

        public CustomAuthorizationHandler(ISessionStorageService sessionStorage)
        {
        _sessionStorage = sessionStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _sessionStorage.GetItemAsync<string>("authToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

