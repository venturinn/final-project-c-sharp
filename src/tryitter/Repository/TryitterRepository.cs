using tryitter.Models;
using Microsoft.EntityFrameworkCore;

namespace tryitter.Repository
{
    public class TryitterRepository
    {
        protected readonly TryitterContext _context;
        public TryitterRepository(TryitterContext context)
        {
            _context = context;
        }
        public IEnumerable<UserDTO> GetUsers()
        {
            var users = _context.Users
            .Select(x => new UserDTO
            {
                UserId = x.UserId,
                Name = x.Name,
                Email = x.Email,
                Module = x.Module,
                Status = x.Status,
            }).ToList();

            return users;
        }

        public UserDTO? GetUserById(int userId)
        {

            var user = _context.Users.Where(user => user.UserId == userId)
            .Select(x => new UserDTO
            {
                UserId = x.UserId,
                Name = x.Name,
                Email = x.Email,
                Module = x.Module,
                Status = x.Status,
            }).FirstOrDefault();

            return user;
        }

        public User AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User? UpdateUser(User user, int userId)
        {
            var userFound = _context.Users.Find(userId);
            if (userFound != null)
            {
                userFound.Name = user.Name;
                userFound.Email = user.Email;
                userFound.Module = user.Module;
                userFound.Status = user.Status;
                userFound.Password = user.Password;

                _context.SaveChanges();

            }

            return _context.Users.Find(userId); ;
        }

        public User? DeleteUserById(int id)
        {
            var userFound = _context.Users.Find(id);

            if (userFound != null)
            {
                _context.Users.Remove(userFound);
                _context.SaveChanges();
            }
            return userFound;
        }
    }
}