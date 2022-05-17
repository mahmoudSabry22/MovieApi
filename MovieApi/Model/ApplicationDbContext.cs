﻿using Microsoft.EntityFrameworkCore;

namespace MovieApi.Model
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options)
        {

        }

        public DbSet<Genre> Genres { get; set; }
    }
}
