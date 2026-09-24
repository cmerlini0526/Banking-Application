namespace bankLib;

using bankLib.DB;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.ComponentModel.DataAnnotations;

public class UserCreator
{
    static BankAppDbContext db = new BankAppDbContext();
    
    public static void CreateUser(bool isAdmin=false)
    {
        string? uName = "";
        while (true)
        {
            Console.WriteLine("Please choose a username: ");
            uName = Console.ReadLine();
            try
            {
                if (uName == "0")
                {
                    throw new TimeoutException();
                }
                else if ((uName.Any(c => !char.IsLetterOrDigit(c))) || uName.IsNullOrEmpty())
                {
                    throw new Exception("Only alpha-numeric characters allowed! Please try again or enter 0 to exit");
                }
                else if (db.Users.Any(u => u.UserName == uName))
                {
                    throw new Exception("Username taken! Please try again or enter 0 to exit");
                }
                else
                {
                    Console.WriteLine("Username Valid.");
                    break;
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Returning...");
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        if (uName != "0" && !uName.IsNullOrEmpty())
        {
            Console.WriteLine("Please Choose Password: ");
            string pWord = SetPassword();

            db.Add(new User()
            {
                UserName = uName,
                UserPass = pWord
            });
            db.SaveChanges();
            if (isAdmin)
            {
                Console.WriteLine("Valid Credentials. User: " + uName + " created");

            }
            else
            {
                Console.WriteLine("Credentials Accepted! Welcome to BANK " + uName);
            }
        }

    }

    public static string SetPassword()
    {
        string pWord = "";
        while (true)
        {
            pWord = PasswordHider.ReadInput();
            Console.WriteLine();
            try
            {
                if (pWord == "0")
                {
                    throw new TimeoutException();
                }
                else if (pWord.IsNullOrEmpty() || (pWord.Length < 8))
                {
                    throw new Exception("Password too short. Please try again");
                }
                else if ((!pWord.Any(c => char.IsDigit(c))))
                {
                    throw new Exception("Password must contain at least 1 number. Please try again");
                }
                else if (!pWord.Any(c => char.IsSymbol(c)) && !pWord.Any(c => char.IsPunctuation(c)))
                {
                    throw new Exception("Password must contain at least one special character. Please try again");
                }
                else
                {
                    Console.WriteLine("Password Valid!");
                    return pWord;
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Returning...");
                throw new TimeoutException();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
