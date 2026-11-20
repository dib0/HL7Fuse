namespace HL7FuseWebApi.Models
{
    public class Login
    {
        public string Client_Id { get; set; } = string.Empty;
        public string Client_Secret { get; set; } = string.Empty;
        public string Grant_Type { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
    }
}
