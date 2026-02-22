using Microsoft.AspNetCore.Http;

public class HouseCheckRequest
{
    public IFormFile Image { get; set; }
}

public class HouseCheckResponse
{
    public bool IsHouseRelated { get; set; }
    public string Message { get; set; }
    public string PredictedLabel { get; set; }
}