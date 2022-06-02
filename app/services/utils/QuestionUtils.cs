using System.Collections.Generic;
using System.IO;

namespace services.utils
{
    public class QuestionUtils
    {
        public static List<Question> ReadQuestions()
        {

            var questions = new List<Question>();
            using (var reader = new StreamReader(@"C:\Users\MSI\Desktop\de_toate\Facultate\Sem2\ELL\TheActualApp\services\utils\data\questions.tsv"))
            {
                reader.ReadLine();
                while (!reader.EndOfStream) // Ignore headers
                {
                    var line = reader.ReadLine();
                    var values = line.Split('\t');

                    var content = values[0];
                    var factor = int.Parse(values[1]);
                    var coef = int.Parse(values[2]);

                    questions.Add(new Question(content, (PersonalityFactor)factor, coef));
                }
            }
            return questions;
        }
    }
}
