﻿using DisqussTopics.Data;
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
                .Include(u => u.PostUpvotes)
                .Include(u => u.PostDownvotes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return currentUser;
        }
    }
}
