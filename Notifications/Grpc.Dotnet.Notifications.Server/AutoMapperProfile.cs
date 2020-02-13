
using AutoMapper;
using Grpc.Dotnet.Notifications.Server.Domain.Commands;
using Grpc.Dotnet.Notifications.V1;

namespace Grpc.Dotnet.Notifications.Server
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SendEmailRequest.Types.Recipient, string>()
                .ConvertUsing(source => source.Email ?? string.Empty);

            CreateMap<SendEmailRequest, SendEmailNotificationCommand>()
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(o => o.BodyType))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(o => o.Body.Text))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(o => o.Subject.Text))
                .ForMember(dest => dest.Recipients, opt => opt.MapFrom(o => o.Recipients));

            CreateMap<SendPushRequest.Types.Recipient, string>()
                .ConvertUsing(source => source.Name ?? string.Empty);

            CreateMap<SendPushRequest, SendPushNotificationCommand>()
                .ForMember(dest => dest.Notification, opt => opt.MapFrom(o => o.Message.Text))
                .ForMember(dest => dest.Recipients, opt => opt.MapFrom(o => o.Recipients));
        }
    }
}
