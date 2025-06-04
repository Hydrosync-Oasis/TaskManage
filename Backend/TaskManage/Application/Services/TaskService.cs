using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions.Domain;
using Domain.Exceptions.Dto;
using Domain.Repository;
using Mapster;
using TaskStatus = Domain.Entities.TaskStatus;

namespace Application.Services
{
    public class TaskService(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        ICommentRepository commentRepository)
        : ITaskService {
        public async Task UpdateTask(TaskDto dto)
        {
            if (dto.Id == null) throw new ArgumentNullException(nameof(dto.Id));
            var task = await taskRepository.GetTaskById(dto.Id.Value);
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.Status != null) {
                if (!CheckStatusValid(task, dto.Status.Value)) {
                    throw new TaskDependencyException("当依赖节点未完成时，无法设置当前任务状态为非Ready，或当前任务未完成时，必须设置后继节点均为Ready。");
                };
                task.TaskStatus = dto.Status;
            }

            if (dto.Deadline != null) {
                if (!IsDateValid(task, dto.Deadline.Value)) {
                    throw new TaskDependencyException("任务截止时间只能比依赖任务的截止时间晚，且比后继节点早。");
                }
                task.Deadline = dto.Deadline.Value;
            }

            if (dto.Priority != null) task.Priority = dto.Priority.Value;

            if (dto.AssignedUid != null)
            {
                // 注意这里修正为通过 AssignedUid 查用户
                task.AssignedUser = await userRepository.GetUserByIdAsync(dto.AssignedUid.Value);
            }

            if (dto.DependencyTaskIds != null)
            {
                var tasks = await taskRepository.GetAllTasksByProjectId(task.ProjectId);
                var newDependencyTasks = dto.DependencyTaskIds.Select(x => tasks.First(y => y.Id == x)).ToList();

                if (HasCircle(task, newDependencyTasks))
                {
                    throw new TaskCyclicDependencyException();
                }

                task.DependentNodes = newDependencyTasks;
            }

            await taskRepository.UpdateAsync(task);
        }

        private static bool IsDateValid(TaskNode modifiedNode, DateTimeOffset time) {
            var dep = modifiedNode.DependentNodes;
            var suc = modifiedNode.SuccessorNodes;
            var depRes = dep.All(taskNode => taskNode.Deadline <= time);
            var sucRes = suc.All(taskNode => taskNode.Deadline >= time);
            return depRes && sucRes;
        }

        private static bool CheckStatusValid(TaskNode modifiedNode, TaskStatus status) {
            var dep = modifiedNode.DependentNodes;
            var suc = modifiedNode.SuccessorNodes;
            return status == TaskStatus.Ready 
                ? suc.All(x => x.TaskStatus == TaskStatus.Ready) 
                : dep.All(x => x.TaskStatus == TaskStatus.Done);
        }

        private static bool HasCircle(TaskNode modifiedNode, List<TaskNode> modifiedDependencyNodes)
        {
            bool F(TaskNode node, HashSet<TaskNode> seen, HashSet<TaskNode> stackSet)
            {
                if (!seen.Add(node))
                    return false;

                stackSet.Add(node);

                foreach (var d in GetDependencyList(node))
                {
                    if (stackSet.Contains(d))
                        return true;

                    if (F(d, seen, stackSet))
                        return true;
                }

                stackSet.Remove(node);
                return false;
            }

            List<TaskNode> GetDependencyList(TaskNode node)
            {
                return node == modifiedNode ? modifiedDependencyNodes : node.DependentNodes;
            }

            return F(modifiedNode, new HashSet<TaskNode>(), new HashSet<TaskNode>());
        }

        /// <summary>
        /// 添加节点，绝不可能出现环
        /// </summary>
        /// <param name="dto">任务节点DTO</param>
        /// <param name="uid">创建用户时的用户id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">若DTO</exception>
        public async Task<int> AddTask(TaskDto dto, int uid)
        {
            if (dto.ProjectId == null) throw new DtoFieldNullException(nameof(dto), nameof(dto.ProjectId));
            if (dto.Priority == null) throw new DtoFieldNullException(nameof(dto), nameof(dto.Priority));
            if (dto.Deadline == null) throw new DtoFieldNullException(nameof(dto), nameof(dto.Deadline));
            if (dto.Title == null) throw new DtoFieldNullException(nameof(dto), nameof(dto.Title));

            dto.DependencyTaskIds ??= [];

            var node = new TaskNode
            {
                Title = dto.Title,
                AssignedUserId = dto.AssignedUid,
                Deadline = dto.Deadline.Value,
                TaskStatus = dto.Status,
                Description = dto.Description,
                Priority = dto.Priority.Value,
                ProjectId = dto.ProjectId.Value,
                CreateUserId = uid,
                DependentNodes = dto.DependencyTaskIds.Select((x)=> GetTaskNodeByIdAsync(x).Result).ToList(),
            };

            return await taskRepository.AddAsync(node);
        }

        public Task RemoveTask(int taskId)
        {
            return taskRepository.DeleteAsync(taskId);
        }

        public async Task<TaskDto> GetTaskInfo(int taskId)
        {
            var taskNode = await taskRepository.GetTaskById(taskId);
            if (taskNode is null) {
                throw new KeyNotFoundException($"没有{taskId}对应的任务");
            }
            return taskNode.Adapt<TaskDto>();
        }

        public async Task<bool> IsTaskExists(int taskId) {
            return await taskRepository.IsTaskExists(taskId);
        }

        private async Task<TaskNode> GetTaskNodeByIdAsync(int taskId) {
            var node = await taskRepository.GetTaskById(taskId);
            if (node == null)
                throw new KeyNotFoundException($"没有{taskId}对应的任务");
            return node;
        }

        public async Task AddCommentAsync(CommentDto commentDto)
        {
            var user = await userRepository.GetUserByIdAsync(commentDto.UserId);
            var task = await taskRepository.GetTaskById(commentDto.TaskId);
            if (user is null) {
                throw new DtoFieldOutOfRangeException(nameof(commentDto), nameof(commentDto.UserId));
            }
            if (task is null) {
                throw new DtoFieldOutOfRangeException(nameof(commentDto), nameof(commentDto.TaskId));
            }

            var comment = new Comment() {
                Content = commentDto.Content,
                CreatedTime = commentDto.CreatedTime,
                Id = commentDto.TaskId,
                Owner = user,
                Task = task
            };
            await commentRepository.AddAsync(comment);
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int id)
        {
            return (await commentRepository.GetCommentByIdAsync(id)
                )?.Adapt<CommentDto>();
        }

        public Task DeleteCommentAsync(int id)
        {
            return commentRepository.DeleteByCommentIdAsync(id);
        }

        //通过任务ID获取所有评论
        public async Task<List<CommentDto>> GetAllCommentsByTaskIdAsync(int taskId)
        {
            return (await commentRepository.GetAllCommentsByTaskIdAsync(taskId))
                .Adapt<List<CommentDto>>();
        }

    }


}