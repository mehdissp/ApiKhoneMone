using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Payments
{
    // Models/ZarinpalModels.cs
    public class ZarinpalRequestResponse
    {
        public ZarinpalRequestData data { get; set; }
    }

    public class ZarinpalRequestData
    {
        public int code { get; set; }
        public string authority { get; set; }
    }

    public class ZarinpalVerifyResponse
    {
        public ZarinpalVerifyData data { get; set; }
    }

    public class ZarinpalVerifyData
    {
        public int code { get; set; }
        public long ref_id { get; set; }
    }
}
