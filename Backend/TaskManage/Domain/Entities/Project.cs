﻿namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public required User Owner { get; set; }
        public required int OwnerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<TaskNode> Tasks { get; set; } = [];
    }
}
