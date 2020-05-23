using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;

namespace expressionWpfTest1
{
    class Expression
    {

        public class StringToValue
        {
            private string str;
            private double value;
            private VariableCollection VC;
            public StringToValue(string s)
            {
                str =tool.predeal(s);
                VC = new VariableCollection(s);
                divStringExp dse = new divStringExp(s, VC);
            }
            public double getValue()
            {
                divStringExp dse = new divStringExp(str, VC);
                value = dse.deal();
                return value;
            }
            public VariableCollection GetVariables()
            {
                return VC;
            }
            public void setVariables(VariableCollection V)
            {
                VC = V;
            }
        }
        public class VariableCollection
        {
            public Variable[] vlist { get; }
            public List<string> Name { get; }
            public List<double> Value { get;  }
            public List<int> Index { get; }
            public List<int> Length { get; }
            public VariableCollection(string s)
            {
                Name = new List<string>();
                Value = new List<double>();
                Index = new List<int>();
                Length = new List<int>();
                string pattern = @"[a-zA-Z_][a-zA-Z_0-9]*";
                string input = s;
                MatchCollection m = Regex.Matches(input, pattern);
                if (m.Count != 0)
                {
                    for (int i = 0; i < m.Count; i++)
                    {
                        int flag = 0;
                        for (int j = 0; j < Name.Count; j++)
                        {
                            if (Name[j] == m[i].Value)
                            {
                                flag = 1;
                            }
                        }
                        for (int j = 0; j < tool.Function.Length; j++)
                        {
                            if (m[i].Value == tool.Function[j])
                            {
                                flag = 1;
                            }
                        }
                        for (int j = 0; j < tool.BinaryFunction.Length; j++)
                        {
                            if (m[i].Value == tool.BinaryFunction[j])
                            {
                                flag = 1;
                            }
                        }
                        if (flag == 0)
                        {
                            Name.Add(m[i].Value);
                            Value.Add(1);
                            Length.Add(m[i].Length);
                        }
                    }

                    vlist = new Variable[Name.Count];   //生成参数数组

                    for (int i = 0; i < Name.Count; i++)//找到相同的参数的位置
                    {
                        List<int> temp = new List<int>();
                        for (int j = 0; j < m.Count; j++)
                        {
                            if (Name[i] == m[j].Value)
                            {
                                temp.Add(m[j].Index);
                            }
                        }
                        vlist[i] = new Variable(Name[i], Value[i], temp, Length[i]);
                    }
                }
            }
            public void display()
            {
                Console.WriteLine("变量组的参数" + Name.Count + " " + Value.Count + " " + Index.Count + " " + Length.Count + " ");
                for (int i = 0; i < Name.Count; i++)
                {
                    Console.WriteLine("{0} {1} {2}", Name[i], Value[i], Length[i]);
                    for (int j = 0; j < vlist[i].index.Count; j++)
                    {
                        Console.Write(vlist[i].index[j] + " ");
                    }
                    Console.WriteLine("参数end");
                }
            }

        }
        public class Variable
        {
            public string name { get; }
            public double value { get; set; }
            public List<int> index { get; }
            public int count { get; }
            public Variable(string n, double v, List<int> i, int c)
            {
                name = n;
                value = v;
                index = i;
                count = c;
            }

        }
        

