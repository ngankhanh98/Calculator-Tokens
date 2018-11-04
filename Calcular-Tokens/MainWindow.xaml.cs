using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calcular_Tokens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        class Fraction
        {
            public static string SEPERATOR = "/";
            public int Numerator { get; set; }
            public int Denominator { get; set; }

            public Fraction()
            {
                Numerator = 0;
                Denominator = 1;
            }

            public static Fraction Parse(string line)
            {
                var result = new Fraction();

                var tokens = line.Split(new string[] { SEPERATOR }, StringSplitOptions.None);

                int num = int.Parse(tokens[0]);
                int den = int.Parse(tokens[1]);

                result.Numerator = num;
                result.Denominator = den;

                return result;
            }

            public static Result TryParse(string line)
            {
                var result = new Result();
                var f = new Fraction();
                var tokens = line.Split(new string[] { SEPERATOR },
                    StringSplitOptions.RemoveEmptyEntries)
                    .Select(token => token.Trim())
                    .ToArray();

                try
                {
                    int num = int.Parse(tokens[0]);
                    int den = int.Parse(tokens[1]);
                    f.Numerator = num;
                    f.Denominator = den;
                    result.Data = f;

                    if (den == 0)
                    {
                        result.IsSuccessful = false;
                        result.Errors.Add(new Error() { Code = 2, Message = "Devided by Zerp" });
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccessful = false;
                    result.Errors.Add(new Error() { Code = 1, Message = ex.Message });
                }

                return result;
            }

            public override string ToString()
            {
                if(Numerator%Denominator==0)
                    return $"{ Numerator/Denominator}";


                int max = Denominator > Numerator ? Denominator : Numerator;
                int min = Denominator + Numerator - max;

                while (min != 0)
                {
                    int temp = min;
                    min = max % min;
                    max = temp;
                }

                return $"{Numerator / max}{SEPERATOR}{Denominator / max}";
            }
        }

        class Error
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }

        class Result
        {
            public bool IsSuccessful { get; set; }
            public List<Error> Errors { get; set; }
            public object Data { get; set; }

            public Result()
            {
                Data = new Fraction();
                IsSuccessful = true;
                Errors = new List<Error>();
            }

            public static Result operator +(Result a, Result b)
            {
                var result = new Result();
                var f = new Fraction();
                var fa = a.Data as Fraction;
                var fb = b.Data as Fraction;

                int den = fa.Denominator * fb.Denominator;
                int num = fa.Denominator * fb.Numerator + fa.Numerator * fb.Denominator;

                f.Denominator = den;
                f.Numerator = num;
                result.Data = f;

                try
                {
                    if (fa.Denominator == 0 || fb.Denominator == 0)
                    {
                        result.IsSuccessful = false;
                        result.Errors.Add(new Error { Code = 1, Message = "Divided by Zero" });
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccessful = false;
                    result.Errors.Add(new Error { Code = 2, Message = ex.Message });
                }

                return result;
            }
        }


        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            var longline = txtInOut.Text;
            var tokens = longline.Split(new string[] { "+" },
                    StringSplitOptions.RemoveEmptyEntries)
                    .Select(token => token.Trim())
                    .ToArray();

            string express = "";
            var result = new Result();

            if (longline.Contains("/") == true)
            {
                var a = new Result();

                foreach (var i in tokens)
                {
                    a = Fraction.TryParse(i);
                    result = result + a;
                    express = express + a.Data.ToString();
                    if (i != tokens.Last())
                        express = express + " + ";
                }

                if (result.IsSuccessful)
                {
                    var f = result.Data as Fraction;
                    txtInOut.Text = f.ToString();
                    lblExpression.Content = express.ToString();
                }
            }
            else
            {
                Console.WriteLine("Int");
                int sum = 0;

                foreach (var i in tokens)
                {
                    sum = sum + int.Parse(i);
                    express = express + i;
                    if (i != tokens.Last())
                        express = express + " + ";
                }

                txtInOut.Text = sum.ToString();
                lblExpression.Content = express.ToString();
            }
        }



    }
}

