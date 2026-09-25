using System.ComponentModel;
using System.Data;
using System.Runtime;
using System.Timers;
using System.Transactions;
using System.Xml.Serialization;
using bankLib.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols;

namespace bankLib
{
    public class AccountActions
    {
        #region Print Account Details Method

        public static void PrintDetails(Account account)
        {
            Console.WriteLine("Account Number   : " + account.AccId);
            Console.WriteLine("Account Owner ID : " + account.AccOwnerId);
            Console.WriteLine("Account Balance  : " + Convert.ToDouble(account.AccBalance).ToString("C"));
            Console.WriteLine("Account Branch   : " + account.AccBranch);
            Console.WriteLine("Account Type     : " + account.AccType);
            Console.WriteLine("Account Active   : " + account.AccIsActive);   
        }

        #endregion

        #region Withdraw Method

        public static double Withdraw(double amt, Account acc)
        {
            if (amt <= acc.AccBalance && amt > 0)
            {
                TransactionHistory newTrans = new TransactionHistory()
                                                    {AccId = acc.AccId,
                                                    UserId = acc.AccOwnerId,
                                                    TransDate = DateTime.Now,
                                                    TransOldBal = Convert.ToDouble(acc.AccBalance),
                                                    TransChange = -amt,
                                                    TransType = "Withdrawal"
                                                    };
                acc.AccBalance -= amt;
                UIComponents.db.Add(newTrans);
                UIComponents.db.Update(acc);
                UIComponents.db.SaveChanges();
                return Convert.ToDouble(acc.AccBalance);
            }
            else if (amt == 0)
            {
                return -1;
            }
            else if (amt > acc.AccBalance)
            {
                throw new WarningException("Insufficient balance to perform transaction");
            }
            else if (amt < 0)
            {
                throw new WarningException("Cannot withdraw negative balance");
            }
            else
            {
                throw new Exception();
            }
        }

        #endregion

        #region Deposit Method

        public static double Deposit(double amt, Account acc)
        {
            if (amt > 0)
            {
                TransactionHistory newTrans = new TransactionHistory()
                                                    {AccId = acc.AccId,
                                                    UserId = Convert.ToInt32(acc.AccOwnerId),
                                                    TransDate = DateTime.Now,
                                                    TransOldBal = Convert.ToDouble(acc.AccBalance),
                                                    TransType = "Deposit"
                                                    };
                if (acc.AccType == "Loan")
                {
                    if (amt <= acc.AccBalance)
                    {
                        acc.AccBalance -= amt;
                        newTrans.TransChange = -amt;   
                    }
                    else
                    {
                        Console.WriteLine("Deposit too large, please enter a valid amount up to " + acc.AccBalance);
                    }

                }
                else
                {
                    acc.AccBalance += amt;
                    newTrans.TransChange = amt;
                }
                UIComponents.db.Add(newTrans);
                UIComponents.db.Update(acc);
                UIComponents.db.SaveChanges();
                return Convert.ToDouble(acc.AccBalance);
            }
            else if (amt == 0)
            {
                return -1;
            }
            else if (amt < 0)
            {
                throw new WarningException("Cannot deposit negative balance");
            }
            else
            {
                throw new Exception();
            }
        }
        
        #endregion

        #region Transfer Method

        public static double Transfer(double amt, Account accSender, Account accReceiver)
        {
            if (amt > 0 && amt <= accSender.AccBalance)
            {
                TransactionHistory newTrans = new TransactionHistory()
                                                    {
                                                    AccId = accSender.AccId,
                                                    UserId = Convert.ToInt32(accSender.AccOwnerId),
                                                    TransDate = DateTime.Now,
                                                    TransOldBal = Convert.ToDouble(accSender.AccBalance),
                                                    TransType = "Transfer",
                                                    TransChange = -amt
                                                    };
                TransactionHistory receiverTrans = new TransactionHistory()
                                                    {
                                                    AccId = accReceiver.AccId,
                                                    UserId = Convert.ToInt32(accReceiver.AccOwnerId),
                                                    TransDate = DateTime.Now,
                                                    TransOldBal = Convert.ToDouble(accReceiver.AccBalance),
                                                    TransType = "Transfer",
                                                    TransChange = amt
                                                    };
                accSender.AccBalance -= amt;
                accReceiver.AccBalance += amt;
                UIComponents.db.Add(newTrans);
                UIComponents.db.Add(receiverTrans);
                UIComponents.db.Update(accSender);
                UIComponents.db.Update(accReceiver);
                UIComponents.db.SaveChanges();
                return Convert.ToDouble(accSender.AccBalance);
            }
            else if (amt == 0)
            {
                return -1;
            }
            else if (amt < 0)
            {
                throw new WarningException("Cannot transfer negative balance");
            }
            else
            {
                throw new Exception();
            }
        }

        #endregion

        #region Request Check Book Method

        public static void RequestCheck(Account acc)
        {
            if (UIComponents.db.CheckRequests.Any(e => e.AccId == acc.AccId))
            {
                var request = UIComponents.db.CheckRequests.Where(e => e.AccId == acc.AccId)
                                                        .OrderByDescending(e => e.ReqId)
                                                        .Take(1)
                                                        .Single();
                if (request.ReqAccepted == true)
                {
                    CheckRequest newReq = new CheckRequest() 
                                    {
                                      AccId = acc.AccId,
                                      UserId = acc.AccOwnerId,
                                      ReqOpenDate = DateTime.Now
                                    };
                    UIComponents.db.Add(newReq);
                    UIComponents.db.SaveChanges();
                    Console.WriteLine("Check book request has been submitted!");
                }
                else
                {
                    Console.WriteLine("Previous request still pending");
                }
            }
            else
            {
                CheckRequest newReq = new CheckRequest() 
                                {
                                  AccId = acc.AccId,
                                  UserId = acc.AccOwnerId,
                                  ReqOpenDate = DateTime.Now
                                };
                UIComponents.db.Add(newReq);
                UIComponents.db.SaveChanges();
                Console.WriteLine("Check book request has been submitted!");
            }
        }

        #endregion

    }
}