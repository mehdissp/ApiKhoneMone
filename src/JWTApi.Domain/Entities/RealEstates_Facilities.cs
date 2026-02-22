using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JWTApi.Domain.Entities
{
    public class RealEstates_Facilities
    {
        public int RealEstatesId { get; set; }
        public int FacilitiesId { get; set; }

        //public RealEstates RealStates { get; set; } = default!;
        //public Facilities Facilities { get; set; } = default!;
    }
}
