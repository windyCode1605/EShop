namespace CR.InfrastructureBase
{
    /// <summary>
    /// Consent values representing user's authorization decision
    /// </summary>
    public static class ConsentValue
    {
        /// <summary>
        /// User grants consent and authorizes the application
        /// </summary>
        public const string Grant = "grant";

        /// <summary>
        /// User denies consent and rejects the application
        /// </summary>
        public const string Deny = "deny";
    }
}
