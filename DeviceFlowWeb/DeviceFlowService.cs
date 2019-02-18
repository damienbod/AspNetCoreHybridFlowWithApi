using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
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

            //Console.WriteLine($"user code   : {response.UserCode}");
            //Console.WriteLine($"device code : {response.DeviceCode}");
            //Console.WriteLine($"URL         : {response.VerificationUri}");
            //Console.WriteLine($"Complete URL: {response.VerificationUriComplete}");

            //Console.WriteLine($"\nPress enter to launch browser ({response.VerificationUri})");
            //Console.ReadLine();

            return response;
        }

    }
}
