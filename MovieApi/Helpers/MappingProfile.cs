using AutoMapper;


namespace MovieApi.Helpers
{
    public class MappingProfile :Profile
    {

        public MappingProfile()
        {
            CreateMap<Movie, MovieDetailsDto>();
                //.ForMember(dest => dest.GenreName, src => src.MapFrom(src => src.Genres.Name));

            CreateMap<MovieDto, Movie>()
                .ForMember(src =>src.Poster, opt =>opt.Ignore());
        }
    }
}
