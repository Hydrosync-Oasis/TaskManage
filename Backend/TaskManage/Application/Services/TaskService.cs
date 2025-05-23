﻿using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repository;
using Mapster;

namespace Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;
        private readonly IUserRepository userRepository;
        private readonly ICommentRepository commentRepository;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository, ICommentRepository commentRepository)
        {
            this.taskRepository = taskRepository;
            this.userRepository = userRepository;
            this.commentRepository = commentRepository;
        }

        public async Task UpdateTask(TaskDto dto)
        {
            if (dto.Id == null) throw new ArgumentNullException(nameof(dto.Id));
            var task = await taskRepository.GetNodeById(dto.Id.Value);
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.Status != null) task.TaskStatus = dto.Status;
            if (dto.Deadline != null) task.Deadline = dto.Deadline.Value;
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
                    throw new Exception("出现了环形依赖");
                }

                task.DependentNodes = newDependencyTasks;
            }

            await taskRepository.UpdateAsync(task);
        }

        private static bool HasCircle(TaskNode startNode, List<TaskNode> modifiedDependencyNodes)
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
                return node == startNode ? modifiedDependencyNodes : node.DependentNodes;
            }

            return F(startNode, new HashSet<TaskNode>(), new HashSet<TaskNode>());
        }

        public async Task<int> AddTask(TaskDto dto, int uid)
        {
            if (dto.ProjectId == null) throw new ArgumentNullException(nameof(dto.ProjectId));
            if (dto.Priority == null) throw new ArgumentNullException(nameof(dto.Priority));
            if (dto.Deadline == null) throw new ArgumentNullException(nameof(dto.Deadline));
            if (dto.Title == null) throw new ArgumentNullException(nameof(dto.Title));

            var node = new TaskNode
            {
                Title = dto.Title,
                AssignedUserId = dto.AssignedUid,
                Deadline = dto.Deadline.Value,
                TaskStatus = dto.Status,
                Description = dto.Description,
                Priority = dto.Priority.Value,
                ProjectId = dto.ProjectId.Value,
                CreateUserId = uid
            };

            return await taskRepository.AddAsync(node);
        }

        public Task RemoveTask(int taskId)
        {
            return taskRepository.DeleteAsync(taskId);
        }

        public async Task<TaskDto> GetTaskInfo(int taskId)
        {
            var taskNode = await taskRepository.GetNodeById(taskId);
            return taskNode.Adapt<TaskDto>();
        }

        public async Task<TaskNode> GetTaskNodeByIdAsync(int id)
        {
            var node = await taskRepository.GetNodeById(id);
            if (node == null) throw new ArgumentNullException(nameof(node));
            return node;
        }

        // 评论相关实现
        public Task AddCommentAsync(Comment comment)
        {
            return commentRepository.AddAsync(comment);
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            var comments = await commentRepository.GetAllCommentsByTaskIdAsync(id);
            var comment = comments.FirstOrDefault(c => c.Id == id);
            return comment ?? throw new Exception($"找不到 ID 为 {id} 的评论");
        }


        public Task DeleteCommentAsync(int id)
        {
            return commentRepository.DeleteByCommentIdAsync(id);
        }

        //通过任务ID获取所有评论
        public async Task<List<Comment>> GetAllCommentsByTaskIdAsync(int taskId)
        {
            return await commentRepository.GetAllCommentsByTaskIdAsync(taskId);
        }

    }


}