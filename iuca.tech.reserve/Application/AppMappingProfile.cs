using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<Client, ClientDTO>().ReverseMap();
        CreateMap<Request, RequestDTO>().ReverseMap();
        CreateMap<Equipment, EquipmentDTO>().ReverseMap();
        CreateMap<EquipmentRequest, EquipmentRequestDTO>().ReverseMap();
    }
}
