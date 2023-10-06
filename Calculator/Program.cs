using System;

class Calculator
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
        double result = 0;

        for (int i = 0; i < Math.Abs(num2); i++)
        {
            result += num1;
        }




        return (num2 < 0) ? -result : result;
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

class Program
{
    static void Main(string[] args)
    {
        Calculator calculator = new Calculator();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("Select an operation:");
            Console.WriteLine("1. Add");
            Console.WriteLine("2. Subtract");
            Console.WriteLine("3. Multiply");
            Console.WriteLine("4. Divide");
            Console.WriteLine("5. Exit");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 5)
            {
                Console.WriteLine("Invalid input. Please enter a valid option (1-5).");
                continue;
            }

            if (choice == 5)
            {
                exit = true;
                continue;
            }

            Console.Write("Enter the first number: ");
            double num1;
            if (!double.TryParse(Console.ReadLine(), out num1))
            {
                Console.WriteLine("Invalid input for number. Please enter a valid number.");
                continue;
            }

            Console.Write("Enter the second number: ");
            double num2;
            if (!double.TryParse(Console.ReadLine(), out num2))
            {
                Console.WriteLine("Invalid input for number. Please enter a valid number.");
                continue;
            }

            try
            {
                double result = 0;
                switch (choice)
                {
                    case 1:
                        result = calculator.Add(num1, num2);
                        break;
                    case 2:
                        result = calculator.Subtract(num1, num2);
                        break;
                    case 3:
                        result = calculator.Multiply(num1, num2);
                        break;
                    case 4:
                        result = calculator.Divide(num1, num2);
                        break;
                }

                Console.WriteLine($"Result: {result}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}