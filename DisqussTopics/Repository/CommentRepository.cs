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

        public void DeleteComment(Comment comment)
        {
            _context.Comments.Remove(comment);
        }

        public async Task<Comment> GetCommentById(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Upvotes)
                .Include(c => c.Downvotes)
                .FirstOrDefaultAsync(c => c.Id == id);

            return comment;
        }

        public async Task<Comment> GetCommentByIdNoTracking(int id)
        {
            var comment = await _context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return comment;
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
                .Include(c => c.Upvotes)
                .Include(c => c.Downvotes)
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
            _context.Comments.Update(comment);
        }
    }
}
