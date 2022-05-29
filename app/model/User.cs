using model.many_to_many;
using model.types;
using System;
using System.Collections.Generic;

namespace model
{
    public class User
    {
        public int ID { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public float Openness { get; set; }
        public float Conscientiousness { get; set; }
        public float Extraversion { get; set; }
        public float Agreeableness { get; set; }
        public float Neuroticism { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }

        public ICollection<Shelf> BooksOnShelf { get; set; }
        public ICollection<Favourite> Favourites { get; set; }


        public User() { }
        public User(String username, String password, Gender gender, int age, float ope, float cos, float ext, float agr, float neu)
        {
            Username = username;
            Password = password;
            Openness = ope;
            Conscientiousness = cos;
            Extraversion = ext;
            Agreeableness = agr;
            Neuroticism = neu;
            Gender = gender;
            Age = age;
        }

        //public void ResetPersonalityTest(float ope, float cos, float ext, float agr, float neu)
        //{
        //    Openness = ope;
        //    Conscientiousness = cos;
        //    Extraversion = ext;
        //    Agreeableness = agr;
        //    Neuroticism = neu;
        //}

    }
}
