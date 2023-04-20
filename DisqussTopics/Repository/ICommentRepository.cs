using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetComments();
        Task<IEnumerable<Comment>> GetCommentsNoTracking();
        Task<IEnumerable<Comment>> GetPostComments();
        Task<IEnumerable<Comment>> GetPostCommentsNoTracking();
        Post GetCommentById(int id);
        Task<Post> GetCommentByIdNoTracking(int id);
        void InsertComment(Comment comment);
        void UpdateComment(Comment comment);
        void DeleteComment(int id);
        Task SaveAsync();
    }
}
