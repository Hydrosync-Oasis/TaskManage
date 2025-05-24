using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITaskService
    {
        public Task UpdateTask(TaskDto dto);

        /// <summary>
        /// 创建一个任务
        /// </summary>
        /// <param name="dto">任务描述对象</param>
        /// <param name="uid">创建者id</param>
        /// <returns></returns>
        public Task<int> AddTask(TaskDto dto, int uid);

        public Task RemoveTask(int taskId);

        public Task<TaskDto> GetTaskInfo(int taskId);

        public Task<TaskNode> GetTaskNodeByIdAsync(int id);

        public Task AddCommentAsync(Comment comment);         // 添加评论（仍需 Comment 对象）

        public Task<Comment> GetCommentByIdAsync(int id);     // 通过评论ID获取评论

        public Task DeleteCommentAsync(int id);               // 通过评论ID删除评论

        public Task<List<Comment>> GetAllCommentsByTaskIdAsync(int taskId);                //通过任务ID获取所有评论
    }
}

