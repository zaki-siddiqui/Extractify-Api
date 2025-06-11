using AutoMapper;
using Extractify.Application.DTOs;
using Extractify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ScrapingTask, ScrapingTaskDto>().ReverseMap();
            CreateMap<ScrapedData, ScrapedDataDto>().ReverseMap();
        }
    }
}
