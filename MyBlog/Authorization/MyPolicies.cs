namespace MyBlog.Authorization
{
    public class MyPolicies
    {
        public const string SuperAdminAccessOnly = "SuperAdminAccessOnly";
        public const string AdminAndAboveAccess = "AdminAndAboveAccess";
        public const string PostsWriterAndAboveAccess = "PostsWriterAndAboveAccess";
    }
}
