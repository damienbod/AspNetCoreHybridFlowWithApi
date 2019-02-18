using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceFlowWeb
{
    public class DeviceFlowService
    {
        private readonly IOptions<AuthConfigurations> _authConfigurations;
        private readonly IHttpClientFactory _clientFactory;

        public DeviceFlowService(IOptions<AuthConfigurations> authConfigurations, IHttpClientFactory clientFactory)
        {
            _authConfigurations = authConfigurations;
            _clientFactory = clientFactory;
        }

        internal async Task<DeviceAuthorizationResponse> BeginLogin()
        {
            var discoClient = new DiscoveryClient(_authConfigurations.Value.StsServer);
            var disco = await discoClient.GetAsync();
            if (disco.IsError)
            {
                throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
            }

            var client = _clientFactory.CreateClient();
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = "deviceFlowWebClient"
            });

            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            return response;
        }

        public async Task<TokenResponse> RequestTokenAsync(DeviceAuthorizationResponse authorizeResponse)
        {
            var discoClient = new DiscoveryClient(_authConfigurations.Value.StsServer);
            var disco = await discoClient.GetAsync();
            if (disco.IsError)
            {
                throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
            }

            var client = _clientFactory.CreateClient();

            while (true)
            {
                var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "device",
                    DeviceCode = authorizeResponse.DeviceCode
                });

                if (response.IsError)
                {
                    if (response.Error == "authorization_pending" || response.Error == "slow_down")
                    {
                        Console.WriteLine($"{response.Error}...waiting.");
                        Thread.Sleep(authorizeResponse.Interval * 1000);
                    }
                    else
                    {
                        throw new Exception(response.Error);
                    }
                }
                else
                {
                    return response;
                }
            }
        }

    }
}
