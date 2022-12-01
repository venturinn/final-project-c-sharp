using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tryitter.Models;
public class Post
{

    [Column("post_id")]
    [Key]
    public int PostId { get; set; }
    public string Content { get; set; } = default!;

    [Column("user_id")]
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
}