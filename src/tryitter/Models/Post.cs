using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tryitter.Models;
public class Post
{
    [Key]
    [Column("post_id")]
    public int PostId { get; set; }
    public string Content { get; set; } = default!;
    [ForeignKey("UserId")]
    [Column("user_id")]
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
}