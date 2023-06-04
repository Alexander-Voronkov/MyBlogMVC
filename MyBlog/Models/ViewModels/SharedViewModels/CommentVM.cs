using MyBlog.Data.Entities;

namespace MyBlog.Models.ViewModels.SharedViewModels
{
    public class CommentVM
    {
        public Comment Comment { get; set; } = default!;
        public bool IsReply { get; set; }
        public int CurrentNested { get; set; }

        public const int MaxNested = 5;

        public string BackgroundColor
        {
            get => CurrentNested % 2 == 0 ? "lightgray" : "white";
        }
    }
}
