using Microsoft.AspNetCore.Identity;

namespace MyBlog.Data.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
