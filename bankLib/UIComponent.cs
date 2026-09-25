using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection.Metadata;
using bankLib.DB;
using bankLib;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Storage;
using Azure.Identity;
using System.Linq.Expressions;

namespace bankLib
{
    public class UIComponents
    {
        public static BankAppDbContext db = new BankAppDbContext();
        public virtual void MainMenu()
        {
            int userChoice = 0;
            while (true)
            {
                Console.WriteLine("!~~~~~~~~ Welcome to Bank ~~~~~~~~!");
                Console.WriteLine("1. Customer");
                Console.WriteLine("2. Admin");
                Console.WriteLine("3. New Customer");
                Console.WriteLine("4. Exit");

                try
                {
                    userChoice = Convert.ToInt32(Console.ReadLine());
                    switch (userChoice)
                    {
                        case 1:
                            User loggedCustomer = LoginHandler.Login();
                            if (loggedCustomer == null)
                            {
                                throw new DataException("Returning...");   
                            }
                            else
                            {
                                UserMenu(loggedCustomer);
                            }
                            break;
                        case 2:
                            User loggedAdmin = LoginHandler.Login();
                            if (loggedAdmin == null)
                            {
                                throw new DataException("Returning...");   
                            }
                            else if (Convert.ToBoolean(loggedAdmin.UserIsAdmin))
                            {
                                AdminMenu(loggedAdmin);
                            }
                            else
                            {
                                Console.WriteLine("User is not an Admin");
                                throw new DataException("Returning...");
                            }
                            break;
                        case 3:
                            UserCreator.CreateUser();
                            break;
                        case 4:
                            Console.WriteLine("Thank you for banking with us!");
                            throw new WarningException();
                        default:
                            throw new Exception();
                    }
                }
                catch (DataException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (WarningException)
                {
                    break;
                }
                catch
                {
                    Console.WriteLine("Please enter a number 1-4");
                }

            }
        }

        public void UserMenu(User user)
        {
            int userChoice = 0;
            var userAccounts = db.Accounts.Where(e => e.AccOwnerId == user.UserId).ToList();

            while (true)
            {
            Console.Clear();
            Console.WriteLine("Welcome, " + user.UserName + "!");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Check Account Details");
		    Console.WriteLine("2. Withdraw");
		    Console.WriteLine("3. Deposit");
		    Console.WriteLine("4. Transfer");
		    Console.WriteLine("5. Last 5 Transactions");
		    Console.WriteLine("6. Request Check Book");
		    Console.WriteLine("7. Change Password");
		    Console.WriteLine("8. Exit");

                try
                {
                    userChoice = Convert.ToInt32(Console.ReadLine());
                    
                    Console.Clear();
                    
                    switch (userChoice)
                    {
                        #region 1. Get Account Details

                        case 1:
                            while (true)
                            {
                                Console.WriteLine("Which account details would you like to see?");
                                Console.WriteLine("1. Checking");
                                Console.WriteLine("2. Savings");
                                Console.WriteLine("3. Loan");
                                Console.WriteLine("4. Return");
                                try
                                {
                                    int accountChoice = Convert.ToInt32(Console.ReadLine());
                                    switch (accountChoice)
                                    {
                                        case 1:
                                            AccountActions.PrintDetails(CheckAccount(userAccounts, "Checking", user));
                                            break;
                                        case 2:
                                            AccountActions.PrintDetails(CheckAccount(userAccounts, "Savings", user));
                                            break;
                                        case 3:
                                            AccountActions.PrintDetails(CheckAccount(userAccounts, "Loan", user));
                                            break;
                                        case 4:
                                            throw new WarningException();
                                 }
                                }
                                catch (DataException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (WarningException)
                                {
                                    break;
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a number 1 - 4");
                                }                           
                            }
                            break;

                        #endregion

                        #region 2. Withdraw

                        case 2:
                            while (true)
                            {
                                Console.WriteLine("Which account would you like to withdraw from?");
                                Console.WriteLine("1. Checking");
                                Console.WriteLine("2. Savings");
                                Console.WriteLine("3. Return to Menu");
                                try
                                {
                                    int withChoice = Convert.ToInt32(Console.ReadLine());
                                    Account withAcc;
                                    switch (withChoice)
                                    {
                                    case 1:
                                        withAcc = CheckAccount(userAccounts, "Checking", user);
                                        if (!withAcc.AccIsActive)
                                        {
                                            throw new WarningException(withAcc.AccType + " account is not active");
                                        }
                                        CallWithdraw(withAcc);
                                        break;

                                    case 2:
                                        withAcc = CheckAccount(userAccounts, "Savings", user);
                                        if (!withAcc.AccIsActive)
                                        {
                                            throw new WarningException(withAcc.AccType + " account is not active");
                                        }
                                        CallWithdraw(withAcc);
                                        break;                       
                                    case 3:
                                    throw new TimeoutException();
                                    default:
                                        throw new Exception();
                                    }
                                }
                                catch (TimeoutException)
                                {
                                    break;
                                }
                                catch (DataException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a number 1-3");
                                }
                            }
                        break;

                        #endregion

                        #region 3. Deposit

                        case 3:
                            while (true)
                            {
                                Console.WriteLine("Which account would you like to deposit to?");
                                Console.WriteLine("1. Checking");
                                Console.WriteLine("2. Savings");
                                Console.WriteLine("3. Loan");
                                Console.WriteLine("4. Return to Menu");
                                try
                                {
                                    int depChoice = Convert.ToInt32(Console.ReadLine());
                                    Account depAcc;

                                    switch (depChoice)
                                    {
                                    case 1:
                                        depAcc = CheckAccount(userAccounts, "Checking", user);
                                        if (!depAcc.AccIsActive)
                                        {
                                            throw new WarningException(depAcc.AccType + " account is not active");
                                        }
                                        CallDeposit(depAcc);
                                        break;     
                                    case 2:
                                        depAcc = CheckAccount(userAccounts, "Savings", user);
                                        if (!depAcc.AccIsActive)
                                        {
                                            throw new WarningException(depAcc.AccType + " account is not active");
                                        }
                                        CallDeposit(depAcc);
                                        break;  
                                    case 3:
                                        depAcc = CheckAccount(userAccounts, "Loan", user);
                                        if (!depAcc.AccIsActive)
                                        {
                                            throw new WarningException(depAcc.AccType + " account is not active");
                                        }
                                        CallDeposit(depAcc);
                                        break;     
                                    case 4:
                                        throw new TimeoutException();
                                    default:
                                        throw new Exception();
                                    }
                                }
                                catch (WarningException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (TimeoutException)
                                {
                                    Console.WriteLine("Returning...");
                                    break;
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a number 1-3");
                                }
                            }
                            break;

                        #endregion

                        #region 4. Transfer

                        case 4:
                            while (true)
                            {
                                Console.WriteLine("Which account would you like to transfer from?");
                                Console.WriteLine("1. Checking");
                                Console.WriteLine("2. Savings");
                                Console.WriteLine("3. Return to Menu");
                                try
                                {
                                    int transferChoice = Convert.ToInt32(Console.ReadLine());
                                    Account? transAcc = null;

                                    switch (transferChoice)
                                    {
                                    case 1:
                                        transAcc = CheckAccount(userAccounts, "Checking", user);
                                        if (!transAcc.AccIsActive)
                                        {
                                            throw new WarningException(transAcc.AccType + " account is not active");
                                        }
                                        CallTransfer(transAcc);
                                        break;
                                    case 2:
                                        transAcc = CheckAccount(userAccounts, "Savings", user);
                                        if (!transAcc.AccIsActive)
                                        {
                                            throw new WarningException(transAcc.AccType + " account is not active");
                                        }
                                        CallTransfer(transAcc);
                                        break;  
                                    case 3:
                                        throw new TimeoutException();
                                    default:
                                        throw new Exception();
                                    }
                                }
                                catch (WarningException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (TimeoutException)
                                {
                                    Console.WriteLine("Returning...");
                                    break;
                                }
                                catch (DataException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a number 1-3");
                                }
                            }
                            break;


                        #endregion

                        #region 5. Last 5 Transactions

                        case 5:
                            try
                            {
                            var lastTrans = db.TransactionHistories.Where(e => e.UserId == user.UserId)
                                                                    .OrderByDescending(e => e.TransId)
                                                                    .Take(5)
                                                                    .ToList();
                            int count = 1;
                            foreach (var item in lastTrans)
                            {
                                string change;
                                if (item.TransChange < 0)
                                    {
                                        change = "-" + Math.Abs(item.TransChange).ToString("C");
                                    }
                                else
                                    {
                                        change = item.TransChange.ToString("C");
                                    }
                                double newBal = item.TransOldBal + item.TransChange;
                                Console.WriteLine($"==============================");
                                Console.WriteLine($"Transaction #{count}: ");
                                Console.WriteLine("Transaction ID     : " + item.TransId);
                                Console.WriteLine("Transaction Date   : " + item.TransDate);
                                Console.WriteLine("Transaction Type   : " + item.TransType);
                                Console.WriteLine("Transaction Amount : " + change);
                                Console.WriteLine("Original Balance   : " + item.TransOldBal.ToString("C"));
                                Console.WriteLine("New Balance        : " + newBal.ToString("C"));
                                count++;
                            }
                            WaitToReturn();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;

                        #endregion
                        
                        #region 6. Request Check Book

                        case 6:
                            while (true)
                            {
                                Console.WriteLine("Which account would you like to request a check book for?");
                                Console.WriteLine("1. Checking");
                                Console.WriteLine("2. Savings");
                                Console.WriteLine("3. Return");
                                try
                                {
                                    int accountChoice = Convert.ToInt32(Console.ReadLine());
                                    Account accReq;
                                    switch (accountChoice)
                                    {
                                        case 1:
                                            AccountActions.RequestCheck(CheckAccount(userAccounts, "Checking", user));
                                            break;
                                        case 2:
                                            AccountActions.RequestCheck(CheckAccount(userAccounts, "Savings", user));
                                            break;
                                        case 3:
                                            throw new WarningException();
                                        default:
                                            throw new Exception();
                                 }
                                }
                                catch (DataException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (WarningException)
                                {
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    Console.WriteLine("Please enter a number 1 - 3");
                                }                           
                            }
                            break;

                        #endregion

                        case 7:
                            User? passChange;
                            try
                            {
                                passChange = LoginHandler.Login(user.UserName);
                                if (passChange == null)
                                {
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Enter New Password:");
                                    passChange.UserPass = UserCreator.SetPassword();
                                    db.Update(passChange);
                                    db.SaveChanges();
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                break;
                            }



                        case 8:
                            Console.WriteLine("Returning to Main Menu...");
                            throw new WarningException();
                        default:
                            throw new Exception();   
                    }
                }
                catch (WarningException)
                {
                    break;
                }
                catch
                {
                    Console.WriteLine("Please enter a number 1-8");
                }

            }

        }

        public void AdminMenu(User admin)
        {
            int userChoice = 0;
            while (true)
            {
            Console.Clear();
            Console.WriteLine("Welcome, Admin!");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Create New User/Account");
		    Console.WriteLine("2. Delete User/Account");
		    Console.WriteLine("3. Edit User/Account Details");
		    Console.WriteLine("4. Display Summary");
		    Console.WriteLine("5. Reset Customer Password");
		    Console.WriteLine("6. Approve Checkbook Request");
		    Console.WriteLine("7. Exit");

                try
                {
                    userChoice = Convert.ToInt32(Console.ReadLine());
                    Console.Clear();
                    
                    switch (userChoice)
                    {
                        #region 1. Create New User/Account

                        case 1:
                            while (true)
                            {
                                Console.WriteLine("Which would you like to create?");
                                Console.WriteLine("1. User");
                                Console.WriteLine("2. Account");
                                Console.WriteLine("3. Return");
                                try
                                {
                                    int createChoice = Convert.ToInt32(Console.ReadLine());
                                    switch (createChoice)
                                    {
                                        case 1:
                                            UserCreator.CreateUser(true);
                                            break;
                                        case 2:
                                            AccountCreator.CreateAccount();
                                            break;
                                        case 3:
                                            throw new WarningException();
                                 }
                                }
                                catch (DataException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (WarningException)
                                {
                                    break;
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a number 1 - 4");
                                }                           
                            }
                            break;

                        #endregion

                        #region 2. Delete User/Account

                        case 2:
                            while (true)
                            {
                                Console.WriteLine("Which would you like to delete?");
                                Console.WriteLine("1. User");
                                Console.WriteLine("2. Account");
                                Console.WriteLine("3. Return to Menu");
                                try
                                {
                                    int delChoice = Convert.ToInt32(Console.ReadLine());
                                    switch (delChoice)
                                    {
                                    case 1:
                                        Console.WriteLine("Enter the UserID you would like to delete: ");
                                        AdminActions.DeleteAccount("User", admin);
                                        break;
                                    case 2:
                                        Console.WriteLine("Enter the AccountID you would like to delete: ");
                                        AdminActions.DeleteAccount("Account", admin);
                                        break;                   
                                    case 3:
                                    throw new TimeoutException();
                                    default:
                                        throw new Exception();
                                    }
                                }
                                catch (TimeoutException)
                                {
                                    break;
                                }
                                catch (DataException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a number 1-3");
                                }
                            }
                        break;

                        #endregion

                        #region 3. Edit User/Account Details

                        case 3:
                            while (true)
                            {
                                Console.WriteLine("Which would you like to Edit?");
                                Console.WriteLine("1. User");
                                Console.WriteLine("2. Account");
                                Console.WriteLine("3. Return to Menu");
                                try
                                {
                                    int editChoice = Convert.ToInt32(Console.ReadLine());
                                    switch (editChoice)
                                    {
                                    case 1:
                                        Console.WriteLine("Enter the UserID you would like to edit: ");
                                        AdminActions.EditAccount("User", admin);
                                        break;
                                    case 2:
                                        Console.WriteLine("Enter the AccountID you would like to edit: ");
                                        AdminActions.EditAccount("Account", admin);
                                        break;                   
                                    case 3:
                                    throw new TimeoutException();
                                    default:
                                        throw new Exception();
                                    }
                                }
                                catch (TimeoutException)
                                {
                                    break;
                                }
                                catch (DataException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a number 1-3");
                                }
                            }
                        break;

                        #endregion

                        #region 4. Display Summary

                        case 4:
                            AdminActions.PrintSummary();
                            break;


                        #endregion

                        #region 5. Reset Customer Password

                        case 5:
                            try
                            {
                                Console.WriteLine("Enter UserID to reset password: ");
                                AdminActions.ResetPass();
                                break;
                            }
                            catch (TimeoutException)
                            {
                                break;
                            }
                            catch (DataException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;

                        #endregion
                        
                        #region 6. Approve Checkbook Request

                        case 6:
                            while (true)
                            {
                                try
                                {
                                    Console.WriteLine("Outstanding Requests: ");
                                    AdminActions.ApproveReqs();
                                    break;
                                }
                                catch (TimeoutException)
                                {
                                    break;
                                }
                                catch (DataException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            }
                            break;

                        #endregion

                        case 7:
                            Console.WriteLine("Returning to Main Menu...");
                            throw new WarningException();
                        default:
                            throw new Exception();   
                    }
                }
                catch (WarningException)
                {
                    break;
                }
                catch
                {
                    Console.WriteLine("Please enter a number 1-7");
                }

            }

        }

        #region CheckAccount Method

        public Account CheckAccount(List<Account> accounts, string type, User? user = null)
        {
            if (accounts.Any(e => e.AccType == type))
            {
                return accounts.Single(e => e.AccType == type);
            }
            else
            {
                if (user != null)
                {
                throw new DataException(user.UserName + " does not have a " + type + " account");
                }
                else
                {
                    throw new DataException("Account not found");
                }
            }
        }

        #endregion

        #region CheckTransfer Method

        public Account CheckTransfer(Account accSender)
        {
            Account transAcc;
            Console.WriteLine("Please enter the account ID you would like to transfer to: ");
            while (true)
            {
                try
                {
                    int receiverID = Convert.ToInt32(Console.ReadLine());

                    if (receiverID == 0)
                    {
                        throw new TimeoutException();
                    }
                    else if (receiverID == accSender.AccId)
                    {
                        throw new WarningException("Cannot transfer to self");
                    }

                    if (db.Accounts.Any(e => e.AccId == receiverID))
                    {
                        transAcc = db.Accounts.Single(e => e.AccId == receiverID);
                    }
                    else
                    {
                        throw new DataException("Account with ID " + receiverID + " not found");
                    }

                    if (transAcc.AccIsActive && transAcc.AccType != "Loan")
                    {
                        
                        Console.WriteLine("Account found!");
                        return transAcc;
                    }
                    else if (transAcc.AccType == "Loan")
                    {
                        throw new WarningException("Cannot transfer to loan account");
                    }
                    else
                    {
                        throw new WarningException("Account is not active");
                    }
                }
                catch (TimeoutException)
                {
                    throw new TimeoutException();
                }
                catch (WarningException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception)
                {
                    Console.WriteLine("Please enter a valid number or 0 to exit.");
                }

            }
        }
        
        #endregion

        #region CheckValue Method

        public double CheckValue(string type, Account? acc = null)
        {
            while (true)
            {
                try
                {
                    double input = Convert.ToDouble(Console.ReadLine());
                    if (acc == null || input <= acc.AccBalance )
                    {
                        if (input == 0)
                        {
                            throw new TimeoutException();
                        }
                        else if (input < 0)
                        {
                            throw new WarningException("Cannot " + type + " negative balance");
                        }
                        else if (input > 1000000000000000)
                        {
                            throw new WarningException("Input too large. Please try again");
                        }
                        else
                        {
                            return input;
                        }
                    }
                    else
                    {
                        throw new WarningException("Deposit too large, please enter a valid amount up to " + acc.AccBalance);
                    }
                }
                catch (TimeoutException)
                {
                    return -1;
                }
                catch (WarningException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        #endregion

        #region CallWithdraw Method

        public void CallWithdraw(Account withAcc)
        {
            double withAmount;
            Console.WriteLine("Please enter the amount you would like to withdraw");
            withAmount = CheckValue("Withdraw", withAcc);
            if (withAmount == -1)
            {
                Console.WriteLine("Returning...");
            }
            else
            {
            double newBal = AccountActions.Withdraw(withAmount, withAcc);
            Console.WriteLine("Withdrawal successful!");
            Console.WriteLine("New balance is : " + Convert.ToDouble(withAcc.AccBalance).ToString("C"));   
            }
        }

        #endregion

        #region CallDeposit Method

        public void CallDeposit(Account depAcc)
        {
            double depAmount;
            Console.WriteLine("Please enter the amount you would like to deposit");
            if (depAcc.AccType == "Loan")
            {
                depAmount = CheckValue("Deposit", depAcc);
            }
            else
            {
                depAmount = CheckValue("Deposit", null);
            }
            if (depAmount == -1)
            {
                Console.WriteLine("Returning...");
            }
            else
            {
            double newBal = AccountActions.Deposit(depAmount, depAcc);
            Console.WriteLine("Deposit successful!");
            Console.WriteLine("New balance is : " + Convert.ToDouble(depAcc.AccBalance).ToString("C"));   
            }
        }
        
        #endregion

        #region CallTransfer Method

        public void CallTransfer(Account transAcc)
        {
            double transAmount;
            Console.WriteLine("Please enter the amount you would like to transfer");
            transAmount = CheckValue("Transfer");
            if (transAmount == -1)
            {
                Console.WriteLine("Returning...");
            }
            else
            {  
                Account? receiverAcc = null;
                while (true)
                    {
                    try
                    {
                        if (receiverAcc == null)
                        {
                            receiverAcc = CheckTransfer(transAcc);
                        }
                        double newBal = AccountActions.Transfer(transAmount, transAcc, receiverAcc);
                        if (newBal == -1)
                        {
                            Console.WriteLine("Returning...");
                            break;
                        }
                        Console.WriteLine("Transfer successful!");
                        Console.WriteLine(Convert.ToDouble(transAmount).ToString("C") + " sent to account with ID " + receiverAcc.AccId);
                        Console.WriteLine("New balance of sender account is : " + Convert.ToDouble(transAcc.AccBalance).ToString("C"));
                        if (receiverAcc.AccOwnerId == transAcc.AccOwnerId)
                        {
                            Console.WriteLine("New balance of receiver account is: " + Convert.ToDouble(receiverAcc.AccBalance).ToString("C"));
                        }
                        break;
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Returning...");
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Please enter a valid number or 0 to exit.");
                    }
                }
            }
        }

        #endregion

        #region WaitToReturn Method

        public static void WaitToReturn()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to Return to Menu...");

            while (true)
            {
                var keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
        }
        
        #endregion
    }
}