namespace BanCheckerWPF.Classes
{
    public class Nonce
    {
        public Nonce(string name)
        {
            Name = name;
        }
        public Nonce(string name,bool fresh)
        {
            Name = name;
            Fresh = fresh;
        }

        public string Name { get; set; }
        public bool Fresh { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