        public class divStringExp
        {
            private string str;
            private List<string> expressions;
            private VariableCollection VC;
            public divStringExp(string s, VariableCollection V)
            {
                VC = V;
                str = s;
                expressions = new List<string>();
                divExp();
                show();
            }
            public void divExp()
            {
                int left = str.IndexOf('(');
                Stack<char> s = new Stack<char>();
                List<int> lefts = new List<int>();
                List<int> rights = new List<int>();
                bool flag = false;
                //if (left >= 0)//是否有符号
                //{
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '(')
                    {
                        if (s.Count == 0)
                        {
                            lefts.Add(i);
                        }
                        flag = true;
                        s.Push('(');
                    }
                    if (flag && str[i] == ')')
                    {
                        s.Pop();
                        if (s.Count == 0)
                        {
                            rights.Add(i);
                        }
                    }
                }
                if (rights.Count != lefts.Count)
                {
                    Console.WriteLine("input error");
                }
                for (int i = 0; i < lefts.Count; i++)
                {
                    for (int j = lefts[i]; j >= 0; j--)
                    {
                        if (j == 0 || str[j] == '-')
                        {
                            lefts[i] = j;
                            break;
                        }
                        if (str[j] == '+')
                        {
                            lefts[i] = j;
                            break;
                        }
                    }
                    //记录跳过多少括号
                    int count = 0;
                    for (int t = rights[i]; t < str.Length; t++) //(12/222)/222/344/323*(12+12)
                    {
                        if (str[t] == '+' || str[t] == '-')
                        {
                            rights[i] = t - 1;
                            break;
                        }
                        if (i + 1 + count < lefts.Count && t >= lefts[i + 1 + count])
                        {
                            t = rights[count + i + 1];
                            rights[i] = t;
                            lefts[count + i + 1] = -1;//标记该括号被跳过
                            count++;
                        }
                        if (t + 1 == str.Length)//字符串结束了
                        {
                            rights[i] = t;
                        }
                    }
                }
                for (int i = 0; i < lefts.Count; i++)
                {
                    Console.WriteLine("exp左右括号:" + lefts[i] + "  " + rights[i]);
                }
                string temp = "";
                //最后要给第一项加符号
                List<string> strTemp = new List<string>();
                for (int i = 0, start = 0; i < str.Length; i++)
                {
                    while (start < lefts.Count && lefts[start] == -1)
                    {
                        start++;
                    }
                    if (start < lefts.Count && i == lefts[start])
                    {

                        expressions.Add(str.Substring(i, rights[start] - lefts[start] + 1));
                        i = rights[start];
                        start++;
                        strTemp.Add(temp);
                        temp = "";
                    }
                    else
                    {
                        temp = temp + str[i];
                    }
                    if (i == str.Length - 1)
                    {
                        strTemp.Add(temp);
                    }
                }
                for (int i = 0; i < strTemp.Count; i++)
                {

                    for (int j = strTemp[i].Length - 1, start = j; j >= 0; j--)
                    {
                        if (strTemp[i][j] == '+' || strTemp[i][j] == '-' || j == 0)
                        {
                            expressions.Add(strTemp[i].Substring(j, start - j + 1));
                            start = j - 1;
                        }
                    }
                }
                

                for (int i = 0; i < expressions.Count; i++)//去除空元素
                {
                    if (expressions[i] == "")
                    {
                        expressions.RemoveAt(i);
                    }
                }
               
            }
            public double deal()
            {
                double result = 0;
                for (int i = 0; i < expressions.Count; i++)
                {
                    if (expressions[i][0] == '+')
                    {
                        divStringItem dsi = new divStringItem(expressions[i].Remove(0, 1), VC);
                        result = result + dsi.deal();
                    }
                    else if (expressions[i][0] == '-')
                    {
                        divStringItem dsi = new divStringItem(expressions[i].Remove(0, 1), VC);
                        result = result - dsi.deal();
                    }
                    else
                    {
                        divStringItem dsi = new divStringItem(expressions[i], VC);
                        result = result + dsi.deal();
                    }
                }
                return result;
            }

