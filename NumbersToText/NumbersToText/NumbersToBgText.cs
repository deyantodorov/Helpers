namespace NumbersToText
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Converts decimal number to Bulgarian currency text
    /// Конвертиране на число към текст в левове и стотинки. 
    /// Примерен вход: "123.123"
    /// Примерен изход: "сто двадесет и три лева и сто двадесет и три стотинки"
    /// </summary>
    public class NumbersToBgText
    {
        private const string TooBigNumber = "Твърде голямо число";
        private const string InvalidNumber = "Невалидно число";

        /// <summary>
        /// Convert any valid decimal number from 0 to 2000000000 to Bulgarian text. You could use values as 123.123 or just 123.
        /// </summary>
        /// <param name="inputNumber">Example input: "123.123"</param>
        /// <returns>Example output: "сто двадесет и три лева и сто двадесет и три стотинки"</returns>
        public static string Number2BgLeva(double inputNumber)
        {
            string[] input = Regex.Split(inputNumber.ToString("F2"), @"\.");

            int leva = GetNumber(input[0]);
            int stotinka = input.Length == 1 ? 0 : GetNumber(input[1]);

            var text = Number2BgText(leva);
            text += leva == 1 ? " лев" : " лева";

            if (stotinka != 0)
            {
                text = text.Replace("един ", string.Empty);
            }

            if (stotinka >= 1 && stotinka != 0)
            {
                text += " и " + Number2BgText(stotinka, true);
                text += stotinka == 1 ? " стотинка" : " стотинки";
            }

            return text;
        }

        private static int GetNumber(string input)
        {
            int number;

            if (int.TryParse(input, out number))
            {
                number = int.Parse(input);

                if (number > 2000000000)
                {
                    throw new ArgumentOutOfRangeException("input", TooBigNumber);
                }
            }
            else
            {
                throw new ArgumentException("input", InvalidNumber);
            }

            return number;
        }

        /// <summary>
        /// Convert integer values to Bulgarian text.
        /// </summary>
        /// <param name="inputNumber">Input value for conversion</param>
        /// <param name="stotinki">Is number have stotinki or not :)</param>
        /// <returns></returns>
        private static string Number2BgText(int inputNumber, bool stotinki = false)
        {
            var num0 = new Dictionary<int, string>
            {
                {0, "нула"},
                {1, "един"},
                {2, "две"},
                {3, "три"},
                {4, "четири"},
                {5, "пет"},
                {6, "шест"},
                {7, "седем"},
                {8, "осем"},
                {9, "девет"},
                {10, "десет"},
                {11, "единадесет"},
                {12, "дванадесет"}
            };

            var num100 = new Dictionary<int, string>
            {
                {1, "сто"}, 
                {2, "двеста"}, 
                {3, "триста"} 
            };

            int number = int.Parse(inputNumber.ToString());

            var div10 = (number - number % 10) / 10;
            var mod10 = number % 10;
            var div100 = (number - number % 100) / 100;
            var mod100 = number % 100;
            var div1000 = (number - number % 1000) / 1000;
            var mod1000 = number % 1000;
            var div1000000 = (number - number % 1000000) / 1000000;
            var mod1000000 = number % 1000000;
            var div1000000000 = (number - number % 1000000000) / 1000000000;
            var mod1000000000 = number % 1000000000;

            if (number == 0)
            {
                return num0[number];
            }

            // До двайсет
            if (number > 0 && number < 20)
            {
                if (stotinki && number == 1)
                {
                    return "една";
                }

                if (stotinki && number == 2)
                {
                    return "две";
                }

                if (number == 2)
                {
                    return "два";
                }

                var isContains = num0.ContainsKey(number);

                return isContains ? num0[number] : num0[mod10] + "надесет";
            }

            // До сто
            if (number > 19 && number < 100)
            {
                var numberToText = (div10 == 2) ? "двадесет" : num0[div10] + "десет";
                numberToText = mod10 != 0 ? numberToText + " и " + Number2BgText(mod10, stotinki) : numberToText;
                return numberToText;
            }

            /* До хиляда */
            if (number > 99 && number < 1000)
            {
                var isContains = num100.ContainsKey(div100);
                var numberToText = isContains ? num100[div100] : num0[div100] + "стотин";

                if ((mod100 % 10 == 0 || mod100 < 20) && mod100 != 0)
                {
                    numberToText += " и";
                }

                if (mod100 != 0)
                {
                    numberToText += " " + Number2BgText(mod100);
                }

                return numberToText;
            }

            /* До милион */
            if (number > 999 && number < 1000000)
            {
                var numberToText = (div1000 == 1) ? "хиляда" : ((div1000 == 2) ? "две хиляди" : Number2BgText(div1000) + " хиляди");
                num0[2] = "два";

                if ((mod1000 % 10 == 0 || mod1000 < 20) && mod1000 != 0)
                {
                    if (!((mod100 % 10 == 0 || mod100 < 20) && mod100 != 0))
                    {
                        numberToText += " и";
                    }
                }

                if ((mod1000 % 10 == 0 || mod1000 < 20) && mod1000 != 0 && mod1000 < 100)
                {
                    numberToText += " и";
                }

                if (mod1000 != 0)
                {
                    numberToText += " " + Number2BgText(mod1000);
                }

                return numberToText;
            }

            /* Над милион */
            if (number > 999999 && number < 1000000000)
            {
                var numberToText = div1000000 == 1 ? "един милион" : Number2BgText(div1000000) + " милиона";
                if ((mod1000000 % 10 == 0 || mod1000000 < 20) && mod1000000 != 0)
                {
                    if (!((mod1000 % 10 == 0 || mod1000 < 20) && mod1000 != 0))
                    {
                        if (!((mod100 % 10 == 0 || mod100 < 20) && mod100 != 0))
                        {
                            numberToText += " и";
                        }
                    }
                }

                if ((mod1000000 % 10 == 0 || mod1000000 < 20) && mod1000000 != 0 && mod1000000 < 1000)
                {
                    if ((mod1000 % 10 == 0 || mod1000 < 20) && mod1000 != 0 && mod1000 < 100)
                    {
                        numberToText += " и";
                    }
                }

                if (mod1000000 != 0)
                {
                    numberToText += " " + Number2BgText(mod1000000);
                }

                return numberToText;
            }

            /* Над милиард */
            if (number > 99999999 && number <= 2000000000)
            {
                var numberToText = (div1000000000 == 1) ? "един милиард" : "";
                numberToText = (div1000000000 == 2) ? "два милиарда" : numberToText;

                if (mod1000000000 != 0)
                {
                    numberToText += " " + Number2BgText(mod1000000000);
                }

                return numberToText;
            }

            return string.Empty;
        }
    }
}