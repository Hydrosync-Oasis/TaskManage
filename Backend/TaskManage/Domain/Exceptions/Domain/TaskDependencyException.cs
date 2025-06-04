namespace Domain.Exceptions.Domain
{
    /// <summary>
    /// 任务依赖问题，可以是任何属性在依赖关系上出现了冲突。
    /// </summary>
    /// <param name="message">冲突具体消息</param>
    public class TaskDependencyException(string message) : Exception(message);
}
