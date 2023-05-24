using System.ComponentModel.DataAnnotations;

namespace DisqussTopics.Models
{
    public class Rule
    {
        public int Id { get; set; }

        [StringLength(250)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public int TopicId { get; set; }
        public Topic? Topic { get; set; }
    }
}