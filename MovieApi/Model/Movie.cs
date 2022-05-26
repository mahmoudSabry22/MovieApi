namespace MovieApi.Model
{
    public class Movie
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }  
        public int Year { get; set; }

        public double Rate { get; set; }
        public string Storeline { get; set; }
        public byte[] Poster { get; set; }

        public byte GenreId     { get; set; }
        public Genre Genres { get; set; }
    }
}
