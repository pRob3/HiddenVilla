using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenVilla_Client.Pages.Authentication
{
    public partial class RedirectToLogin
    {
        [Inject]
        private NavigationManager navMan { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }

        bool notAuthorized { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationState;

            if(authState?.User?.Identity == null || !authState.User.Identity.IsAuthenticated)
            {
                var returnUrl = navMan.ToBaseRelativePath(navMan.Uri);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    navMan.NavigateTo("login", true);
                }
                else
                {
                    navMan.NavigateTo($"login?returnUrl={returnUrl}", true);
                }
            }
            else
            {
                notAuthorized = true;
            }


        }
    }
}
