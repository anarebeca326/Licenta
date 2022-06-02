namespace services.utils
{
    public class Response
    {
        public int Value { get; set; }
        public PersonalityFactor Factor { get; set; }
        public int Coef {get; set;}

        public Response(int value, PersonalityFactor factor, int coef)
        {
            Value = value;
            Factor = factor;
            Coef = coef;
        }
    }
}
