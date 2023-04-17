namespace DisqussTopics.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int TopicId { get; set; }
        public Topic? Topic { get; set; }
    }
}