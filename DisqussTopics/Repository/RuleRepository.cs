using DisqussTopics.Data;
using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public class RuleRepository : IRuleRepository
    {
        private readonly DTContext _context;

        public RuleRepository(DTContext context)
        {
            _context = context;
        }

        public void InsertRule(Rule rule)
        {
            _context.Rules.Add(rule);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
