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

        public IEnumerable<PostDTO> GetPosts()
        {
            var posts = _context.Posts
                .Select(x => new PostDTO
                {
                    PostId = x.PostId,
                    Content = x.Content,
                    UserId = x.UserId,
                }).ToList();

            return posts;
        }

        public PostDTO? GetPostById(int postId)
        {
            var post = _context.Posts.Where(post => post.PostId == postId)
                .Select(x => new PostDTO
                {
                    PostId = x.PostId,
                    Content = x.Content,
                    UserId = x.UserId,
                }).FirstOrDefault();

            return post;
        }

        public PostDTO? AddPost(PostDTO post) // Utilizado PostDTO para não gerar loop infinito 
        {
            var userFound = _context.Users.Find(post.UserId);
            if (userFound != null)
            {
                var newPost = new Post { Content = post.Content, UserId = post.UserId };
                _context.Posts.Add(newPost);
                _context.SaveChanges();
            }
            else
            {
                return null;
            }

            return post;
        }

        public PostDTO? UpdatePost(Post post, int postId)
        {
            var postFound = _context.Posts.Find(postId);
            if (postFound != null)
            {
                postFound.Content = post.Content;
                _context.SaveChanges();

            }
            else
            {
                return null;
            }

            return GetPostById(postId);
        }

        public Post? DeletePostById(int postId)
        {
            var postFound = _context.Posts.Find(postId);

            if (postFound != null)
            {
                _context.Posts.Remove(postFound);
                _context.SaveChanges();
            }
            return postFound;
        }

        // Requisito: Listar todos os posts de uma conta x
        public IEnumerable<PostDTO>? GetPostsByUserId(int userId)
        {
            var userFound = GetUserById(userId);
            if (userFound == null) return null;

            var post = _context.Posts.Where(post => post.UserId == userId)
                .Select(x => new PostDTO
                {
                    PostId = x.PostId,
                    Content = x.Content,
                    UserId = x.UserId,
                }).ToList();

            return post;
        }

        // Requisito: Lista o último post de uma conta x
        public PostDTO? GetPostByUserId(int userId)
        {
            var userFound = GetUserById(userId);
            var userPosts = GetPostsByUserId(userId);
            if (userFound == null || !userPosts.Any()) return null;

            var post = _context.Posts.Where(post => post.UserId == userId)
                .Select(x => new PostDTO
                {
                    PostId = x.PostId,
                    Content = x.Content,
                    UserId = x.UserId,
                }).OrderBy(x => x.PostId);

            return post.Last();
        }
    }
}