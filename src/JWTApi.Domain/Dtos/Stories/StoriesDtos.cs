using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Stories
{
    public class StoriesDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public bool IsViews { get; set; }
        public StoriesDetails storiesDetails { get; set; }

    }
    public class StoriesDetails
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Url { get; set; }
        public DateTime ExpiredAt { get; set; }

    }
}
