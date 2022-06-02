using System;
using System.Collections.Generic;

namespace services.utils
{
    public class UserUtils
    {
        public static List<float> ComputePersonalityCoefficients(List<Response> responses)
            // r: list of responses to the 60 personality questions
            // returns the list of personality scores (OPE, COS, EXT, AGR, NEU)
        {
            if (responses.Count < 60)
                throw new Exception("Invalid number of responses: " + responses.Count.ToString() + ". (Should be 60)");

            int agr_points = 0, cos_points = 0, ext_points = 0, neu_points = 0, ope_points = 0;

            foreach(Response response in responses)
            {
                switch (response.Factor)
                {
                    case (PersonalityFactor.OPENNESS):
                        ope_points += response.Coef * response.Value; // minimum -6p, maximum 42p
                        break;
                    case (PersonalityFactor.CONSCIENTIOUSNESS):
                        cos_points += response.Coef * response.Value; // minimum -12p, maximum 36p
                        break;
                    case (PersonalityFactor.EXTRAVERSION):
                        ext_points += response.Coef * response.Value;  // minimum -18p, maximum 30p
                        break;
                    case (PersonalityFactor.AGREEABLENESS):
                        agr_points += response.Coef * response.Value; // minimum -6p, maximum 42p
                        break;
                    case (PersonalityFactor.NEUROTICISM):
                        neu_points += response.Coef * response.Value; // minimum -6p, maximum 42p
                        break;
                }
            }

            int ope_score = (ope_points + 6) * 100 / 48;  // +6  =>  scale 0-48
 
            int cos_score = (cos_points + 12) *100 / 48;  // +12  =>  scale 0-48

            int ext_score = (ext_points + 18) * 100 / 48;  // +18  =>  scale 0-48

            int agr_score = (agr_points + 6) * 100 / 48;  // +6  =>  scale 0-48

            int neu_score = (neu_points + 6) * 100 / 48;  // +6  =>  scale 0-48


            return new List<float> { ope_score, cos_score, ext_score, agr_score, neu_score };
        }
    }
}
