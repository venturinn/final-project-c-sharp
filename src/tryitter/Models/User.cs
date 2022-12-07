using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tryitter.Models;
public class User
{

    [Column("user_id")]
    [Key]
    public int UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Module { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Password { get; set; } = default!;
    public virtual ICollection<Post>? Posts { get; } = default!;
}

public class UserDTO
{
    public int UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Module { get; set; } = default!;
    public string Status { get; set; } = default!;
}


/* 
    {
       "Name":"Diego Venturin",
       "Email": "diego@email.com",
       "Module": "CS",
       "Status": "completo",
       "Password": "senha"
    } 

    {
       "Content":"Bi89898",
       "UserId": 3
    }

    */


//     var userFound = _context.Users.Find(post.UserId);
// if (userFound != null)
// {
//     _context.Posts.Add(post);
//     _context.SaveChanges();
// }
// else
// {
//     return null;
// }


// return post;