using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.utils
{
    public class UserUtils
    {
        public static List<float> ComputePersonalityCoefficients(List<int> r)
            // r: list of responses to the 60 personality questions
            // returns the list of personality scores (OPE, COS, EXT, AGR, NEU)
        {
            if (r.Count < 60)
                throw new Exception("Invalid number of responses: " + r.Count.ToString() + ". (Should be 60)");

            int agr_points = r[0] - r[5] + r[10] + r[15] + r[20] - r[25] - r[30] + r[35] + r[43] + r[48] + r[53] + r[58];  // minimum -6p, maximum 42p
            int agr_score = (agr_points + 6) * 100 / 48;  // +6  =>  scale 0-48

            int cos_points = r[1] - r[6] + r[11] + r[16] + r[21] + r[26] - r[31] - r[36] + r[41] - r[46] + r[51] + r[56];  // minimum -12p, maximum 36p
            int cos_score = (cos_points + 12) *100 / 48;  // +12  =>  scale 0-48

            int ext_points = r[2] + r[7] + r[12] + r[17] - r[22] + r[27] - r[32] - r[37] - r[42] - r[47] + r[52] + r[57];  // minimum -18p, maximum 30p
            int ext_score = (ext_points + 18) * 100 / 48;  // +18  =>  scale 0-48

            int neu_points = r[3] + r[8] + r[13] - r[18] + r[23] + r[28] - r[33] - r[38] + r[44] + r[49] + r[54] + r[59];  // minimum -6p, maximum 42p
            int neu_score = (neu_points + 6) * 100 / 48;  // +6  =>  scale 0-48

            int ope_points = r[4] + r[9] + r[14] + r[19] + r[24] - r[29] - r[34] - r[39] + r[40] + r[45] + r[50] + r[55];  // minimum -6p, maximum 42p
            int ope_score = (ope_points + 6) * 100 / 48;  // +6  =>  scale 0-48

            return new List<float> { ope_score, cos_score, ext_score, agr_score, neu_score };
        }
    }
}
