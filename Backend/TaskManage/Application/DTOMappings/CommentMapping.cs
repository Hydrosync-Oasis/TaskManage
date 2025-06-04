using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.DTOMappings;

public class CommentMapping {
    public static void Register(TypeAdapterConfig config) {
        config.NewConfig<Comment, CommentDto>()
            .Map(x => x.UserId, y => y.Owner.Id)
            .Map(x => x.TaskId, y => y.Task.Id)
            .Map(x => x.CommentId, y => y.Id);
    }
}
