namespace CustomIdentity.ViewModels
{
    public class WatchingViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public double Rating { get; set; }
        public List<Comment> Comments { get; set; } 
    }
}