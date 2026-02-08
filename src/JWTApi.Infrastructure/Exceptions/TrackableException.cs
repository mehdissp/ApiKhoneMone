using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Exceptions
{
    public class TrackableException : Exception
    {
        public string TrackId { get; set; }

        public TrackableException(string message, string trackId) : base(message)
        {
            TrackId = trackId;
        }
    }
}
