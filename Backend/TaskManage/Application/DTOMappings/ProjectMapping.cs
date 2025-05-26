using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Mapster;

namespace Application.DTOMappings {
    public class ProjectMapping {
        public static void Register(TypeAdapterConfig config) {
            config.NewConfig<Project, ProjectDto>()
                .Map(x => x.OwnerUid, x => x.OwnerId);
        }
    }
}
