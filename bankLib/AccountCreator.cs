namespace bankLib;
using bankLib.DB;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Identity.Client.Extensibility;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

public static class AccountCreator
{
    public static void CreateAccount(User? user = null)
    {
        #region Initializing Variables

        string? aName = "";
        string aType = "";
        int aBalance = 0;
        string aBranch = "";
        int branchChoice = 0;
        int typeChoice = 0;
        if (user == null)
        {
            user = LoginHandler.Login();
        }
        try
        {
            if (user == null)
            {
                throw new Exception("Returning to menu...");
            }

        #endregion

        #region Enter Your Name

        while (true)
        {
            try
            {
                Console.WriteLine("Please enter your name: ");
                aName = Console.ReadLine();
                if (aName == "0")
                {
                    throw new TimeoutException();
                }
                else if (aName.IsNullOrEmpty())
                {
                    throw new Exception("Invalid name. Please try again");
                }
                else if (aName.Any(c => !char.IsLetter(c)))
                {
                    throw new Exception("Must only contain letters");
                }
                else
                {
                    Console.WriteLine("Name accepted!");
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

        #endregion

        #region Choose Your Branch

        while (true)
        {
            Console.WriteLine("Which branch will you be banking with?");
            Console.WriteLine("1. New York");
            Console.WriteLine("2. Dallas");
            Console.WriteLine("3. Miami");
            Console.WriteLine("4. San Francisco");
            try
            {
               branchChoice = Convert.ToInt32(Console.ReadLine());
                if (branchChoice < 1 || branchChoice > 4)
                {
                    throw new Exception();
                }
                else
                {
                    switch (branchChoice)
                    {
                        case 1:
                            aBranch = "New York";
                            break;
                        case 2:
                            aBranch = "Dallas";
                            break;
                        case 3:
                            aBranch = "Miami";
                            break;
                        case 4:
                            aBranch = "San Francisco";
                            break;
                    }
                    break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Input, please enter 1-4");
            }
        }

        #endregion

        #region Choose Your Account Type

        while (true)
        {
            Console.WriteLine("What kind of account would you like to open?");
            Console.WriteLine("1. Savings");
            Console.WriteLine("2. Checking");
            Console.WriteLine("3. Loan");
            try
            {
               typeChoice = Convert.ToInt32(Console.ReadLine());
                if (typeChoice < 1 || typeChoice > 3)
                {
                    throw new Exception();
                }
                else
                {
                    switch (typeChoice)
                    {
                        case 1:
                            aType = "Savings";
                            break;
                        case 2:
                            aType = "Checking";
                            break;
                        case 3:
                            aType = "Loan";
                            break;
                    }
                    break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Input, please enter 1-3");
            }
        }

        #endregion

        #region Set Starting Balance

        while (true)
        {
            try
            {
                Console.WriteLine("Please enter your starting balance: ");
                aBalance = Convert.ToInt32(Console.ReadLine());
                if (aBalance >= 0)
                {
                    Console.WriteLine("Balance Confirmed");
                    break;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Input, please enter a positive whole number");
            }
        }

        #endregion

        UIComponents.db.Add(new Account()
        {
            AccName = aName,
            AccBranch = aBranch,
            AccBalance = aBalance,
            AccType = aType,
            AccIsActive = true,
            AccOwnerId = user.UserId
        });
        UIComponents.db.SaveChanges(); 

        Console.WriteLine("Account Created Successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
