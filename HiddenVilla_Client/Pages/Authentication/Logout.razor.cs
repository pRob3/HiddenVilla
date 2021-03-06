using HiddenVilla_Client.Service.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenVilla_Client.Pages.Authentication
{
    public partial class Logout
    {
        [Inject]
        public IAuthenticationService authenticationService { get; set; }

        [Inject]
        public NavigationManager navMan { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await authenticationService.Logout();
            navMan.NavigateTo("/");
        }
    }
}
