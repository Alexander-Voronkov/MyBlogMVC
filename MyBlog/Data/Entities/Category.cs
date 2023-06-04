namespace MyBlog.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<Post>? Posts { get; set; }
    }
}
