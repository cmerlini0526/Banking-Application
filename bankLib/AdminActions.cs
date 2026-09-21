using System.Data;
using Microsoft.EntityFrameworkCore;
using bankLib.DB;
using System.Collections.Immutable;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Azure.Core;

namespace bankLib
{
    public static class AdminActions
    {
    static BankAppDbContext db = new BankAppDbContext();

        #region DeleteAccount

        public static void DeleteAccount(string type, User admin)
        {
            while (true)
            {
                try
                {
                    int input = Convert.ToInt32(Console.ReadLine());
                    User user = db.Users.Single(e => e.UserId == input);
                    if (type == "User")
                    {
                        if (input == 0)
                        {
                            throw new TimeoutException();

                        }
                        else if (admin.UserId == input)
                        {
                            throw new Exception("Cannot delete self");
                        }
                        else if (user.UserIsAdmin == true)
                        {
                            throw new Exception("Cannot delete admin");
                        }
                        else if (db.Users.Any(e => e.UserId == input))
                        {
                            Console.WriteLine("User #" + input + " Found");
                            Console.WriteLine("Are you sure you would like to delete user and all accounts?");
                            Console.WriteLine("Type YES to delete, or enter anything to return");
                            string response = Console.ReadLine();
                            if (response == "YES")
                            {
                                var accounts = db.Accounts.Where(e => e.AccOwnerId == input).ToList();
                                foreach (Account acc in accounts)
                                {
                                    acc.AccName = null;
                                    acc.AccBalance = null;
                                    acc.AccType = null;
                                    acc.AccBranch = null;
                                    acc.AccIsActive = false;
                                    db.Update(acc);
                                    db.SaveChanges();
                                }
                                user.UserName = null;
                                user.UserPass = null;
                                db.Update(user);
                                db.SaveChanges();
                                Console.WriteLine("User and Accounts Deleted");
                                break;
                            }
                            else
                            {
                                throw new TimeoutException();
                            }
                        }
                        else
                        {
                            throw new Exception("User not found, try again or enter 0 to exit");
                        }
                    }
                    else if (type == "Account")
                    {
                        if (input == 0)
                        {
                            throw new TimeoutException();
                        }
                        else if (db.Users.Any(e => e.UserId == input))
                        {
                            Console.WriteLine("Account #" + input + " Found");
                            Console.WriteLine("Are you sure you would like to delete?");
                            Console.WriteLine("Type YES to delete, or enter anything to return");
                            string response = Console.ReadLine();
                            if (response == "YES")
                            {
                                Account acc = db.Accounts.Single(e => e.AccId == input);
                                acc.AccName = null;
                                acc.AccBalance = null;
                                acc.AccType = null;
                                acc.AccBranch = null;
                                acc.AccIsActive = false;
                                db.Update(acc);
                                db.SaveChanges();
                                Console.WriteLine("Account Deleted");
                                break;
                            }
                            else
                            {
                                throw new TimeoutException();
                            }
                        }
                        else
                        {
                            throw new Exception("Account not found, try again or enter 0 to exit");
                        }

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
        }

        #endregion


        #region EditAccount

        public static void EditAccount(string type, User admin)
        {
            while (true)
            {
                try
                {
                    int input = Convert.ToInt32(Console.ReadLine());
                    User user = db.Users.Single(e => e.UserId == input);
                    if (type == "User")
                    {
                        if (input == 0)
                        {
                            throw new TimeoutException();

                        }
                        else if (db.Users.Any(e => e.UserId == input))
                        {
                            User userEdit = db.Users.Single(e => e.UserId == input);
                            Console.WriteLine("User #" + input + " Found");
                            Console.WriteLine("For each field, enter new value, or press enter to skip.");
                            Console.WriteLine("Alternatively enter 0 to cancel");
                            Console.WriteLine("Enter New Username: ");
                            string newUser = Console.ReadLine();
                            if (newUser == "0")
                            {
                                throw new TimeoutException();
                            }
                            else if (!newUser.IsNullOrEmpty())
                            {
                                userEdit.UserName = newUser;
                            }
                            Console.WriteLine("Enter New Password: ");
                            string newPass = PasswordHider.ReadInput();
                            if (newPass == "0")
                            {
                                throw new TimeoutException();
                            }
                            else if (!newPass.IsNullOrEmpty())
                            {
                                userEdit.UserPass = newPass;
                            }
                            db.Update(userEdit);
                            db.SaveChanges();
                            Console.WriteLine("User Edited");
                            break;
                        }
                        else
                        {
                            throw new Exception("User not found, try again or enter 0 to exit");
                        }
                    }
                    else if (type == "Account")
                    {
                        if (input == 0)
                        {
                            throw new TimeoutException();

                        }
                        else if (db.Accounts.Any(e => e.AccId == input))
                        {
                            Account accEdit = db.Accounts.Single(e => e.AccId == input);
                            Console.WriteLine("User #" + input + " Found");
                            Console.WriteLine("For each field, enter new value, or press enter to skip.");
                            Console.WriteLine("Alternatively enter 0 to cancel");
                            Console.WriteLine("Enter New Account Name: ");
                            string newName = Console.ReadLine();
                            if (newName == "0")
                            {
                                throw new TimeoutException();
                            }
                            else if (!newName.IsNullOrEmpty())
                            {
                                accEdit.AccName = newName;
                            }
                            Console.WriteLine("Enter New Branch: ");
                            string newBranch = Console.ReadLine();
                            if (newBranch == "0")
                            {
                                throw new TimeoutException();
                            }
                            else if (!newBranch.IsNullOrEmpty())
                            {
                                accEdit.AccBranch = newBranch;
                            }
                            Console.WriteLine("Enter New Balance: ");
                            double newBal = Convert.ToDouble(Console.ReadLine());
                            if (newBal >= 0)
                            {
                                accEdit.AccBalance = newBal;
                            }
                            Console.WriteLine("Enter New Type: ");
                            string newType = Console.ReadLine();
                            if (newType == "0")
                            {
                                throw new TimeoutException();
                            }
                            else if (!newType.IsNullOrEmpty())
                            {
                                accEdit.AccType = newType;
                            }
                            Console.WriteLine("Enter Active Status: (true/false)");
                            string newActive = Console.ReadLine();
                            if (newActive == "0")
                            {
                                throw new TimeoutException();
                            }
                            else if (!newActive.IsNullOrEmpty())
                            {
                                accEdit.AccIsActive = Convert.ToBoolean(newActive);;
                            }
                            db.Update(accEdit);
                            db.SaveChanges();
                            Console.WriteLine("Account Edited");
                            break;
                        }
                        else
                        {
                            throw new Exception("Account not found, try again or enter 0 to exit");
                        }

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
        }

        #endregion

        #region PrintSummary

        public static void PrintSummary()
        {
            var stats = new 
                {
                totalUsers = (from u in db.Users
                            select u).Count(),
                totalAccs = (from a in db.Accounts
                            select a).Count(),
                totalActive = (from a in db.Accounts
                            where a.AccIsActive == true
                            select a).Count(),
                totalInactive = (from a in db.Accounts
                            where a.AccIsActive != true
                            select a).Count(),
                totalTrans = (from t in db.TransactionHistories
                            select t).Count(),
                totalReqs = (from r in db.CheckRequests
                            select r).Count(),
                totalBalance = (from a in db.Accounts
                                select a.AccBalance).Sum(),
                avgBalance = (from a in db.Accounts
                                select a.AccBalance).Average(),
                };
            Console.WriteLine("========== SUMMARY ==========");
            Console.WriteLine("        Total Users : " + stats.totalUsers);
            Console.WriteLine("     Total Accounts : " + stats.totalAccs);
            Console.WriteLine("  Total Active Accs : " + stats.totalActive);
            Console.WriteLine("Total Inactive Accs : " + stats.totalInactive);
            Console.WriteLine(" Total Transactions : " + stats.totalTrans);
            Console.WriteLine(     "Total Requests : " + stats.totalReqs);
            Console.WriteLine("      Total Balance : " + Convert.ToDouble(stats.totalBalance).ToString("C"));
            Console.WriteLine("    Average Balance : " + Convert.ToDouble(stats.avgBalance).ToString("C"));
            Console.WriteLine("==============================");

        }

        #endregion

        #region ResetPass

        public static void ResetPass()
        {
            while (true)
            {
                try
                {
                    int input = Convert.ToInt32(Console.ReadLine());
                    if (input == 0)
                    {
                        throw new TimeoutException();
                    }
                    else if (db.Users.Any(e => e.UserId == input))
                    {
                        User user = db.Users.Single(e => e.UserId == input);
                        Console.WriteLine("User #" + input + " Found");
                        Console.WriteLine("Are you sure you would like to reset user password?");
                        Console.WriteLine("Type YES to delete, or enter anything to return");
                        string response = Console.ReadLine();
                        if (response == "YES")
                        {
                            user.UserPass = "password";
                            db.Update(user);
                            db.SaveChanges();
                            Console.WriteLine("User Password Reset");
                            break;
                        }
                        else
                        {
                            throw new TimeoutException();
                        }
                    }
                    else
                    {
                        throw new Exception("User not found, try again or enter 0 to exit");
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
        }

        #endregion

        #region ApproveReqs

        public static void ApproveReqs()
        {
            var outstanding = (from req in db.CheckRequests
                                where req.ReqAccepted != true
                                orderby req.ReqOpenDate ascending
                                select req).Take(5).ToList();
            Console.WriteLine("5 Oldest Outstanding Requests: ");
            for (int i=0;i<outstanding.Count;i++)
            {
                Console.WriteLine($"====== Request #{(i + 1)} ======");
                Console.WriteLine($"Request ID   : " + outstanding[i].ReqId);
                Console.WriteLine($"Request User : " + outstanding[i].UserId);
                Console.WriteLine($"Request Acc  : " + outstanding[i].AccId);
                Console.WriteLine($"Request Date : " + outstanding[i].ReqOpenDate);
                Console.WriteLine($"==========================");
                Console.WriteLine();
            }
            Console.WriteLine("Enter the request ID you would like to approve: ");

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "0")
                {
                    break;
                }
                if (db.CheckRequests.Any(e => e.ReqId.ToString() == input && e.ReqAccepted != true))
                {
                    CheckRequest reqAccept = db.CheckRequests.Single(e => e.ReqId.ToString() == input);
                    reqAccept.ReqRespondDate = DateTime.Now;
                    reqAccept.ReqAccepted = true;
                    db.Update(reqAccept);
                    db.SaveChanges();
                    Console.WriteLine($"Reqest ID {reqAccept.ReqId} has been approved!");
                    break;
                }
                else
                {
                    Console.WriteLine("Request not found or already approved. Try again or enter 0 to exit.");
                }
            }

        }

        #endregion

    }
}