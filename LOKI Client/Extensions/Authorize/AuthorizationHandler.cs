using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.Extensions.Authorize
{
    public class AuthorizationHandler : DelegatingHandler
    {
        private readonly UserProvider _userProvider;

        public AuthorizationHandler(UserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Check if the token is expired
            if (_userProvider.IsTokenExpired())
            {
                // Notify HomeViewModel about token expiration
                WeakReferenceMessenger.Default.Send(new OpenLoginPageRequest());
            }

            // Add the Authorization header if the token is valid
            var token = _userProvider.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
