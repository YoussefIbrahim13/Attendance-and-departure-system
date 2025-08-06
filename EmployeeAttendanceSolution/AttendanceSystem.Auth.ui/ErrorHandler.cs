using Microsoft.AspNetCore.Components;
using System.Net;


    public class ErrorHandler
    {
        private readonly NavigationManager _navigation;

        public ErrorHandler(NavigationManager navigation)
        {
            _navigation = navigation;
        }

        public void HandleError(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    _navigation.NavigateTo("/logout");
                    break;
                case HttpStatusCode.Forbidden:
                    _navigation.NavigateTo("/access-denied");
                    break;
                default:
                    // Log other errors
                    break;
            }
        }
    }

