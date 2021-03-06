using HiddenVilla_Client.Service.IService;
using Microsoft.AspNetCore.Components;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenVilla_Client.Pages.Authentication
{
    public partial class Register
    {
        private UserRequestDTO userForRegistration = new UserRequestDTO();
        public bool IsProcessing { get; set; } = false;
        public bool ShowRegistrationErrors { get; set; }
        public IEnumerable<string> Errors { get; set; }

        [Inject]
        public IAuthenticationService authService { get; set; }

        [Inject]
        public NavigationManager navMan { get; set; }

        private async Task RegisterUser()
        {
            ShowRegistrationErrors = false;
            IsProcessing = true;
            var result = await authService.RegisterUser(userForRegistration);

            if (result.IsRegistrationSuccessful)
            {
                IsProcessing = false;
                navMan.NavigateTo("/login");
            }
            else
            {
                IsProcessing = false;
                Errors = result.Errors;
                ShowRegistrationErrors = true;
            }
        }
    }
}
