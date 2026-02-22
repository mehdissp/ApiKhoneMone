using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Descriptions { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public  CategoryType CategoryType { get; set; }
       
        public ICollection<Facilities> Facilities { get; set; } = new List<Facilities>();
        public ICollection<SpecialFeature> SpecialFeature { get; set; } = new List<SpecialFeature>();
        public ICollection<RealEstates> RealEstates { get; set; } = new List<RealEstates>();
        public ICollection<RealEstatesRent> RealEstatesRents { get; set; } = new List<RealEstatesRent>();
        public ICollection<SearchRequest> SearchRequests { get; set; } = new List<SearchRequest>();
        private Category() { }
        public Category(string name, string icon, string desc , CategoryType categoryType)
        {
            Name = name;
            Icon = icon;
            Descriptions = desc;
            CategoryType = categoryType;
        }

        public void UpdateCategory(string name, string icon, string descriptions, CategoryType categoryType)
        {
            Name = name;
            Icon = icon;
            Descriptions = descriptions;
            CategoryType = categoryType;
        }
    }
}
