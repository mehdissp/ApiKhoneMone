using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Stories
{
    public class StoryForSite
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public List<StoryUser> StoryUser { get; set; }
    }
    public class StoryUser
    {
        public int Id { get; set; }
        public string   Name { get; set; }
        public string Url { get; set; }
        public string Caption { get; set; }
        public string Link { get; set; }
        public string LinkText { get; set; }

    }
}
