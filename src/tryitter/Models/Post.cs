using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tryitter.Models;
public class Post
{

    [Column("post_id")]
    [Key]
    public int PostId { get; set; }
    [MaxLength(300)]
    public string Content { get; set; } = default!;

    [Column("user_id")]
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public virtual User? User { get; set; } = default!;
}

public class PostDTO
{
    public int PostId { get; set; }
    [MaxLength(300)]
    public string Content { get; set; } = default!;
    public int UserId { get; set; }
}

public class PostUser
{
    [MaxLength(300)]
    public string Content { get; set; } = default!;
}

