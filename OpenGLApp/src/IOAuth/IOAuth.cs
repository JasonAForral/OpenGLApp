using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGLApp.src.IOAuth
{
    public class IOAuthj
    {
        protected readonly string DatePattern = "MM-DD-yyy";
        public bool IsValidCurrency(string inputString, int maxLength)
        {
            double inputCurrency;
            double exclusiveUpperBound;

            if (maxLength > 311 || inputString.Length > maxLength || string.IsNullOrEmpty(inputString))
                return false;

            try
            {
                inputCurrency = double.Parse(inputString);
            }
            catch
            {
                return false;
            }

            exclusiveUpperBound = Math.Pow(10, maxLength - 3);
            return exclusiveUpperBound >= 0 && inputCurrency < exclusiveUpperBound;
        }

        public bool IsValidDate(string inputString, out DateTime result)
        {
            return (DateTime.TryParseExact(inputString, DatePattern, null, System.Globalization.DateTimeStyles.None, out result));
        }



        public bool IsType(string input, string valueType)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input is null or white space", nameof(input));
            }

            if (string.IsNullOrWhiteSpace(valueType))
            {
                throw new ArgumentException("Value type is null or white space", nameof(valueType));
            }

            valueType = valueType.ToLowerInvariant();

            return valueType switch
            {
                "double" => double.TryParse(input, out _),
                "int" => int.TryParse(input, out _),
                _ => throw new ArgumentException(),
            };
        }
    }
}
