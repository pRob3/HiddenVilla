using HiddenVilla_Client.Service.IService;
using Microsoft.AspNetCore.Components;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HiddenVilla_Client.Pages.Authentication
{
    public partial class Login
    {
        private AuthenticationDTO userForAuthentication = new AuthenticationDTO();
        public bool IsProcessing { get; set; } = false;
        public bool ShowLoginError { get; set; }
        public string Error { get; set; }

        public string ReturnUrl { get; set; }

        [Inject]
        public IAuthenticationService authService { get; set; }

        [Inject]
        public NavigationManager navMan { get; set; }

        private async Task LoginUser()
        {
            ShowLoginError = false;
            IsProcessing = true;
            var result = await authService.Login(userForAuthentication);

            if (result.IsAuthSuccessful)
            {
                IsProcessing = false;

                var absoluteUri = new Uri(navMan.Uri);
                var queryParam = HttpUtility.ParseQueryString(absoluteUri.Query);

                ReturnUrl = queryParam["returnUrl"];

                if (string.IsNullOrEmpty(ReturnUrl))
                {
                    navMan.NavigateTo("/");
                }
                else
                {
                    navMan.NavigateTo("/"+ ReturnUrl);
                }
                
            }
            else
            {
                IsProcessing = false;
                Error = result.ErrorMessage;
                ShowLoginError = true;
            }
        }
    }
}
