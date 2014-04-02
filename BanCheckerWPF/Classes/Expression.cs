using System.Collections;
using System.Collections.Generic;

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
            return  Entity + " " + Action + " " + X;
        }
    }

    public class ExpressionComparer : IEqualityComparer<Expression>
    {
        public bool Equals(Expression x, Expression y)
        {
            if (x.ToString()==y.ToString())
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Expression obj)
        {
            return 0;
        }
    }
}
