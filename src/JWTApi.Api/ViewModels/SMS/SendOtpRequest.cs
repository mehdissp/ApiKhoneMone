namespace JWTApi.Api.ViewModels.SMS
{
    public class SendOtpRequest
    {
        public string Mobile { get; set; }
        public string CaptchaId { get; set; }
        public string CaptchaValue { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
    }
}
