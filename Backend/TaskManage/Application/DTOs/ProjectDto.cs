﻿namespace Application.Dtos {
    public class ProjectDto {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int? OwnerUid { get; set; }

        public DateTimeOffset? CreatedAt { get; set; } 
    }
}
