using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs {
    public class ProjectDto {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int? OwnerUid { get; set; }

        public DateTimeOffset? CreatedAt { get; set; } 
    }
}
