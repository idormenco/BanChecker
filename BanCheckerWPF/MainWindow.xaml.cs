using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using BanCheckerWPF.Classes;
using Action = BanCheckerWPF.Classes.Action;
using Expression = BanCheckerWPF.Classes.Expression;

namespace BanCheckerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Expression> InitialAssumtionsCollection { get; set; }
        public ObservableCollection<Expression> AnnotatedProtocolCollection { get; set; }
        public BanRules BanRules = new BanRules();
        public ExpressionComparer Ec = new ExpressionComparer();
        public HSComparer HsComparer = new HSComparer();
        public HashSet<HashSet<string>> forPrint;
        public HashSet<Expression> WorkingList;
        public MainWindow()
        {
            InitialAssumtionsCollection = new ObservableCollection<Expression>();
            AnnotatedProtocolCollection = new ObservableCollection<Expression>();
            InitialAssumtionsCollection.Add(new Expression(new Step(1), "A",new Belives(), new Key("kAS","A","S")));
            InitialAssumtionsCollection.Add(new Expression(new Step(2), "B", new Belives(), new Key("kBS", "B", "S")));
            InitialAssumtionsCollection.Add(new Expression(new Step(3), "A", new Belives(), new Expression("S", new Controls(), new Key("kAB", "A", "B"))));
            InitialAssumtionsCollection.Add(new Expression(new Step(4), "B", new Belives(), new Expression("S", new Controls(), new Key("kAB", "A", "B"))));
            InitialAssumtionsCollection.Add(new Expression(new Step(5), "A", new Belives(), new Expression("S", new Controls(), new Fresh(new Key("kAB", "A", "B")))));
            InitialAssumtionsCollection.Add(new Expression(new Step(6), "B", new Belives(), new Expression("S", new Controls(), new Fresh(new Key("kAB", "A", "B")))));
            InitialAssumtionsCollection.Add(new Expression(new Step(7), "A", new Belives(), new Fresh(new Nonce("na"))));
            InitialAssumtionsCollection.Add(new Expression(new Step(8), "B", new Belives(), new Fresh(new Nonce("nb"))));
            WorkingList=new HashSet<Expression>(Ec);
            forPrint = new HashSet<HashSet<string>>(HsComparer);
            InitializeComponent();
            InitialAssumtions.DataContext = InitialAssumtionsCollection;
            AnnotatedProtocol.DataContext = AnnotatedProtocolCollection;
        }

        public Action ParseAction(string str)
        {
            switch (str.ToLower())
            {
                case "belives":
                    return new Belives();
                case "said":
                    return new Said();
                case "received":
                    return new Received();
                case "controls":
                    return new Controls();
            }
            return null;
        }

        private void AddAssumtion_OnClick(object sender, RoutedEventArgs e)
        {
            var expression = new Expression();
            string text = NewAssumtion.Text;
            if (text == "") MessageBox.Show("Please insert an assumtion!");
            else
            {
                var split = text.Split(' ');
                // if text is composed from less than 3 tokens displays an error message and exit function
                if (split.Length < 3)
                {
                    MessageBox.Show("Please provide an valid input!");
                    return;
                }
                expression.Entity = split[0].ToUpper();
                expression.Action = ParseAction(split[1]);
                if (expression.Action == null)
                {
                    MessageBox.Show("Please insert a valid action");
                }
                else
                {
                    object x;
                    
                    //getting the start position of object
                    int objectStartPosition = split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1;
                    
                    //checking if object exists.
                    //if not exit with user warning.
                    if (objectStartPosition > text.Length)
                    {
                        MessageBox.Show("Please insert a valid input!");
                        return;
                    }
                    String objectText = text.Substring(objectStartPosition);
                    switch (split[2].ToLower())
                    {
                        case "em":
                            x = ParseEncryptedMessage(objectText);
                            break;
                        case "exp":
                            x = ParseExpression(objectText);
                            break;
                        case "fr":
                            x = ParseFresh(objectText);
                            break;
                        case "key":
                            x = ParseKey(objectText);
                            break;
                        case "mess":
                            x = ParseMessage(objectText);
                            break;
                        case "non":
                            x = ParseNonce(objectText);
                            break;
                        case "pk":
                            x = ParsePublicKey(objectText);
                            break;
                        default:
                            x = null;
                            break;
                    }
                    if (x == null)
                    {
                        MessageBox.Show("Please provide an valid input!");
                    }
                    else
                    {
                        expression.X = x;
                        InitialAssumtionsCollection.Add(expression);
                    }

                }
            }
        }

        private Key ParseKey(string s)
        {
            var split = s.Split(' ');
            if (split.Length < 3 || split[0] == "" || split[1] == "" || split[2] == "") return null;
            return new Key(split[0], split[1].ToUpper(), split[2].ToUpper());
        }

        public PublicKey ParsePublicKey(string s)
        {
            var split = s.Split(' ');
            if (split.Length < 2 || split[0] == "" || split[1] == "") return null;
            return new PublicKey(split[0].ToUpper(), split[1]);
        }

        public Nonce ParseNonce(string s)
        {
            if (s == "") return null;
            return new Nonce(s);
        }
        
        public Message ParseMessage(string s)
        {
            //TODO: add verifications
            if (s == "") return null;
            string[] strings = s.Split(',');
            var mes = new Message();
            foreach (var s1 in strings)
            {
                object x;
                var split2 = s1.Split(' ');

                if (split2[0].Length + 1 > s1.Length) return null;

                string messageText = s1.Substring(split2[0].Length + 1);

                switch (split2[0].ToLower())
                {
                    case "exp":
                        x = ParseExpression(messageText);
                        break;
                    case "fr":
                        x = ParseFresh(messageText);
                        break;
                    case "key":
                        x = ParseKey(messageText);
                        break;
                    case "non":
                        x = ParseNonce(messageText);
                        break;
                    case "pk":
                        x = ParsePublicKey(messageText);
                        break;
                    default:
                        x = null;
                        break;
                }
                if (x == null) return null;
                mes.MessageList.Add(x);
            }
            return mes;
        }
        public Fresh ParseFresh(string s)
        {
            if (s == "") return null;
            object x;
            var split = s.Split(' ');
            if (split.Length == 0 || (split[0].Length + 1 > s.Length)) return null;
            String freshText = s.Substring(split[0].Length + 1);
            switch (split[0].ToLower())
            {
                case "em":
                    x = ParseEncryptedMessage(freshText);
                    break;
                case "key":
                    x = ParseKey(freshText);
                    break;
                case "non":
                    x = ParseNonce(freshText);
                    break;
                case "pk":
                    x = ParsePublicKey(freshText);
                    break;
                case "mess":
                    x = ParseMessage(freshText);
                    break;
                default:
                    x = null;
                    break;
            }
            if (x == null) return null;
            return new Fresh(x);
        }

        public Expression ParseExpression(string s)
        {
            if (s == "") return null;
            Expression e = new Expression();
            string[] split = s.Split(' ');
            int expressionLength = split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1;
            if (split.Length < 3 || expressionLength > s.Length) return null;
            e.Entity = split[0].ToUpper();
            e.Action = ParseAction(split[1]);
            object x;
            String expressionContent = s.Substring(expressionLength);
            switch (split[2].ToLower())
            {
                case "em":
                    x = ParseEncryptedMessage(expressionContent);
                    break;
                case "fr":
                    x = ParseFresh(expressionContent);
                    break;
                case "key":
                    x = ParseKey(expressionContent);
                    break;
                case "mess":
                    x = ParseMessage(expressionContent);
                    break;
                case "non":
                    x = ParseNonce(expressionContent);
                    break;
                case "pk":
                    x = ParsePublicKey(expressionContent);
                    break;
                default:
                    x = null;
                    break;
            }
            if (x == null)
            {
                return null;
            }
            e.X = x;
            return e;
        }

        public EncryptedMessage ParseEncryptedMessage(string s)
        {
            if (s == "") return null;
            string[] split = s.Split(' ');
            var em = new EncryptedMessage(split[split.Length - 1]);
            s = s.Substring(0, s.Length - 1 - em.Key.Length);
            string[] strings = s.Split(',');
            foreach (var s1 in strings)
            {
                object x;
                var split2 = s1.Split(' ');
                if (split2.Length == 1)
                    return null;
                switch (split2[0].ToLower())
                {
                    case "exp":
                        x = ParseExpression(s1.Substring(split2[0].Length + 1));
                        break;
                    case "fr":
                        x = ParseFresh(s1.Substring(split2[0].Length + 1));
                        break;
                    case "key":
                        x = ParseKey(s1.Substring(split2[0].Length + 1));
                        break;
                    case "mess":
                        x = ParseMessage(s1.Substring(split2[0].Length + 1));
                        break;
                    case "non":
                        x = ParseNonce(s1.Substring(split2[0].Length + 1));
                        break;
                    case "pk":
                        x = ParsePublicKey(s1.Substring(split2[0].Length + 1));
                        break;
                    case "em":
                        x = ParseEncryptedMessage(s1.Substring(split2[0].Length + 1));
                        break;
                    default:
                        x = null;
                        break;
                }
                if (x == null) return null;
                em.MessageList.Add(x);
            }
            return em;
        }

        private void DeleteAssumtion_OnClick(object sender, RoutedEventArgs e)
        {
            int selectedIndex = InitialAssumtions.SelectedIndex;
            if (selectedIndex >= 0)
            {
                InitialAssumtionsCollection.RemoveAt(selectedIndex);
            }
        }

        private void AddProtocolStep_OnClick(object sender, RoutedEventArgs e)
        {
            var expression = new Expression();
            string text = NewProtocolStep.Text;
            if (text == "") MessageBox.Show("Completati toate campurile!!");
            else
            {
                var split = text.Split(' ');
                int objectStartPosition = split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1;
                if (objectStartPosition > text.Length)
                {
                    MessageBox.Show("Please provide a valid input!");
                    return;
                }

                String objectText = text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1);
                expression.Entity = split[0].ToUpper();
                expression.Action = ParseAction(split[1]);
                if (expression.Action == null)
                {
                    MessageBox.Show("Input invalid");
                }
                else
                {
                    object x;
                    switch (split[2].ToLower())
                    {
                        case "em":
                            x = ParseEncryptedMessage(objectText);
                            break;
                        case "exp":
                            x = ParseExpression(objectText);
                            break;
                        case "fr":
                            x = ParseFresh(objectText);
                            break;
                        case "key":
                            x = ParseKey(objectText);
                            break;
                        case "mess":
                            x = ParseMessage(objectText);
                            break;
                        case "non":
                            x = ParseNonce(objectText);
                            break;
                        case "pk":
                            x = ParsePublicKey(objectText);
                            break;
                        default:
                            x = null;
                            break;
                    }
                    if (x == null)
                    {
                        MessageBox.Show("Input invalid");
                    }
                    else
                    {
                        expression.X = x;
                        AnnotatedProtocolCollection.Add(expression);
                    }

                }
            }
        }

        private void DeleteProtocolStep_OnClick(object sender, RoutedEventArgs e)
        {
            int selectedIndex = AnnotatedProtocol.SelectedIndex;
            if (selectedIndex >= 0)
            {
                AnnotatedProtocolCollection.RemoveAt(selectedIndex);
            }
        }


        private void Work_OnClick(object sender, RoutedEventArgs e)
        {
            WorkingList.Clear();
            forPrint.Clear();
            Output.Text = "";
            if (InitialAssumtionsCollection.Count == 0 || AnnotatedProtocolCollection.Count == 0)
            {
                MessageBox.Show("Insert annotated protocol and initial belives");
            }
            else
            {
                foreach (var expression in InitialAssumtionsCollection)
                {
                    WorkingList.Add(expression);
                }
                foreach (var expression in AnnotatedProtocolCollection)
                {
                    WorkingList.Add(expression);
                    while (true)
                    {
                        bool check = false;
                        var auxList = WorkingList.ToList();
                        for (int i = 0; i < auxList.Count; i++)
                        {
                            var rls = new HashSet<string>();
                            var result = BanRules.ApplyRule(auxList[i], null);
                            if (result.Count != 0)
                            {
                                
                                int count = WorkingList.Count;
                                foreach (var exp in result)
                                {
                                    int k = WorkingList.Count;
                                    WorkingList.Add(exp);

                                    if (k < WorkingList.Count) { rls.Add("Auto:" + auxList[i]); rls.Add(exp.ToString()); }
                                }
                                if (count != WorkingList.Count)
                                {
                                    check = true;
                                }
                                
                            }

                            forPrint.Add(rls);
                        }
                        for (int i = 0; i < auxList.Count-1; i++)
                        {
                            for (int j = i+1; j < auxList.Count; j++)
                            {
                                var rls = new HashSet<string>();
                                var result = BanRules.ApplyRule(auxList[i], auxList[j]);
                                if (result.Count != 0)
                                {
                                    
                                    var count = WorkingList.Count;
                                    
                                    foreach (var exp in result)
                                    {
                                        int k = WorkingList.Count;
                                        WorkingList.Add(exp);
                                        if (k < WorkingList.Count) { rls.Add(auxList[i] + "  +  " + auxList[j]); rls.Add(exp.ToString()); }
                                    }
                                    if (count != WorkingList.Count)
                                    {
                                        check = true;
                                    }
                                }
                                forPrint.Add(rls);
                            }
                        }

                        if (!check)
                        {
                            break;
                        }
                    }
                    Output.Text = "";
                    foreach (var hashSet in forPrint)
                    {
                        foreach (var exprString in hashSet)
                        {
                            Output.Text += exprString + "\n";
                        }
                        Output.Text += "-----------------------------\n";
                    }
                    Output.Text += "=============================";

                }
            }
        }
    }

    public class HSComparer : IEqualityComparer<HashSet<string>>
    {
        public bool Equals(HashSet<string> x, HashSet<string> y)
        {
            if (x.Count!=y.Count)
            {
                return false;
            }
            string[] xArr = x.ToArray();
            string[] yArr = y.ToArray();
            int k = 0;
            for (int i = 0; i < xArr.Length; i++)
            {
                if (String.Compare(xArr[i], yArr[i], StringComparison.Ordinal)!=0) return false;
                k++;
            }
            if (k==xArr.Length)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(HashSet<string> obj)
        {
            return -1;
        }
    }
}
