using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface IUserRepository
    {
        string GetUserId();
        Task<DTUser> GetUserByIdAsync(string userId);
        void UpdateUser(DTUser user);
        Task SaveAsync();
    }
}
