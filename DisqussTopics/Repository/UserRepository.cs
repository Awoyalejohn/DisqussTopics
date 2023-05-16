using DisqussTopics.Data;
using DisqussTopics.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DisqussTopics.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DTContext _context;

        public UserRepository(IHttpContextAccessor httpContextAccessor, DTContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public string GetUserId()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return currentUserId;
        }

        public async Task<DTUser> GetUserByIdAsync(string userId)
        {
            var currentUser = await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Topic)
                        .ThenInclude(t => t.DTUsers)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Upvotes)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Downvotes)
                .Include(u => u.PostUpvotes)
                    .ThenInclude(p => p.Downvotes)
                .Include(u => u.PostDownvotes)
                    .ThenInclude(p => p.Upvotes)
                .Include(u => u.CommentUpvotes)
                .Include(u => u.CommentDownvotes)
                .Include(u => u.SubscibedTopics)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return currentUser;
        }

        public void UpdateUser(DTUser user)
        {
            _context.Users.Update(user);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
