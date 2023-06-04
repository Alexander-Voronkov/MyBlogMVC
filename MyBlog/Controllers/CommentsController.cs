using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Authorization;
using MyBlog.Data;
using MyBlog.Data.Entities;
using MyBlog.Models;
using MyBlog.Models.ViewModels.SharedViewModels;

namespace MyBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        private readonly ILogger<CommentsController> _logger;

        public CommentsController(
            ApplicationContext context,
            UserManager<User> userManager,
            ILogger<CommentsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentModel commentModel)
        {
            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser is null)
            {
                return Unauthorized();
            }

            Comment comment = new Comment
            {
                Message = commentModel.Message,
                ParentCommentId = commentModel.ParentCommentId,
                PostId = commentModel.PostId,
                UserId = currentUser.Id,
                Created = DateTime.Now,
            };

            await _context.AddAsync(comment);
            await _context.SaveChangesAsync();

            CommentVM commentVM = new CommentVM
            {
                Comment = comment,
                CurrentNested = commentModel.CurrentNested,
                IsReply = commentModel.IsReply,
            };

            await _context
                .Entry(commentVM.Comment)
                .Reference(c => c.Post)
                .LoadAsync();

            return PartialView("_WriteParentCommentAndChildrenPartial", commentVM);
        }


        [Authorize(MyPolicies.AdminAndAboveAccess)]
        public async Task<IActionResult> DeleteComment([FromBody] int? commentId)
        {
            if (commentId is null)
            {
                return BadRequest();
            }

            Comment? comment = await _context.Comments.FindAsync(commentId);

            if (comment is null)
            {
                return NotFound();
            }

            await _context.Entry(comment)
                .Collection(c => c.ChildComments!)
                .LoadAsync();

            await _context
                .Entry(comment)
                .Reference(c => c.ParentComment)
                .LoadAsync();

            _context.Comments.Remove(comment);

            int deletedComments = 1;

            await RemoveChildrenComments(comment);

            async Task RemoveChildrenComments(Comment comment)
            {
                foreach (var childComment in comment.ChildComments)
                {
                    _context.Comments.Remove(childComment);

                    deletedComments++;

                    await _context
                        .Entry(childComment)
                        .Collection(c => c.ChildComments)
                        .LoadAsync();

                    if (childComment.ChildComments!.Count > 0)
                    {
                        await RemoveChildrenComments(childComment);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(deletedComments);
        }
    }
}
