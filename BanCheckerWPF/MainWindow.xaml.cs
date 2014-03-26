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
        public HashSet<Expression> WorkingList;
        public MainWindow()
        {
            InitialAssumtionsCollection = new ObservableCollection<Expression>();
            AnnotatedProtocolCollection = new ObservableCollection<Expression>();
            InitialAssumtionsCollection.Add(new Expression("A",new Belives(), new Key("kAS","A","S")));
            InitialAssumtionsCollection.Add(new Expression("B", new Belives(), new Key("kBS", "B", "S")));
            InitialAssumtionsCollection.Add(new Expression("A", new Belives(), new Expression("S",new Controls(), new Key("kAB","A","B"))));
            InitialAssumtionsCollection.Add(new Expression("B", new Belives(), new Expression("S", new Controls(), new Key("kAB", "A", "B"))));
            InitialAssumtionsCollection.Add(new Expression("A", new Belives(), new Expression("S", new Controls(), new Fresh(new Key("kAB", "A", "B")))));
            InitialAssumtionsCollection.Add(new Expression("B", new Belives(), new Expression("S", new Controls(), new Fresh(new Key("kAB", "A", "B")))));
            InitialAssumtionsCollection.Add(new Expression("A", new Belives(), new Fresh(new Nonce("na"))));
            InitialAssumtionsCollection.Add(new Expression("B", new Belives(), new Fresh(new Nonce("nb"))));
            WorkingList=new HashSet<Expression>(Ec);
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
            if (text == "") MessageBox.Show("introduceti text!!");
            else
            {
                var split = text.Split(' ');
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
                            x =
                                ParseEncryptedMessage(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "exp":
                            x =
                                ParseExpression(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "fr":
                            x =
                                ParseFresh(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "key":
                            x = ParseKey(text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "mess":
                            x =
                                ParseMessage(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "non":
                            x =
                                ParseNonce(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "pk":
                            x =
                                ParsePublicKey(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
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
                        InitialAssumtionsCollection.Add(expression);
                    }

                }
            }
        }

        private Key ParseKey(string s)
        {
            var split = s.Split(' ');
            if (split[0] == "" || split[1] == "" || split[2] == "") return null;
            return new Key(split[0], split[1].ToUpper(), split[2].ToUpper());
        }

        public PublicKey ParsePublicKey(string s)
        {
            var split = s.Split(' ');
            if (split[0] == "" || split[1] == "") return null;
            return new PublicKey(split[0].ToUpper(), split[1]);
        }

        public Nonce ParseNonce(string s)
        {
            if (s == "") return null;
            return new Nonce(s);
        }

        public Message ParseMessage(string s)
        {
            if (s == "") return null;
            string[] strings = s.Split(',');
            var mes = new Message();
            foreach (var s1 in strings)
            {
                object x;
                var split2 = s1.Split(' ');
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
                    case "non":
                        x = ParseNonce(s1.Substring(split2[0].Length + 1));
                        break;
                    case "pk":
                        x = ParsePublicKey(s1.Substring(split2[0].Length + 1));
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
            switch (split[0].ToLower())
            {
                case "em":
                    x = ParseEncryptedMessage(s.Substring(split[0].Length + 1));
                    break;
                case "key":
                    x = ParseKey(s.Substring(split[0].Length + 1));
                    break;
                case "non":
                    x = ParseNonce(s.Substring(split[0].Length + 1));
                    break;
                case "pk":
                    x = ParsePublicKey(s.Substring(split[0].Length + 1));
                    break;
                case "mess":
                    x = ParseMessage(s.Substring(split[0].Length + 1));
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
            e.Entity = split[0].ToUpper();
            e.Action = ParseAction(split[1]);
            object x;
            switch (split[2].ToLower())
            {
                case "em":
                    x = ParseEncryptedMessage(s.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                    break;
                case "fr":
                    x = ParseFresh(s.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                    break;
                case "key":
                    x = ParseKey(s.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                    break;
                case "mess":
                    x = ParseMessage(s.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                    break;
                case "non":
                    x = ParseNonce(s.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                    break;
                case "pk":
                    x = ParsePublicKey(s.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                    break;
                default:
                    x = null;
                    break;
            }
            if (x == null)
            {
                return null;
            }
            else
            {
                e.X = x;
                return e;
            }
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
                            x =
                                ParseEncryptedMessage(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "exp":
                            x =
                                ParseExpression(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "fr":
                            x =
                                ParseFresh(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "key":
                            x = ParseKey(text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "mess":
                            x =
                                ParseMessage(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "non":
                            x =
                                ParseNonce(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
                            break;
                        case "pk":
                            x =
                                ParsePublicKey(
                                    text.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1));
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
                //MessageBox.Show(WorkingList.Count.ToString());
                foreach (var expression in AnnotatedProtocolCollection)
                {
                    WorkingList.Add(expression);
                    while (true)
                    {
                        bool check = false;
                        var auxList = WorkingList.ToList();
                        for (int i = 0; i < auxList.Count; i++)
                        {
                            var result = BanRules.ApplyRule(auxList[i], null);
                            
                            if (result.Count != 0)
                            {
                                int count = WorkingList.Count;
                                foreach (var exp in result)
                                {
                                    WorkingList.Add(exp);
                                }
                                if (count != WorkingList.Count)
                                {
                                    check = true;       
                                }
                            }
                        }
                        for (int i = 0; i < auxList.Count-1; i++)
                        {
                            for (int j = i+1; j < auxList.Count; j++)
                            {
                                var result = BanRules.ApplyRule(auxList[i], auxList[j]);
                                if (result.Count != 0)
                                {
                                    var count = WorkingList.Count;
                                    foreach (var exp in result)
                                    {
                                        WorkingList.Add(exp);
                                    }
                                    if (count != WorkingList.Count)
                                    {
                                        check = true;
                                    }
                                }   
                            }
                        }

                        if (!check)
                        {
                            break;
                        }
                    }
                    Output.Text = "";
                    foreach (var expression1 in WorkingList)
                    {
                        Output.Text += expression1 + "\n";
                    }

                }

            }
        }
    }
}
