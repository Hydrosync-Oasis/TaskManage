using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public required User Owner { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<TaskNode> Tasks { get; set; } = [];
    }
}
