using Calculator.Enum;
using CalculatorOperations;
using System;

class Program
{
    static void Main(string[] args)
    {
        CalculatorOperation calculator = new CalculatorOperation();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("Select an operation:");
            Console.WriteLine($"{(int)CalculatorOperationvalues.Add}. Add");
            Console.WriteLine($"{(int)CalculatorOperationvalues.Subtract}. Subtract");
            Console.WriteLine($"{(int)CalculatorOperationvalues.Multiply}. Multiply");
            Console.WriteLine($"{(int)CalculatorOperationvalues.Divide}. Divide");
            Console.WriteLine($"{(int)CalculatorOperationvalues.Exit}. Exit");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(CalculatorOperationvalues), choice))
            {
                Console.WriteLine("Invalid input. Please enter a valid option (1-5).");
                continue;
            }

            CalculatorOperationvalues operation = (CalculatorOperationvalues)choice;

            if (operation == CalculatorOperationvalues.Exit)
            {
                exit = true;
                continue;
            }

            Console.Write("Enter the first number: ");
            double num1;
            if (!double.TryParse(Console.ReadLine(), out num1))
            {
                Console.WriteLine("Invalid input for the number. Please enter a valid number.");
                continue;
            }

            Console.Write("Enter the second number: ");
            double num2;
            if (!double.TryParse(Console.ReadLine(), out num2))
            {
                Console.WriteLine("Invalid input for the number. Please enter a valid number.");
                continue;
            }

            double result = 0;

            switch (operation)
            {
                case CalculatorOperationvalues.Add:
                    result = calculator.Add(num1, num2);
                    break;
                case CalculatorOperationvalues.Subtract:
                    result = calculator.Subtract(num1, num2);
                    break;
                case CalculatorOperationvalues.Multiply:
                    result = calculator.Multiply(num1, num2);
                    break;
                case CalculatorOperationvalues.Divide:
                    try
                    {
                        result = calculator.Divide(num1, num2);
                    }
                    catch (DivideByZeroException ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    break;
            }

            Console.WriteLine($"Result: {result}");
        }
    }
}
