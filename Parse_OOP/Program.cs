using System;
using System.Linq;
using System.Collections.Generic;
using ParseResult = System.Tuple<Parse_OOP.Expression, int>;


// изменена система навигации 
// добавлена возможность просмотра списка констант

namespace Parse_OOP
{
    // калькулятор 
    class Program
    {
        public static Dictionary<string, double> constants = new Dictionary<string, double>();
        static void Main(string[] args)
        {
            constants.Add("Pi", 3.141592653589793);
            constants.Add("Tau", constants["Pi"]*2);
            constants.Add("e", 2.718281828459045);
            constants.Add("с", 299792458);
            MainMenu();
        }
        static void test_expr(string expr)
        {
            ExpressionCalculator calculator = new ExpressionCalculator(expr);
            double evaluated = calculator.Calculate();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Ответ] {evaluated}");
        }


        static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Выберите действие");
            Console.WriteLine("1 - Посчитать выражение");
            Console.WriteLine("2 - Меню констант");
            ConsoleKey ki = Console.ReadKey(true).Key;
            if (ki == ConsoleKey.D1 || ki == ConsoleKey.NumPad1)
            {
                CalculatorMenu();
            }
            else if (ki == ConsoleKey.D2 || ki == ConsoleKey.NumPad2)
            {
                DictionaryMenu();
            }
        }
        static void CalculatorMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Введите пример на который желаете получить ответ");
            test_expr(Console.ReadLine());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nEnter - повторить \nEsc - назад");
            ConsoleKey ki = Console.ReadKey(true).Key;
            if (ki == ConsoleKey.Enter)
            {
                CalculatorMenu();
            }
            if (ki == ConsoleKey.Escape)
            {
                MainMenu();
            }
        }
        static void DictionaryMenu()
        {
            Console.Clear();
            Console.WriteLine("1 - Вывести список переменных");
            Console.WriteLine("2 - Создать новую переменную (изменить старую)");

            Console.WriteLine("\nEsc - назад");
            ConsoleKey ki = Console.ReadKey(true).Key;
            if (ki == ConsoleKey.D1 || ki == ConsoleKey.NumPad1)
            {
                ShowDictionary();
            }
            else if (ki == ConsoleKey.D2 || ki == ConsoleKey.NumPad2)
            {
                ChangeDictionary();
            }
            if (ki == ConsoleKey.Enter)
            {
                DictionaryMenu();
            }
            else if (ki == ConsoleKey.Escape)
            {
                MainMenu();
            }
        }

        static void ChangeDictionary()
        {
            Console.Clear();
            Console.WriteLine("Введите обозначение новой переменной");
            string key = Console.ReadLine();
            Console.WriteLine("Введите значение новой переменной");
            ExpressionCalculator calculator = new ExpressionCalculator(Console.ReadLine());
            double value = calculator.Calculate();

            if (constants.ContainsKey(key))
            {
                Console.WriteLine("Вы хотите присвоить новое значение данной переменной(Y/N)?");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    double oldValue = constants[key];
                    constants.Remove(key);
                    constants.Add(key, value);
                    Console.WriteLine($"Значение переменной <{key}> было изменено с <{oldValue}> на <{value}>");
                }
                else if (Console.ReadKey(true).Key == ConsoleKey.N)
                {
                    ChangeDictionary();
                }
            }
            else
            {
                constants.Add(key, value);
                Console.WriteLine("Была создана новая константа\n");
            }

            Console.WriteLine("\nEnter - повторить \nEsc - назад");
            ConsoleKey ki = Console.ReadKey(true).Key;
            if (ki == ConsoleKey.Enter)
            {
                ChangeDictionary();
            }
            else if (ki == ConsoleKey.Escape)
            {
                DictionaryMenu();
            }
        }
        static void ShowDictionary()
        {
            Console.Clear();
            foreach (KeyValuePair<string, double> t in constants)
            {
                Console.WriteLine($"{t.Key} = {t.Value}");
            }
            Console.WriteLine("\nEsc - назад");
            ConsoleKey ki = Console.ReadKey(true).Key;
            if (ki == ConsoleKey.Enter)
            {
                ShowDictionary();
            }
            else if (ki == ConsoleKey.Escape)
            {
                DictionaryMenu();
            }

        }

        //static void test_expr(string expr, double expected)
        //{
        //    ExpressionCalculator calculator = new ExpressionCalculator(expr);
        //    double evaluated = calculator.Calculate();
        //    if (evaluated != expected)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine($"[FAILED] {expr} != {expected}");
        //    }
        //    else
        //    {
        //        Console.ForegroundColor = ConsoleColor.Green;
        //        Console.WriteLine($"[PASSED] {expr} = {expected}");
        //    }
        //}
    }
    
    class Token
    {

    }

    class NumberToken : Token 
    {
        public readonly double value;

        public NumberToken(double value)
        {
            this.value = value;
        }
    }
    class OperatorToken : Token
    {
        public enum OperatorType
        {
            PLUS,
            MINUS,
            DIVIDE,
            MULTIPLY
        };

        public readonly OperatorType type;

        public OperatorToken(OperatorType type)
        {
            this.type = type;
        }
    }
    class ParenthesisToken : Token
    {
        public enum ParenthesisType
        {
            LEFT,
            RIGHT
        }

        public readonly ParenthesisType type;

        public ParenthesisToken(ParenthesisType type)
        {
            this.type = type;
        }

    }

    abstract class Expression 
    {
        public abstract double Calculate();
    }

    abstract class BinOperation: Expression
    {
        protected Expression left;
        protected Expression right;

        public BinOperation(Expression left, Expression right) 
        {
            this.left = left;
            this.right = right;
        }

        public override double Calculate()
        {
            double leftValue = left.Calculate();
            double rightValue = right.Calculate();
            return OperationCalculate(leftValue, rightValue);
        }

        protected abstract double OperationCalculate(double leftValue, double rightValue);

    }

    class Plus:BinOperation
    {
        public Plus(Expression left, Expression right):base(left, right) { }
        protected override double OperationCalculate(double leftValue, double rightValue)
        {
            return (leftValue + rightValue);
        }
    }
    class Minus : BinOperation
    {
        public Minus(Expression left, Expression right) : base(left, right) { }
        protected override double OperationCalculate(double leftValue, double rightValue)
        {
            return (leftValue - rightValue);
        }
    }
    class Multiply : BinOperation
    {
        public Multiply(Expression left, Expression right) : base(left, right) { }
        protected override double OperationCalculate(double leftValue, double rightValue)
        {
            return (leftValue * rightValue);
        }
    }
    class Divide : BinOperation
    {
        public Divide(Expression left, Expression right) : base(left, right) { }
        protected override double OperationCalculate(double leftValue, double rightValue)
        {
            return (leftValue / rightValue);
        }
    }
    class Number : Expression
    {
        private readonly double value;

        public Number(double value)
        {
            this.value = value;
        }

        public override double Calculate() 
        {
            return value;
        }


    }

    class ExpressionCalculator
    {
        private List<Token> tokens;
        private Expression expression;
        public ExpressionCalculator(string expr) 
        {
            Tokenize(expr);
            var (operand, end_index2) = PlusMinusRule(0);
            expression = operand;
        }

        public double Calculate()
        {
            return expression.Calculate();
        }

        void Tokenize(string expr)
        {
            tokens = new List<Token>();
            for (int i = 0; i < expr.Count();)
            {
                if ('0' <= expr[i] && expr[i] <= '9')
                {
                    int j = i;
                    while (i < expr.Count() && (('0' <= expr[i] && expr[i] <= '9') || expr[i] == ','))
                    {
                        i += 1;
                    }
                    string number_str = expr.Substring(j, i - j);
                    tokens.Add(new NumberToken(Convert.ToDouble(number_str)));
                }

                else if (('A' <= expr[i] && expr[i] <= 'Z') || ('a' <= expr[i] && expr[i] <= 'z'))
                {
                    int j = i;
                    while (i < expr.Count() && (('A' <= expr[i] && expr[i] <= 'Z') || ('a' <= expr[i] && expr[i] <= 'z')))
                    {
                        i += 1;
                    }
                    string const_str = expr.Substring(j, i - j);
                    tokens.Add(new NumberToken(Program.constants[const_str]));
                }


                else if (expr[i] == '+')
                {
                    tokens.Add(new OperatorToken(OperatorToken.OperatorType.PLUS));
                    i++;
                }
                else if (expr[i] == '-')
                {
                    tokens.Add(new OperatorToken(OperatorToken.OperatorType.MINUS));
                    i++;
                }
                else if ( expr[i] == '*')
                {
                    tokens.Add(new OperatorToken(OperatorToken.OperatorType.MULTIPLY));
                    i++;
                }
                else if (expr[i] == '/')
                {
                    tokens.Add(new OperatorToken(OperatorToken.OperatorType.DIVIDE));
                    i++;
                }
                else if (expr[i] == '(')
                {
                    tokens.Add(new ParenthesisToken(ParenthesisToken.ParenthesisType.LEFT));
                    i++;
                }
                else if (expr[i] == ')')
                {
                    tokens.Add(new ParenthesisToken(ParenthesisToken.ParenthesisType.RIGHT));
                    i++;
                }
                else if (expr[i] == ' ' || expr[i] == '\t')
                {
                    i++;
                }
                else
                {
                    Console.WriteLine($"unexpected token {expr[i]}");
                }
            }
        }

        ParseResult NumberRule( int start_index)
        {
            if (start_index < tokens.Count() && tokens[start_index] is NumberToken)
            {
                NumberToken token = (NumberToken)tokens[start_index];
                return new ParseResult(new Number(token.value), start_index + 1);
            }
            return new ParseResult(new Number(0), tokens.Count());
        }
        ParseResult ParenthesisRule(int start_index)
        {
            if (start_index < tokens.Count())
            {
                if (tokens[start_index] is ParenthesisToken && ((ParenthesisToken)tokens[start_index]).type == ParenthesisToken.ParenthesisType.LEFT)
                {
                    var (value, end_index) = PlusMinusRule(start_index + 1);
                    if (end_index < tokens.Count())
                    {
                        if (tokens[end_index] is ParenthesisToken && ((ParenthesisToken)tokens[end_index]).type == ParenthesisToken.ParenthesisType.RIGHT)
                        {
                            return new ParseResult(value, end_index + 1);
                        }
                    }
                }
            }
            return NumberRule(start_index);
        }
        ParseResult MultiplyDivideRule(int start_index)
        {
            var (first_operand, end_index1) = ParenthesisRule(start_index);

            while (end_index1 < tokens.Count())
            {
                if (tokens[end_index1] is OperatorToken)
                {
                    OperatorToken token = (OperatorToken)tokens[end_index1];
                    if (token.type == OperatorToken.OperatorType.MULTIPLY)
                    {
                        var (second_operand, end_index2) = ParenthesisRule(end_index1 + 1);
                        first_operand = new Multiply(first_operand, second_operand);
                        end_index1 = end_index2;
                        continue;
                    }
                    else if (token.type == OperatorToken.OperatorType.DIVIDE)
                    {
                        var (second_operand, end_index2) = ParenthesisRule(end_index1 + 1);
                        first_operand = new Divide(first_operand, second_operand);
                        end_index1 = end_index2;
                        continue;
                    }
                }
                break;
            }


            return new ParseResult(first_operand, end_index1);
        }
        ParseResult PlusMinusRule(int start_index)
        {
            var (first_operand, end_index1) = MultiplyDivideRule(start_index);

            while (end_index1 < tokens.Count())
            {
                if (tokens[end_index1] is OperatorToken)
                {
                    OperatorToken token = (OperatorToken)tokens[end_index1];
                    if (token.type == OperatorToken.OperatorType.PLUS)
                    {
                        var (second_operand, end_index2) = MultiplyDivideRule(end_index1 + 1);
                        first_operand = new Plus(first_operand, second_operand);
                        end_index1 = end_index2;
                        continue;
                    }
                    else if (token.type == OperatorToken.OperatorType.MINUS)
                    {
                        var (second_operand, end_index2) = MultiplyDivideRule(end_index1 + 1);
                        first_operand = new Minus(first_operand, second_operand);
                        end_index1 = end_index2;
                        continue;
                    }
                }
                break;
            }

            return new ParseResult(first_operand, end_index1);
        }
    }
}
