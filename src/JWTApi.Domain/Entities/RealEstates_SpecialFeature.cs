using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class RealEstates_SpecialFeature
    {
        public int RealEstatesId { get; set; }
        public int SpecialFeatureId { get; set; }

        //public RealEstates RealStates { get; set; } = default!;
        //public SpecialFeature SpecialFeature { get; set; } = default!;
    }
}
