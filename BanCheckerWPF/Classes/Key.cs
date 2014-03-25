using System;

namespace BanCheckerWPF.Classes
{
    public class Key
    {
        public Key(string name, string entity1, string entity2)
        {
            Name = name;
            Entity1 = entity1;
            Entity2 = entity2;
        }
        public Key(string name, string entity1, string entity2,bool fresh)
        {
            Name = name;
            Entity1 = entity1;
            Entity2 = entity2;
            Fresh = fresh;
        }

        public string Name { get; set; }
        public string Entity1 { get; set; }
        public string Entity2 { get; set; }
        public bool Fresh { get; set; }

        public override string ToString()
        {
            return Entity1 + "<-" + Name + "->" + Entity2;
        }

        public bool EntityKnowsKey(string e)
        {
            return String.Compare(Entity1, e, StringComparison.Ordinal) == 0 ||
                   String.Compare(Entity2, e, StringComparison.Ordinal) == 0;
        }

        public string GetOtherEntity(string e)
        {
            if (String.Compare(Entity1, e, StringComparison.Ordinal)==0)
            {
                return Entity2;
            }
            if (String.Compare(Entity2, e, StringComparison.Ordinal) == 0)
            {
                return Entity1;
            }
            return "";
        }
    }
}
