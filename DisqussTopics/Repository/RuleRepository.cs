using DisqussTopics.Data;
using DisqussTopics.Models;
using Microsoft.EntityFrameworkCore;

namespace DisqussTopics.Repository
{
    public class RuleRepository : IRuleRepository
    {
        private readonly DTContext _context;

        public RuleRepository(DTContext context)
        {
            _context = context;
        }

        public void DeleteRule(Rule rule)
        {
            _context.Rules.Remove(rule);
        }

        public async Task<Rule?> GetRuleById(int id)
        {
            return await _context.Rules
                .Include(r => r.Topic)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rule?> GetRuleByIdNoTracking(int id)
        {
            return await _context.Rules
                .AsNoTracking()
                .Include(r => r.Topic)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public void InsertRule(Rule rule)
        {
            _context.Rules.Add(rule);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateRule(Rule rule)
        {
            _context.Rules.Update(rule);
        }
    }
}
