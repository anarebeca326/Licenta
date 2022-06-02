using System;
namespace services.utils
{
    public class Question
    {
        public String Content { get; set; }
        public PersonalityFactor RelatedFactor { get; set; }
        public int Coef { get; set; }

        public Question(String content, PersonalityFactor factor, int coef)
        {
            Content = content;
            RelatedFactor = factor;
            Coef = coef;
        }
    }
}
