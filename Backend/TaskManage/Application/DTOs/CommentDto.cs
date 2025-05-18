namespace Application.Dtos
{
    // 用于前端提交新评论的数据传输对象
    public class CommentCreateDto
    {
        public int TaskId { get; set; }
        public string Content { get; set; } = null!;
    }
}
