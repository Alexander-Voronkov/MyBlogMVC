namespace MyBlog.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string? Body { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public byte[] MainPostImage { get; set; } = default!;
        public bool IsDeleted { get; set; }

        public int? CategoryId { get; set; }
        public string? UserId { get; set; }

        public Category? Category { get; set; }
        public User? User { get; set; }

        public ICollection<Comment>? Comments { get; set; }
        public ICollection<PostImage>? PostImages { get; set; }

    }
}
