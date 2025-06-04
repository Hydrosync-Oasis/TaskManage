namespace Domain.Exceptions.Domain
{
    public class TaskCyclicDependencyException() : TaskDependencyException("任务中出现了环形依赖");
}
