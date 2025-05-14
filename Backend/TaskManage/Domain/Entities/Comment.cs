using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment
    {
        public TaskNode Task { get; set; }

        public int Id { get; set; }

        public required string Content { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public required User Owner { get; set; }
        public bool IsHidden { get; set; }
    }
}
