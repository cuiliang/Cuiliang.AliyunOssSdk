using System;
using System.Threading.Tasks;
using Sample;

class Program
{
    public static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();

        Console.ReadKey();
    }

    public static async Task MainAsync(string[] args)
    {
        try
        {
            await NewSdkTester.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ex:" + ex.Message);
            throw;
        }


    }
}