using System.Collections.Generic;
using BanCheckerWPF.Classes;

namespace BanCheckerWPF
{
    class BanRules
    {
        public List<Expression> ApplyRule(Expression expression1, Expression expression2)
        {
            var result = new List<Expression>();
            if (MessageMeaning(expression1, expression2) != null)
            {
                result.AddRange(MessageMeaning(expression1, expression2));
            }
            if (NonceVerification(expression1, expression2) != null)
            {
                result.Add(NonceVerification(expression1, expression2));
            }

            if (Jurisdiction(expression1, expression2) != null)
            {
                result.Add(Jurisdiction(expression1, expression2));
            }
            if (BeliefConjuncatenation(expression1, expression2) != null)
            {
                result.AddRange(BeliefConjuncatenation(expression1, expression2));
            }
            if (ReceivingRule(expression1, expression2) != null)
            {
                result.AddRange(ReceivingRule(expression1, expression2));
            }
            if (FreshnessConjuncatenation(expression1, expression2) != null)
            {
                result.Add(FreshnessConjuncatenation(expression1, expression2));
            }
            return result;
        }

        public List<Expression> MessageMeaning(Expression e1, Expression e2)
        {
            List<Expression> result = new List<Expression>();
            if (e2 != null && (e1 != null && (e1.Action.GetType() == typeof(Belives) && e1.X.GetType() == typeof(Key) && ((Key)(e1.X)).EntityKnowsKey(e1.Entity)
                                              && e1.Entity == e2.Entity && e2.Action.GetType() == typeof(Received) && e2.X.GetType() == typeof(EncryptedMessage)
                                              && ((Key)(e1.X)).Name == ((EncryptedMessage)e2.X).Key)))
            {
                var message = new Message(((EncryptedMessage)(e2.X)).MessageList, false);
                var expresion = new Expression(((Key)(e1.X)).GetOtherEntity(e1.Entity), new Said(), message);
                result.Add(new Expression(e1.Entity, new Belives(), expresion));
                result.Add(new Expression(e1.Entity, new Received(), message));
            }

            if (e1 != null && (e2 != null && (e2.Action.GetType() == typeof(Belives) && e2.X.GetType() == typeof(Key) && ((Key)(e2.X)).EntityKnowsKey(e2.Entity)
                                              && e1.Entity == e2.Entity && e1.Action.GetType() == typeof(Received) && e1.X.GetType() == typeof(EncryptedMessage)
                                              && ((Key)(e2.X)).Name == ((EncryptedMessage)e1.X).Key)))
            {
                var message = new Message(((EncryptedMessage)(e1.X)).MessageList, false);
                var expresion = new Expression(((Key)(e2.X)).GetOtherEntity(e2.Entity), new Said(), message);
                result.Add(new Expression(e2.Entity, new Belives(), expresion));
                result.Add(new Expression(e2.Entity,new Received(), message));
            }

            if (e2 != null && (e1 != null && (e1.Action.GetType() == typeof(Belives) && e1.X.GetType() == typeof(PublicKey)
                                              && e1.Entity == e2.Entity && e2.Action.GetType() == typeof(Received) && e2.X.GetType() == typeof(EncryptedMessage)
                                              && ((PublicKey)(e1.X)).GetKeyInverse() == ((EncryptedMessage)e2.X).Key)))
            {
                var message = new Message(((EncryptedMessage)(e2.X)).MessageList, false);
                var expresion = new Expression(((PublicKey)(e1.X)).Entity, new Said(), message);
                result.Add(new Expression(e1.Entity, new Belives(), expresion));
                result.Add(new Expression(e1.Entity, new Received(), message));
            }

            if (e1 != null && (e2 != null && (e2.Action.GetType() == typeof(Belives) && e2.X.GetType() == typeof(PublicKey)
                                              && e1.Entity == e2.Entity && e1.Action.GetType() == typeof(Received) && e1.X.GetType() == typeof(EncryptedMessage)
                                              && ((PublicKey)(e2.X)).GetKeyInverse() == ((EncryptedMessage)e1.X).Key)))
            {
                var message = new Message(((EncryptedMessage)(e1.X)).MessageList, false);
                var expresion = new Expression(((PublicKey)(e2.X)).Entity, new Said(), message);
                result.Add(new Expression(e2.Entity, new Belives(), expresion));
                result.Add(new Expression(e2.Entity, new Received(), message));
            }
            return result;
        }

