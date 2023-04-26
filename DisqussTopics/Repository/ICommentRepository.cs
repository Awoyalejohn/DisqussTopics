using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetComments();
        Task<IEnumerable<Comment>> GetCommentsNoTracking();
        Task<IEnumerable<Comment>> GetPostComments();
        Task<IEnumerable<Comment>> GetPostCommentsNoTracking(Post post);
        Task<Comment> GetCommentById(int id);
        Task<Comment> GetCommentByIdNoTracking(int id);
        void InsertComment(Comment comment);
        void UpdateComment(Comment comment);
        void DeleteComment(Comment comment);
        Task SaveAsync();
    }
}
