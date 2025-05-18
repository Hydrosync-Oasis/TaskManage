using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services {
    public class TaskService(ITaskRepository taskRepository, IUserRepository userRepository) : ITaskService {
        public async Task UpdateTask(TaskDto dto) {
            ArgumentNullException.ThrowIfNull(dto.Id);
            var task = await taskRepository.GetNodeById(dto.Id.Value);
            ArgumentNullException.ThrowIfNull(task);
            if (dto.Title is not null) {
                task.Title = dto.Title;
            }
            if (dto.Description is not null) {
                task.Description = dto.Description;
            }
            if (dto.Status is not null) {
                task.TaskStatus = dto.Status;
            }
            if (dto.Deadline is not null) {
                task.Deadline = dto.Deadline.Value;
            }
            if (dto.Priority is not null) {
                task.Priority = dto.Priority.Value;
            }
            if (dto.AssignedUid is not null) {
                task.AssignedUser = await userRepository.GetUserByIdAsync(dto.Id.Value);
            }

            if (dto.DependencyTaskIds is not null) {
                var tasks = await taskRepository.GetAllTasksByProjectId(task.Project.Id);
                var newDependencyTasks =
                    dto.DependencyTaskIds.Select(x=>tasks.First(y=>y.Id == x)).ToList();

                if (HasCircle(tasks.First(x => x.Id == task.Id), newDependencyTasks)) {
                    throw new Exception("出现了环形依赖");
                }

                // 说明没有依赖

                task.DependentNodes = newDependencyTasks;
            }

            await taskRepository.UpdateAsync(task);
        }

        private static bool HasCircle(TaskNode startNode, List<TaskNode> modifiedDependencyNodes) {
            // 判断从startNode开始是否会dfs到它自己
            var res = F(startNode, [startNode], [startNode]);
            return res;

            //是否有环
            bool F(TaskNode node, HashSet<TaskNode> seen, HashSet<TaskNode> stackSet) {
                if (!seen.Add(node)) {
                    return false;
                }
                foreach (var d in GetDependencyList(node)) {
                    stackSet.Add(d);

                    if (F(d, seen, stackSet)) {
                        return true;
                    }

                    stackSet.Remove(d);
                }

                return false;
            }

            List<TaskNode> GetDependencyList(TaskNode node) {
                return node == startNode ? modifiedDependencyNodes : node.DependentNodes;
            }
        }

        public async Task<int> AddTask(TaskDto dto, int uid) {
            ArgumentNullException.ThrowIfNull(dto.ProjectId);
            ArgumentNullException.ThrowIfNull(dto.Priority);
            ArgumentNullException.ThrowIfNull(dto.Deadline);
            ArgumentNullException.ThrowIfNull(dto.Title);
            
            TaskNode node = new() {
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

        public Task RemoveTask(int taskId) {
            return taskRepository.DeleteAsync(taskId);
        }

        public async Task<TaskDto> GetTaskInfo(int taskId) {
            return (await taskRepository.GetNodeById(taskId)).Adapt<TaskDto>();
        }
    }
}
