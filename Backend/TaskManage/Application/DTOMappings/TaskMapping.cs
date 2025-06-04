using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.DTOMappings {
    public class TaskMapping {
        public static void Register(TypeAdapterConfig config) {
            config.NewConfig<TaskNode, TaskDto>()
                .Map(x => x.DependencyTaskIds, y => y.DependentNodes.Select(z => z.Id))
                .Map(x => x.Status, y => y.TaskStatus);
        }
    }
}
