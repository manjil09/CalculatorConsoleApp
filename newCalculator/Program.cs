// See https://aka.ms/new-console-template for more information
while (true)
{
    Console.WriteLine("Enter an expression (e.g '1+2*3' or 'q' to quit):");
    string expresstion = Console.ReadLine() ?? "";

    if (expresstion.ToLower() == "q")
        break;

    try
    {
        double result = evaluateExpression(expresstion);
        Console.WriteLine($"\nResult: {result}\n");
    }
    catch (ArgumentException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("\nPlease enter a valid expression.\n");
    }
}

double evaluateExpression(string expression)
{
    Stack<double> operandStack = new();
    Stack<char> operatorStack = new();

    for (int i = 0; i < expression.Length; i++)
    {
        char currentChar = expression[i];

        if (char.IsDigit(currentChar) || currentChar == '.')
        {
            string operand = currentChar.ToString();
            while (i < expression.Length - 1 && (char.IsDigit(expression[i + 1]) || expression[i + 1] == '.'))
            {
                operand += expression[i + 1];
                i++;
            }
            operandStack.Push(double.Parse(operand));
        }
        else if (currentChar == '(')
        {
            if (i == 0 || !isOperator(expression[i - 1]))
                throw new InvalidOperationException();

            operatorStack.Push(currentChar);
        }
        else if (currentChar == ')')
        {
            if (i >= expression.Length - 1 || !isOperator(expression[i + 1]))
                throw new InvalidOperationException();

            while (operatorStack.Count > 0 && getPriority(currentChar) <= getPriority(operatorStack.Peek()) && operatorStack.Peek() != '(')
            {
                double value = performOperation(operatorStack.Pop(), operandStack.Pop(), operandStack.Pop());
                operandStack.Push(value);
            }
            operatorStack.Pop();

        }
        else if (isOperator(currentChar))
        {
            while (operatorStack.Count > 0 && getPriority(currentChar) <= getPriority(operatorStack.Peek()))
            {
                double value = performOperation(operatorStack.Pop(), operandStack.Pop(), operandStack.Pop());
                operandStack.Push(value);
            }
            operatorStack.Push(currentChar);
        }
    }
    while (operatorStack.Count > 0)
    {
        double value = performOperation(operatorStack.Pop(), operandStack.Pop(), operandStack.Pop());
        operandStack.Push(value);
    }
    return operandStack.Pop();
}
int getPriority(char @operator)
{
    return @operator switch
    {
        '+' or '-' => 1,
        '*' or '/' => 2,
        _ => 0
    };
}
double performOperation(char @operator, double num1, double num2) => @operator switch
{
    '+' => num2 + num1,
    '-' => num2 - num1,
    '*' => num2 * num1,
    '/' => num2 / num1,
    _ => 0,
};
bool isOperator(char @operator)
{
    if (char.IsDigit(@operator))
        return false;
    if (@operator is '+' or '-' or '*' or '/' or '(' or ')')
        return true;
    else
        throw new ArgumentException("\nIvalid operator.\n");
}