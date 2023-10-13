using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorOperations
{
    public class CalculatorOperation
    {
        public double Add(double num1, double num2)
        {
            return num1 + num2;
        }

        public double Subtract(double num1, double num2)
        {
            return num1 - num2;
        }

        public double Multiply(double num1, double num2)
        {
            //for (int i = 0; i < Math.Abs(num2); i++)
            //{
            //    result += num1;
            //}

            //return (num2 < 0) ? -result : result;

            int sign = 1;

            if (num1 < 0)
            {
                num1 = -num1;
                sign = -sign;
            }

            if (num2 < 0)
            {
                num2 = -num2;
                sign = -sign;
            }

            double result = 0;

            for (int i = 0; i < num2; i++)
            {
                result += num1;
            }

            return sign * result;
        }

        public double Divide(double num1, double num2)
        {
            if (num2 == 0)
            {
                throw new DivideByZeroException("Division by zero is not allowed.");
            }

            double result = 0;
            double absNum1 = Math.Abs(num1);
            double absNum2 = Math.Abs(num2);

            while (absNum1 >= absNum2)
            {
                absNum1 -= absNum2;
                result++;
            }

            if ((num1 < 0 && num2 > 0) || (num1 > 0 && num2 < 0))
            {
                result = -result;
            }

            return result;
        }
    }
}
