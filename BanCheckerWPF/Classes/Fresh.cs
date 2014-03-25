namespace BanCheckerWPF.Classes
{
    public class Fresh
    {
        public object Value { get; set; }

        public Fresh(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return "Fresh(" + Value + ")";
        }
    }
}
