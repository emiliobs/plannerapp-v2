using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlannerApp.Shared.Models;
using PlannerApp.Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PlannerApp.Components.Authentication
{
    public partial class  LoginForm  : ComponentBase
    {
         [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public NavigationManager  Navigation{ get; set; }

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        public ILocalStorageService  Storage{ get; set; }

        public LoginRequest _model = new LoginRequest();

        private bool _isBusy = false;

        public string _errorMEssage = string.Empty;

        private async Task LoginUserAsync()
        {
            _isBusy = true;
            _errorMEssage = string.Empty;

            var response = await HttpClient.PostAsJsonAsync("/api/v2/auth/login",_model);

            if (response.IsSuccessStatusCode)
            {
                var resul = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResult>>();
                //store it in the local storege;
                await Storage.SetItemAsStringAsync("access_token", resul.Value.Token);
                await Storage.SetItemAsync<DateTime>("expiry_date", resul.Value.ExpiryDate);

                await AuthenticationStateProvider.GetAuthenticationStateAsync();

                Navigation.NavigateTo("/");
            }
            else
            {
                var erroResult = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _errorMEssage = erroResult.Message;
            }

            _isBusy = false;
        }
    }
}
