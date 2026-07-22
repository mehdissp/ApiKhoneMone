namespace JWTApi.Api.ViewModels.Posts
{
    public class PostsRequestViewModel
    {
        public int? CategoryId { get; set; }
        public string? SearchStream { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public bool? IsPublished { get; set; }
    }
}
