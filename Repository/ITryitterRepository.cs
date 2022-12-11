using tryitter.Models;

namespace tryitter.Repository
{
    public interface ITryitterRepository
    {
        IEnumerable<UserDTO> GetUsers();
        UserDTO? GetUserById(int userId);
        User AddUser(User user);
        User? UpdateUser(User user, int userId);
        User? DeleteUserById(int id);
        IEnumerable<PostDTO> GetPosts();
        PostDTO? GetPostById(int postId);
        PostDTO? AddPost(PostDTO post);
        PostDTO? UpdatePost(Post post, int postId);
        Post? DeletePostById(int postId);
        IEnumerable<PostDTO>? GetPostsByUserId(int userId);
        PostDTO? GetPostByUserId(int userId);
        UserDTO? GetUserByEmailAndPassword(UserLogin user);
        IEnumerable<PostDTO>? GetPostOrPostsByUserName(string userName, string allOrLast);
        User? GetUserByName(string userName);
        User? GetUserByEmail(string userEmail);
    }
}