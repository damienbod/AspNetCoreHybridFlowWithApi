using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Services.AppAuthentication;

namespace StsServerIdentity.Services.Certificate
{
    public class KeyVaultCertificateService
    {
        private readonly string _keyVaultEndpoint;
        private readonly string _certificateName;

        public KeyVaultCertificateService(string keyVaultEndpoint, string certificateName)
        {
            if (string.IsNullOrEmpty(keyVaultEndpoint))
            {
                throw new ArgumentException("missing keyVaultEndpoint");
            }

            _keyVaultEndpoint = keyVaultEndpoint; // "https://damienbod.vault.azure.net"
            _certificateName = certificateName; // certificateName
        }

        public async Task<(X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate)> GetCertificatesFromKeyVault(
            SecretClient secretClient, CertificateClient certificateClient)
        {
            (X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate) certs = (null, null);

            certs.ActiveCertificate = await GetCertificateAsync(_certificateName, secretClient);

            var certificateItems = GetAllEnabledCertificateVersions(certificateClient);
            //var item = certificateItems.FirstOrDefault();
            //if (item != null)
            //{
            //    certs.ActiveCertificate = await GetCertificateAsync(item.Identifier.Identifier, secretClient);
            //}

            //if (certificateItems.Count > 1)
            //{
            //    certs.SecondaryCertificate = await GetCertificateAsync(certificateItems[1].Identifier.Identifier, secretClient);
            //}

            return certs;
        }

        private List<CertificateProperties> GetAllEnabledCertificateVersions(
            CertificateClient certificateClient)
        {
            var certificateVersions = certificateClient.GetPropertiesOfCertificateVersions(_certificateName);
            var certificateItems = certificateVersions.ToList();

            // Find all enabled versions of the certificate and sort them by creation date in decending order 
            return certificateVersions
              .Where(certVersion => certVersion.Enabled.HasValue && certVersion.Enabled.Value)
              .OrderByDescending(certVersion => certVersion.CreatedOn)
              .ToList();
        }

        private async Task<X509Certificate2> GetCertificateAsync(string certName, SecretClient secretClient)
        {
            // Create a new secret using the secret client.
            var secretName = certName;
            //var secretVersion = "";
            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);

            var privateKeyBytes = Convert.FromBase64String(secret.Value);

            var certificateWithPrivateKey = new X509Certificate2(privateKeyBytes,
                (string)null,
                X509KeyStorageFlags.MachineKeySet);

            return certificateWithPrivateKey;
        }

    }
}