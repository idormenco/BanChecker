namespace BanCheckerWPF.Classes
{
    public class Expression
    {
        public Expression()
        {
        }

        public Expression(string entity, Action action, object x)
        {
            Entity = entity;
            Action = action;
            X = x;
        }

        public string Entity { get; set; }
        public Action Action { get; set; }
        public object X { get; set; }

        public override string ToString()
        {
            return Entity + " " + Action + " " + X;
        }
    }
}
