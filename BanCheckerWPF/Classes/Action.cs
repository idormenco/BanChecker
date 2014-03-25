namespace BanCheckerWPF.Classes
{
    public class Action
    {

    }

    public class Belives : Action
    {
        public override string ToString()
        {
            return "Belives";
        }
    }

    public class Said : Action
    {
        public override string ToString()
        {
            return "Said";
        }
    }

    public class Received : Action
    {
        public override string ToString()
        {
            return "Received";
        }
    }

    public class Controls : Action
    {
        public override string ToString()
        {
            return "Controls";
        }
         
    }
}
