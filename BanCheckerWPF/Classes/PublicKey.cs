namespace BanCheckerWPF.Classes
{
    public class PublicKey
    {
        public string Entity { get; set; }
        public string Name { get; set; }
        public bool Fresh { get; set; }

        public PublicKey(string entity, string name)
        {
            Entity = entity;
            Name = name;
        }

        public PublicKey(string entity, string name,bool fresh)
        {
            Entity = entity;
            Name = name;
            Fresh = fresh;
        }

        public string GetKeyInverse()
        {
            return Name + "^-1";
        }
        public override string ToString()
        {
            return "PK(" + Entity + "," + Name + ")";
        }
    }
}
