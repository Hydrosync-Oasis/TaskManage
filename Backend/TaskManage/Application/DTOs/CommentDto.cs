namespace Application.Dtos
{
    // 用于评论的统一数据传输对象
    public class CommentDto
    {
        public int CommentId { get; set; }              // 评论 ID，用于展示或删除
        public int TaskId { get; set; }                 // 所属任务 ID
        public int UserId { get; set; }                 // 评论人 ID（可由后端自动注入）
        public string Content { get; set; } = null!;    // 评论内容
        public DateTimeOffset CreatedTime { get; set; }  // 新增创建时间
    }
}