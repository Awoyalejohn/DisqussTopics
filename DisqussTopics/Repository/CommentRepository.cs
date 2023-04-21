using DisqussTopics.Data;
using DisqussTopics.Models;
using Microsoft.EntityFrameworkCore;

namespace DisqussTopics.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DTContext _context;

        public CommentRepository(DTContext context)
        {
            _context = context;
        }

        public void DeleteComment(int id)
        {
            throw new NotImplementedException();
        }

        public Post GetCommentById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetCommentByIdNoTracking(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetComments()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetCommentsNoTracking()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetPostComments()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Comment>> GetPostCommentsNoTracking(Post post)
        {
            var comments = await _context.Comments
                .AsNoTracking()
                .Where(c => c.PostId == post.Id)
                .ToListAsync();

            return comments;
        }

        public void InsertComment(Comment comment)
        {
            _context.Comments.Add(comment);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateComment(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
