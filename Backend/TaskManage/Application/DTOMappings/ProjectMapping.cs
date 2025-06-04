using Application.Dtos;
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
