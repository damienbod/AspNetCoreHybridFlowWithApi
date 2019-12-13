namespace AspNetCoreRequireMfaOidc
{
    /// <summary>
    /// https://tools.ietf.org/html/draft-ietf-oauth-amr-values-04
    /// </summary>
    public static class Amr
    {
        /// <summary>
        /// Jones, et al.Expires May 17, 2017
        /// Internet-Draft Authentication Method Reference Values November 2016
        /// Facial recognition
        /// </summary>
        public static string Face = "face";

        /// <summary>
        /// Fingerprint biometric
        /// </summary>
        public static string Fpt = "fpt";

        /// <summary>
        /// Use of geolocation information
        /// </summary>
        public static string Geo = "geo";

        /// <summary>
        /// Proof-of-possession(PoP) of a hardware-secured key.See
        /// Appendix C of[RFC4211] for a discussion on PoP.
        /// </summary>
        public static string Hwk = "hwk";

        /// <summary>
        /// Iris scan biometric
        /// </summary>
        public static string Iris = "iris";

        /// <summary>
        /// Knowledge-based authentication [NIST.800-63-2] [ISO29115]
        /// </summary>
        public static string Kba = "kba";

        /// <summary>
        /// Multiple-channel authentication.  The authentication involves
        /// communication over more than one distinct communication channel.
        /// For instance, a multiple-channel authentication might involve both
        /// entering information into a workstation's browser and providing
        /// information on a telephone call to a pre-registered number.
        /// </summary>
        public static string Mca = "mca";

        /// <summary>
        /// Multiple-factor authentication [NIST.800-63-2]  [ISO29115].  When 
        /// this is present, specific authentication methods used may also be
        /// included.
        /// </summary>
        public static string Mfa = "mfa";

        /// <summary>
        /// One-time password.  One-time password specifications that this
        /// authentication method applies to include[RFC4226] and[RFC6238].
        /// </summary>
        public static string Otp = "otp";

        /// <summary>
        /// Personal Identification Number or pattern (not restricted to
        /// containing only numbers) that a user enters to unlock a key on the
        /// device.This mechanism should have a way to deter an attacker
        /// from obtaining the PIN by trying repeated guesses.
        /// </summary>
        public static string Pin = "pin";

        /// <summary>
        /// Password-based authentication
        /// </summary>
        public static string Pwd = "pwd";

        /// <summary>
        /// Risk-based authentication [JECM]
        /// </summary>
        public static string Rba = "rba";

        /// <summary>
        /// Retina scan biometric Jones, et al.Expires May 17, 2017
        /// Internet-Draft Authentication Method Reference Values November 2016
        /// </summary>
        public static string Retina = "retina";

        /// <summary>
        /// Smart card
        /// </summary>
        public static string Sc = "sc";

        /// <summary>
        /// Confirmation using SMS message to the user at a registered number
        /// </summary>
        public static string Sms = "sms";

        /// <summary>
        /// Proof-of-possession(PoP) of a software-secured key.See
        /// Appendix C of[RFC4211] for a discussion on PoP.
        /// </summary>
        public static string Swk = "swk";

        /// <summary>
        /// Confirmation by telephone call to the user at a registered number
        /// </summary>
        public static string Tel = "tel";

        /// <summary>
        /// User presence test
        /// </summary>
        public static string User = "user";

        /// <summary>
        /// Voice biometric
        /// </summary>
        public static string Vbm = "vbm";

        /// <summary>
        /// Windows integrated authentication, as described in [MSDN]
        /// </summary>
        public static string Wia = "wia";
    }
}