        public Expression NonceVerification(Expression e1, Expression e2)
        {
            if (e1 != null && e2 != null && (e1.Action.GetType() == typeof(Belives) && e1.X.GetType() == typeof(Fresh) && e2.Action.GetType() == typeof(Belives) && e2.X.GetType() == typeof(Expression) && e1.Entity == e2.Entity
                               && ((Expression)e2.X).Action.GetType() == typeof(Said) && ((Fresh)e1.X).Value.ToString() == ((Expression)e2.X).X.ToString()))
            {
                var expr = new Expression(((Expression)e2.X).Entity, new Belives(), ((Expression)e2.X).X);
                return new Expression(e1.Entity, new Belives(), expr);
            }
            if (e1 != null && (e2 != null && (e2.Action.GetType() == typeof(Belives) && e2.X.GetType() == typeof(Fresh) && e1.Action.GetType() == typeof(Belives) && e1.X.GetType() == typeof(Expression) && e2.Entity == e1.Entity
                                              && ((Expression)e1.X).Action.GetType() == typeof(Said) && ((Fresh)e2.X).Value.ToString() == ((Expression)e1.X).X.ToString())))
            {
                var expr = new Expression(((Expression)e1.X).Entity, new Belives(), ((Expression)e1.X).X);
                return new Expression(e2.Entity, new Belives(), expr);
            }
            return null;
        }

        public Expression Jurisdiction(Expression e1, Expression e2)
        {
            if (e2 != null && e1 != null && e1.Action.GetType() == typeof(Belives) && e2.Action.GetType() == typeof(Belives)
                && e1.X.GetType() == typeof(Expression) && e2.X.GetType() == typeof(Expression)
                                              && e1.Entity == e2.Entity
                                              && ((Expression)e1.X).Action.GetType() == typeof(Controls)
                                              && ((Expression)e2.X).Action.GetType() == typeof(Belives)
                                              && ((Expression)e1.X).Entity == ((Expression)e2.X).Entity
                                              && ((Expression)e1.X).X.ToString() == ((Expression)e2.X).X.ToString())
            {
                return new Expression(e1.Entity, new Belives(), ((Expression)e1.X).X);
            }

            if (e1 != null && e2 != null && e2.Action.GetType() == typeof(Belives) && e1.Action.GetType() == typeof(Belives)
                && e1.X.GetType() == typeof(Expression) && e2.X.GetType() == typeof(Expression)
                                              && e1.Entity == e2.Entity
                                              && ((Expression)e2.X).Action.GetType() == typeof(Controls)
                                              && ((Expression)e1.X).Action.GetType() == typeof(Belives)
                                              && ((Expression)e2.X).Entity == ((Expression)e1.X).Entity
                                              && ((Expression)e2.X).X.ToString() == ((Expression)e1.X).X.ToString())
            {
                return new Expression(e2.Entity, new Belives(), ((Expression)e2.X).X);
            }
            return null;
        }

        public List<Expression> BeliefConjuncatenation(Expression e1, Expression e2)
        {
            if (e2 == null && e1.Action.GetType() == typeof(Belives) && e1.X.GetType() == typeof(Expression) && ((Expression)e1.X).X.GetType() == typeof(Message))
            {
                var list = new List<Expression>();
                List<object> messageList = ((Message)((Expression)e1.X).X).MessageList;
                foreach (var o in messageList)
                {
                    var exp = new Expression(((Expression)e1.X).Entity, ((Expression)e1.X).Action, o);
                    var expresion = new Expression(e1.Entity, e1.Action, exp);
                    list.Add(expresion);
                }
                return list;
            }
            if (e2 != null && (e1 != null && (e1.Entity == e2.Entity && e1.Action.GetType() == typeof(Belives) && e2.Action.GetType() == typeof(Belives)
                                              && e1.X.GetType() == typeof(Message) && e2.X.GetType() == typeof(Message))))
            {
                List<object> l1 = new List<object>(((Message)e1.X).MessageList);
                List<object> l2 = new List<object>(((Message)e2.X).MessageList);
                l1.AddRange(l2);
                return new List<Expression> { new Expression(e1.Entity, new Belives(), new Message(l1, false)) };
            }
            return null;
        }
        public List<Expression> ReceivingRule(Expression e1, Expression e2)
        {
            if (e2 == null && e1.Action.GetType() == typeof(Received) && e1.X.GetType() == typeof(Message))
            {
                var list = new List<Expression>();
                List<object> messageList = ((Message) e1.X).MessageList;
                foreach (var o in messageList)
                {
                    var exp = new Expression(e1.Entity, new Received(), o);
                    list.Add(exp);
                }
                return list;
            }
            return null;
        }

        public Expression FreshnessConjuncatenation(Expression e1, Expression e2)
        {
            if (e2==null && e1.Action.GetType() == typeof(Belives) && e1.X.GetType() == typeof(Message) )
            {
                var message = ((Message)(e1.X)).MessageList;
                foreach (var o in message)
                {
                    if (o.GetType() == typeof(Fresh))
                    {
                        return new Expression(e1.Entity, e1.Action, new Fresh(new Message(message,true)));
                    }
                }
            }
            return null;
        }

    }
}