try
{
    Console.WriteLine("Hello, World!");
    throw new Exception("wtf");
}
catch (Exception exception)
{
    var foregroundColor = Console.ForegroundColor;

    Console.ForegroundColor = ConsoleColor.Red;

    Console.Error.WriteLine();
    Console.Error.WriteLine("The following error occurred:");
    Console.Error.WriteLine();
    Console.Error.WriteLine(exception);
    Console.Error.WriteLine();

    Console.ForegroundColor = foregroundColor;

    Environment.Exit(1);
}


