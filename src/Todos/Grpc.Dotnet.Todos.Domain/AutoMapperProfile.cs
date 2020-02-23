using AutoMapper;
using Grpc.Dotnet.Todos.Domain.Commands;
using Grpc.Dotnet.Todos.Domain.Result;

namespace Grpc.Dotnet.Todos.Domain.Domain
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateTodoCommand, Todo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Todo, TodoResult>()
                // all this is not necessary, but it's here to remind what we can do
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsComplete, opt => opt.MapFrom(src => src.IsComplete))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
