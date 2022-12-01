using tryitter.Models;
using Microsoft.EntityFrameworkCore;

namespace tryitter.Repository
{
    public class tryitterRepository
    {
        protected readonly TryitterContext _context;
        public tryitterRepository(TryitterContext context)
        {
            _context = context;
        }
        public IEnumerable<User> GetUsers()
        {
            return _context.Users;
        }
    }
}