            public void show()
            {
                for (int i = 0; i < expressions.Count; i++)
                {
                    Console.WriteLine("divexp:" + expressions[i]);
                }
            }
        }
        public class divStringItem
        {
            private VariableCollection VC;
            private string str;
            //private string dividend;//被除数
            //private string divisor;//除数
            List<string> dividend;
            List<string> dividor;
            public divStringItem(string s, VariableCollection V)
            {
                dividend = new List<string>();
                dividor = new List<string>();
                VC = V;
                str = s;
                divFac();
                show();
            }
            public void divFac()
            {
                bool[] bracket = new bool[str.Length];
                int count = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '(')
                        count++;
                    else if (str[i] == ')')
                        count--;
                    if (count != 0)
                        bracket[i] = true;
                }
                if (count != 0)
                    throw new UserException("括号不匹配");
                for (int i = str.Length - 1, start = str.Length - 1; i >= 0; i--)//1/3
                {
                    if (str[i] == '/')
                    {
                        if (bracket[i] == false)
                        {
                            dividor.Add(str.Substring(i + 1, start - i));
                            start = i - 1;
                        }
                    }
                    else if (str[i] == '*')
                    {
                        if (bracket[i] == false)
                        {
                            dividend.Add(str.Substring(i + 1, start - i));
                            start = i - 1;
                        }
                    }
                    else if (i == 0)
                    {
                        dividend.Add(str.Substring(i, start - i + 1));
                    }

                }
            }
            public double deal()
            {
                double result = 1;
                for (int i = 0; i < dividend.Count; i++)
                {
                    divStringFac f1 = new divStringFac(dividend[i], VC);
                    result = f1.deal() * result;
                }
                for (int i = 0; i < dividor.Count; i++)
                {
                    divStringFac f1 = new divStringFac(dividor[i], VC);
                    result = result / f1.deal();
                }
                return result;
            }
            public void show()
            {
                //Console.WriteLine("end：" + dividend);
                //Console.WriteLine("sor：" + divisor);
            }
        }
        public class divStringFac
        {
            private VariableCollection VC;
            private string str;
            //private string[] parameter;//参数
                                       //private string[] expression;
                                       //private double coefficient; //系数
            private List<double> coefficient;  //系数
            public divStringFac(string s, VariableCollection V)
            {
                VC = V;
                coefficient = new List<double>();
                str = s;
            }
            public double deal() { 
            List<int> lefts;
            List<int> rights;
            lefts = new List<int>();
                rights = new List<int>();
                List<string> expressionTemp = new List<string>();
                if (str.IndexOf('*')>=0||str.IndexOf('^')>=0)
                {
                    int stackCount = 0;
            lefts = new List<int>();
                    rights = new List<int>();
                    for(int i = 0; i<str.Length; i++)
                    {
                        if (str[i] == '(')
                        {
                            if (stackCount == 0)
                            {
                                lefts.Add(i);
                            }
                            stackCount++;
                        }
                        if (str[i] == ')')
                        {
                            stackCount--;
                            if (stackCount == 0)
                            {
                                rights.Add(i);
                            }
                        }
                    }
                
                    List<int> cut = new List<int>();
                    cut.Add(-1);
                    for(int i = 0; i<str.Length; i++)
                    {
                        if (str[i] == '*')
                        {
                            bool flag = false;
                            for(int j = 0; j<lefts.Count; j++)
                            {
                                if (i > lefts[j] && i<rights[j])
                                {
                                    flag = true;
                                }
                            }
                            if (flag == false)
                            {
                                cut.Add(i);
                            }
                        }
                    }
                    if (cut.Count != 1)
                    {
                    
                        int left = 0;
                        int right = 0;
                        cut.Add(str.Length );
                        for (int i = 0; i<cut.Count; i++)
                            Console.WriteLine("1111111111111111111111   " + cut[i]);
                        for (int i = 1; i<cut.Count; i++)
                        {
                            left = cut[i - 1];
                            right = cut[i];
                            expressionTemp.Add(str.Substring(left+1, right - left-1));
                        }
                    }
                    else
                    {
                        expressionTemp.Add(str);
                    }
                }
                else
                {
                    expressionTemp.Add(str);
                }
                Console.WriteLine(expressionTemp.Count + "*分割数");
                for (int i = 0; i<expressionTemp.Count; i++)
                {
                    Console.WriteLine(expressionTemp[i]+"    ");
                
                }
                Console.WriteLine("*end");
                for(int i = 0; i<expressionTemp.Count; i++)     
                {
                    bool flag = true;

                    if (flag)
                    {
                        for (int j = 0; j<tool.Function.Length; j++)//函数
                        {
                            if (tool.Function[j].Length - 1<expressionTemp[i].Length&&expressionTemp[i].Substring(0, tool.Function[j].Length) == tool.Function[j])
                            {
                                Console.WriteLine("exp");
                                flag = false;
                                //expression[i].sub(tool.Function[j].Length+1,expression.Length-2)表达式求值，求值后函数
                                divStringExp dse1 = new divStringExp(expressionTemp[i].Substring(tool.Function[j].Length + 1, expressionTemp[i].Length - tool.Function[j].Length - 2), VC);
                                double va = dse1.deal();
                                switch (j)
                                {
                                    case 0:coefficient.Add(Math.Sin(va));break;
                                    case 1:coefficient.Add(Math.Cos(va));break;
                                    case 2:coefficient.Add(Math.Tan(va));break;
                                    case 3:coefficient.Add(Math.Abs(va));break;
                                    case 4:coefficient.Add(Math.Sqrt(va));break;
                                    case 5:coefficient.Add(Math.Exp(va)); break;
                                    case 6:coefficient.Add(Math.Floor(va)); break;
                                    case 7: { Random r = new Random(); coefficient.Add(r.NextDouble() * Math.Floor(va)); break; }
                                }
                            }
                        }
                        for (int j = 0; j < tool.BinaryFunction.Length; j++)//函数
                        {
                            if (tool.BinaryFunction[j].Length - 1 < expressionTemp[i].Length && expressionTemp[i].Substring(0, tool.BinaryFunction[j].Length) == tool.BinaryFunction[j])
                            {
                                Console.WriteLine("exp2");
                                flag = false;
                                for (int k = tool.BinaryFunction[j].Length + 1; k < expressionTemp[i].Length - 2; k++)
                                {
                                    if (expressionTemp[i][k] == ',')
                                    {
                                        bool flag1 = false;
                                        for (int h = 0; h < lefts.Count; h++)
                                        {
                                            if (k > lefts[h] && k < rights[h])
                                            {
                                                flag1 = true;
                                            }
                                        }
                                        if (flag1 == false)//log(2,3) k=5 8 3
                                        {
                                            divStringExp dse1 = new divStringExp(expressionTemp[i].Substring(tool.BinaryFunction[j].Length + 1, k - tool.BinaryFunction[j].Length - 1), VC);
                                            divStringExp dse2 = new divStringExp(expressionTemp[i].Substring(k + 1, expressionTemp[i].Length - k - 2), VC);
                                            double n1 = dse1.deal();
                                            double n2 = dse2.deal();
                                            if (n1 < 0 || n1 == 1)
                                            {
                                                Console.WriteLine("log1");
                                                if (n1 < 0)
                                                    throw new UserException("底数不能小于0");
                                                else
                                                    throw new UserException("底数不能等于1");
                                            }
                                            else
                                                switch (j)
                                                {
                                                    case 0: coefficient.Add(Math.Log(n2) / Math.Log(n1)); Console.WriteLine(Math.Log(n1) / Math.Log(n2)); break;
                                                }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (expressionTemp[i].IndexOf('^') > 0&&flag)         //次方 ((2)^(2+3))^(a1+9)  (2^3)^(3^2)
                {
                        flag = false;
                        List<string> t = new List<string>();
                        int count = 0;
                    if (expressionTemp[i].IndexOf('(') >= 0)
                    {
                        for (int j = 0, start = 0; j<expressionTemp[i].Length; j++)
                        {
                            
                            if (j == expressionTemp[i].Length - 1||expressionTemp[i][j] == '^'  )
                            {
                                if (j == expressionTemp[i].Length - 1)
                                {
                                    t.Add(expressionTemp[i].Substring(start, j - start + 1));
                                }
                                else
                                {
                                    bool flag1 = false;
                                    for (int i1 = 0; i1<lefts.Count; i1++)
                                    {
                                        if (lefts[i1] < j && rights[i1] > j)
                                            flag1 = true;
                                    }
                                    if (!flag1)
                                    {
                                        t.Add(expressionTemp[i].Substring(start, j - start));
                                        start = j + 1;
                                    }
                                }
                            }
                        }
                        if (t.Count == 1)      //说明不能拆开但有阶乘 ，防止拆开项有括号
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        foreach(string s1 in expressionTemp[i].Split('^'))
                        {
                            t.Add(s1);
                        }
                    }
                        
                        if(flag==false)
                        {
                            for (int j = 0; j<t.Count; j++)
                                 Console.WriteLine(t[j]);
                            double tempResult = 0;
                            divStringExp dse1 = new divStringExp(t[0], VC);
                            tempResult = dse1.deal();
                            for (int j = 1; j<t.Count; j++)
                            {
                                divStringExp dse = new divStringExp(t[j], VC);
                                //t[j]表达式求值后阶乘
                                tempResult = Math.Pow(tempResult, dse.deal());
                            }
                            coefficient.Add(tempResult);
                            Console.WriteLine("^^");
                        }
                    }
                    if (expressionTemp[i][0] == '(' && expressionTemp[i][expressionTemp[i].Length - 1] == ')'&&flag)
                    {
                        flag = false;
                        expressionTemp[i] = expressionTemp[i].Substring(1, expressionTemp[i].Length - 2);
                        divStringExp dse1 = new divStringExp(expressionTemp[i], VC);
                        coefficient.Add(dse1.deal());
                    }
                    if (flag)
                    {
                            if (VC.vlist != null)
                            {
                                for (int j = 0; j<VC.vlist.Length; j++)
                                {

                                    if (VC.vlist[j].name == expressionTemp[i])
                                    {
                                        Console.WriteLine("var");
                                        flag = false;
                                        coefficient.Add(VC.vlist[j].value);
                                    }
                                }
                            }
                    }
                     if(flag)
                     {
                        DataTable table = new System.Data.DataTable();
                        Console.WriteLine("num:"+ expressionTemp[i]);
                        string s1 = table.Compute(expressionTemp[i], "").ToString();
                        coefficient.Add(Convert.ToDouble(s1));
                        Console.WriteLine("numconvert:"+ Convert.ToDouble(s1));
                     }
               
                }
                double result = 1;
                for(int i = 0; i<coefficient.Count; i++)
                {
                    result = result* coefficient[i];
                }
                return result;
            }


            //sin,cos,tan
            //找到外层括号，内层转为表达式，递归求值
            //找到参数
            //计算系数
            //参数与系数相乘

        }
        public static class tool
        {
            public static string[] Function = { "sin", "cos", "tan", "abs", "sqrt", "exp","floor" ,"random"};
            public static string[] BinaryFunction = { "log" };
            public static string  predeal(string s)
            {
                for(int i = 1; i < s.Length; i++)
                {
                    if (char.IsNumber(s[i - 1]) && char.IsLetter(s[i]))
                        s=s.Insert(i, "*");
                    if(s[i-1]==')'&&s[i]=='(')
                        s=s.Insert(i, "*");
                }
                return s;
            }
        }
        public class UserException : ApplicationException//由用户程序引发，用于派生自定义的异常类型
        {

            public UserException() { }
            public UserException(string message)
                : base(message) { }
            public UserException(string message, Exception inner)
                : base(message, inner) { }

        }
    }
}
