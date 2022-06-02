using model.many_to_many;
using model.types;
using System;
using System.Collections.Generic;

namespace model
{
    public class Book
    {
        public int ID { get; set; }
        public String Title { get; set; }
        public String Author { get; set; }
        public String Description { get; set; }
        public float CommunalCoef { get; set; }
        public float AesthetiCoef { get; set; }
        public float DarkCoef { get; set; }
        public float ThrillingCoef { get; set; }
        public float CerebralCoef { get; set; }
        public FeatureClass Communal { get; set; }
        public FeatureClass Aesthetic { get; set; }
        public FeatureClass Dark { get; set; }
        public FeatureClass Thrilling { get; set; }
        public FeatureClass Cerebral { get; set; }
        public int NoRatings { get; set; }
        public String CoverImage { get; set; }

        public virtual ICollection<Shelf> Readers { get; set; }
        public virtual ICollection<Favourite> Enjoyers { get; set; }
        public virtual ICollection<Rated> RatedBy { get; set; }


        public Book() { }
        public Book(String title, String author, String description,
            float communal, float aesthetic, float dark, float thrilling, float cerebral)
        {
            Title = title;
            Author = author;
            Description = description;
            CommunalCoef = communal;
            AesthetiCoef = aesthetic;
            DarkCoef = dark;
            ThrillingCoef = thrilling;
            CerebralCoef = cerebral;
            NoRatings = 1;

            ComputeFeatureClasses();
        }

        public Book(String title, String author, String description,
            float communal, float aesthetic, float dark, float thrilling, float cerebral, String cover_path)
        {
            Title = title;
            Author = author;
            Description = description;
            CommunalCoef = communal;
            AesthetiCoef = aesthetic;
            DarkCoef = dark;
            ThrillingCoef = thrilling;
            CerebralCoef = cerebral;
            CoverImage = cover_path;
            NoRatings = 1;

            ComputeFeatureClasses();
        }

        private void ComputeFeatureClasses()
        {
            var mean = (CommunalCoef + AesthetiCoef + DarkCoef + ThrillingCoef + CerebralCoef) / 5;
            Communal = GetClassForFeature(mean, CommunalCoef);
            Aesthetic = GetClassForFeature(mean, AesthetiCoef);
            Dark = GetClassForFeature(mean, DarkCoef);
            Thrilling = GetClassForFeature(mean, ThrillingCoef);
            Cerebral = GetClassForFeature(mean, CerebralCoef);

        }

        private static FeatureClass GetClassForFeature(float mean, float feature_coef)
        {
            if (feature_coef > mean + 1)
                return FeatureClass.POSITIVE;
            if (feature_coef < mean - 1)
                return FeatureClass.NEGATIVE;
            return FeatureClass.NEUTRAL;
        }

    }
}
