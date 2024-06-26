﻿namespace ExhibitionsService.DAL.Entities
{
    public class Painting
    {
        public int PaintingId { get; set; }
        public string Name { get; set;}
        public string Description { get; set;}
        public DateTime CretionDate { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string ImagePath { get; set; }
        public string? Location { get; set; }
        public bool? IsSold { get; set; }
        public decimal? Price { get; set; }

        public int PainterId { get; set; }
        public Painter Painter { get; set; }

        public ICollection<Genre> Genres { get; set; } = [];
        public ICollection<Style> Styles { get; set; } = [];
        public ICollection<Material> Materials { get; set; } = [];
        public ICollection<Tag> Tags { get; set; } = [];

        public ICollection<PaintingRating> Ratings { get; set; } = [];

        public ICollection<PaintingLike> PaintingLikes { get; set; } = [];
        public ICollection<UserProfile> Likers { get; set; } = [];

        public ICollection<ExhibitionApplication> ExhibitionApplications { get; set;} = [];
        public ICollection<Exhibition> Exhibitions { get; set; } = [];

        public ICollection<ContestApplication> ContestApplications { get; set; } = [];
        public ICollection<Contest> Contests { get; set; } = [];
    }
}
