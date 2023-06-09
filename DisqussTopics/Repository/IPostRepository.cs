﻿using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetPosts();
        Task<IEnumerable<Post>> GetPostsNoTracking();
        Task<IEnumerable<Post>> GetPostsByTopic();
        Task<IEnumerable<Post>> GetPostsByTopicNoTracking();
        Post GetPostBySlug(string slug, string topic);
        Task<Post> GetPostBySlugNoTracking(string slug, string topic);
        Task<Post> GetPostById(int id);
        Task<Post> GetPostByIdNoTracking(int id);
        void InsertPost(Post post);
        void UpdatePost(Post post);
        void DeletePost(Post post);
        Task SaveAsync();
        IQueryable<Post> GetPostsQuery();
    }
}
