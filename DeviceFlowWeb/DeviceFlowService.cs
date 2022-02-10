using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeviceFlowWeb;

public class DeviceFlowService
{
    private readonly AuthConfigurations _authConfigurations;
    private readonly IHttpClientFactory _clientFactory;

    public DeviceFlowService(IOptions<AuthConfigurations> authConfigurations, IHttpClientFactory clientFactory)
    {
        _authConfigurations = authConfigurations.Value;
        _clientFactory = clientFactory;
    }

    public async Task<DeviceAuthorizationResponse> RequestDeviceCode()
    {
        var client = _clientFactory.CreateClient();

        var disco = await GetDiscoveryEndpoints(client);

        var deviceAuthorizationRequest = new DeviceAuthorizationRequest
        {
            Address = disco.DeviceAuthorizationEndpoint,
            ClientId = _authConfigurations.ClientId
        };
        deviceAuthorizationRequest.Scope = "email profile openid";
        var response = await client.RequestDeviceAuthorizationAsync(deviceAuthorizationRequest);

        if (response.IsError)
        {
            throw new Exception(response.Error);
        }

        return response;
    }

    internal async Task<TokenResponse> RequestTokenAsync(string deviceCode, int interval)
    {
        var client = _clientFactory.CreateClient();

        var disco = await GetDiscoveryEndpoints(client);

        while (true)
        {
            if(!string.IsNullOrWhiteSpace(deviceCode))
            {
                var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _authConfigurations.ClientId,
                    DeviceCode = deviceCode
                });

                if (response.IsError)
                {
                    if (response.Error == "authorization_pending" || response.Error == "slow_down")
                    {
                        Console.WriteLine($"{response.Error}...waiting.");
                        await Task.Delay(interval * 1000);
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
            else
            {
                await Task.Delay(interval * 1000);
            }
        }
    }

    private async Task<DiscoveryDocumentResponse> GetDiscoveryEndpoints(HttpClient client)
    {
        var disco = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(
            client, _authConfigurations.StsServer);

        if (disco.IsError)
        {
            throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
        }

        return disco;
    }
}
