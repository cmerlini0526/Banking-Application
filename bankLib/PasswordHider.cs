namespace bankLib;

using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

public class PasswordHider
{
    public static string ReadInput(char mask = '*')
    {
        StringBuilder input = new StringBuilder();
        ConsoleKeyInfo keyInfo;

        while (true)
        {
            keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input.Append(keyInfo.KeyChar);
                Console.Write(mask);
            }
        }
        return input.ToString();
    }
}
