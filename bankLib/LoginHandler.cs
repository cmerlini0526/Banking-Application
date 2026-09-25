namespace bankLib;
using bankLib.DB;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Identity.Client.Extensibility;
using System.ComponentModel;
using System.Reflection.Metadata;

public class LoginHandler
{
    private static BankAppDbContext db = new BankAppDbContext();
    PasswordHider ph = new PasswordHider();

    public static User Login(string userInput = "")
    {
        string? uName;
        string pWord;
        int unAttempts = 0;
        int pwAttempts = 0;
        while (true)
        {
            if (userInput.IsNullOrEmpty())
            {
                Console.WriteLine("Please enter your username: ");
                uName = Console.ReadLine();
            }
            else
            {
                uName = userInput;
            }
            try
            {
                if (unAttempts >= 3)
                {
                    throw new WarningException("Too many attempts. Returning to menu...");
                }
                if (!db.Users.Any(c => c.UserName == uName))
                {
                    throw new Exception("Username not found, please try again.");
                }
                else
                {
                    if (userInput.IsNullOrEmpty())
                    {
                        Console.WriteLine("Username Valid.");                        
                    }

                    break;
                }
            }
            catch (WarningException e)
            {
                Console.WriteLine(e.Message);
                break;
            }
            catch (Exception e)
            {
                unAttempts++;
                Console.WriteLine(e.Message);
            }

        }
        while (true)
        {
            if (unAttempts >= 3)
            {
                break;
            }
            if (userInput.IsNullOrEmpty())
            {
                Console.WriteLine("Please Enter Your Password: ");
            }
            else
            {
                Console.WriteLine("Please Enter Current Password:");
            }
            pWord = PasswordHider.ReadInput();

            var validUser = db.Users.Single(e => e.UserName == uName);
            try
            {
                if (pwAttempts > 3)
                {
                    throw new WarningException("Too many attempts. Please try again later");
                }
                else if (pWord != validUser.UserPass)
                {
                    pwAttempts++;
                    throw new Exception("Incorrect Password.");
                }
                else
                {
                    break;
                }
            }
            catch (WarningException)
            {
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        if (unAttempts < 3 && pwAttempts < 3)
        {
            Console.Clear();
            return db.Users.Single(e => e.UserName == uName);
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Too many attempts. Please try again later");
            return null;
        }
    }
}

