namespace MyBlog.Models.ViewModels.SuperAdminViewModels
{
    public class UserAccessVM
    {
        public string Email { get; set; } = default!;
        public Access Access { get; set; } = default!;
    }

    public enum Access
    {
        Admin,
        PostWriter,
        None,
    }
}
