using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetPosts();
        Post GetPostBySlug(string slug);
        void InsertPost(Post post);
        void UpdatePost(Post post);
        void DeletePost(string slug);
        Task SaveAsync();
    }
}
