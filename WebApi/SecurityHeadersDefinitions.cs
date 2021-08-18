using Microsoft.AspNetCore.Builder;

namespace WebApi
{
    public static class SecurityHeadersDefinitions
    {
        public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev)
        {
            var policy = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .RemoveServerHeader()
                .AddCrossOriginOpenerPolicy(builder =>
                {
                    builder.SameOrigin();
                })
                .AddCrossOriginEmbedderPolicy(builder =>
                {
                    builder.RequireCorp();
                })
                .AddCrossOriginResourcePolicy(builder =>
                {
                    builder.SameOrigin();
                })
                .RemoveServerHeader()
                .AddPermissionsPolicy(builder =>
                {
                    builder.AddAccelerometer().None();
                    builder.AddAutoplay().None();
                    builder.AddCamera().None();
                    builder.AddEncryptedMedia().None();
                    builder.AddFullscreen().All();
                    builder.AddGeolocation().None();
                    builder.AddGyroscope().None();
                    builder.AddMagnetometer().None();
                    builder.AddMicrophone().None();
                    builder.AddMidi().None();
                    builder.AddPayment().None();
                    builder.AddPictureInPicture().None();
                    builder.AddSyncXHR().None();
                    builder.AddUsb().None();
                });

            AddCspHstsDefinitions(isDev, policy);

            return policy;
        }

        private static void AddCspHstsDefinitions(bool isDev, HeaderPolicyCollection policy)
        {
            if (!isDev)
            {
                policy.AddContentSecurityPolicy(builder =>
                {
                    builder.AddObjectSrc().None();
                    builder.AddBlockAllMixedContent();
                    builder.AddImgSrc().None();
                    builder.AddFormAction().None();
                    builder.AddFontSrc().None();
                    builder.AddStyleSrc().None();
                    builder.AddScriptSrc().None();
                    builder.AddBaseUri().Self();
                    builder.AddFrameAncestors().None();
                    builder.AddCustomDirective("require-trusted-types-for", "'script'");
                });
                // maxage = one year in seconds
                policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365);
            }
            else
            {
                // allow swagger UI for dev
                policy.AddContentSecurityPolicy(builder =>
                {
                    builder.AddObjectSrc().None();
                    builder.AddBlockAllMixedContent();
                    builder.AddImgSrc().Self().From("data:");
                    builder.AddFormAction().Self();
                    builder.AddFontSrc().Self();
                    builder.AddStyleSrc().Self().UnsafeInline();
                    builder.AddScriptSrc().Self().UnsafeInline(); //.WithNonce();
                    builder.AddBaseUri().Self();
                    builder.AddFrameAncestors().None();
                });
            }
        }
    }
}
