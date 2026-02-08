namespace JWTApi.Api.ViewModels
{
    public class VerifyCaptchaModel
    {
        public string CaptchaId { get; set; } = string.Empty;
        public string UserInput { get; set; } = string.Empty;
    }
}
