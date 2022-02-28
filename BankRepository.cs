

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace BankLibrary
{
    public class NoValue:ApplicationException
    {
        public NoValue(string message):base(message)
        { 
        }
    }
    public class InsufficientBalance : ApplicationException
    {
        public InsufficientBalance(string message) : base(message)
        {
        }
    }

    public class BankRepository:IBankRepository
    {
        public static int count=0;
        List<SBAccount> sBs = new List<SBAccount>();
        List<SBTransaction> sBt = new List<SBTransaction>();
        public SqlConnection con = new SqlConnection();
        public SqlCommand cmd = new SqlCommand();
        public SqlDataReader dr;
        private SqlConnection GetConnection()
        {
            con = new SqlConnection("Data Source=KANINI-LTP-487\\MSQLSERVERSB007;Initial Catalog=bank;User ID=sa;Password=Raguraman@007");
            con.Open();
            return con;
        }
       public string NewAccount(SBAccount acc)
        {
            //sBs.Add(acc);
            try
            {
                con = GetConnection();
                cmd = new SqlCommand("insert into sbaccount values(@accno,@custname,@custaddress,@balance,@transtype)", con);
                cmd.Parameters.AddWithValue("accno", acc.AccountNumber);
                cmd.Parameters.AddWithValue("custname", acc.CustomerName);
                cmd.Parameters.AddWithValue("custaddress", acc.CustomerAddress);
                cmd.Parameters.AddWithValue("balance", acc.CurrentBalance);
                cmd.Parameters.AddWithValue("transtype", acc.TransactionType);
                cmd.ExecuteNonQuery();
                return "Your account was added sucessfully";
            }
            catch(SqlException s)
            {
                throw s;
            }
        }
       public SBAccount GetAccountDetails(int accno)
        {
            try
            {
                #region
                //if (sBs.Find(a => a.AccountNumber == accno) == null)
                //{
                //    throw new NoValue("The value is not in the list");
                //}
                //else
                //{
                //    //return sBs.Find(a => a.AccountNumber == accno);
                //    foreach (var item in sBs)
                //    {
                //        if(item.AccountNumber==accno)
                //        {
                //            return item;

                //        }
                //    }
                //    return null;
                //    }

                #endregion
                con = GetConnection();
                cmd = new SqlCommand("select * from sbaccount",con);
                dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    for(int i=0;i<dr.FieldCount;i++)
                    {
                        if (Convert.ToString(dr[i])==Convert.ToString(accno))
                        {
                            dr.Close();
                            SqlCommand newcmd = new SqlCommand("select * from sbaccount where accno=@no",con);
                            newcmd.Parameters.AddWithValue("no",accno);
                            SqlDataReader single = newcmd.ExecuteReader();
                            SBAccount sa = new SBAccount();
                            while (single.Read())
                            {
                                    sa.AccountNumber = Convert.ToInt32(single[0]);
                                    sa.CustomerName = Convert.ToString(single[1]);
                                    sa.CustomerAddress = Convert.ToString(single[2]);
                                    sa.CurrentBalance = float.Parse(single[3].ToString());
                                    sa.TransactionType = Convert.ToString(single[4]);
                            }
                            single.Close();
                            return sa;
                        }
                    }
                }
                return null;
            }
            catch (SqlException s)
            {
                throw s;
            }
            catch (NoValue n)
            {
                Console.WriteLine(n.Message);
                return null;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public List<SBAccount> GetAllAccounts()
        {
            con = GetConnection();
            cmd = new SqlCommand("select * from sbaccount",con);
            dr=cmd.ExecuteReader();
            while (dr.Read())
            {
                SBAccount sb = new SBAccount();
                sb.AccountNumber = Convert.ToInt32(dr[0]);
                    sb.CustomerName = Convert.ToString(dr[1]);
                    sb.CustomerAddress = Convert.ToString(dr[2]);
                    sb.CurrentBalance = float.Parse(dr[3].ToString());
                    sb.TransactionType = Convert.ToString(dr[4]);
                    sBs.Add(sb);
            }
            dr.Close();
            return sBs;
        }
        public List<SBAccount> GetAllDisconnectedAccounts()
        {
            con = GetConnection();
            cmd = new SqlCommand("select * from sbaccount");
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                SBAccount sb = new SBAccount();
                sb.AccountNumber = Convert.ToInt32(dr[0]);
                sb.CustomerName = Convert.ToString(dr[1]);
                sb.CustomerAddress = Convert.ToString(dr[2]);
                sb.CurrentBalance = float.Parse(dr[3].ToString());
                sb.TransactionType = Convert.ToString(dr[4]);
                sBs.Add(sb);
            }
            return sBs;
        }
            public string DepositAmount(int accno, decimal amt)
        {
            #region
            //SBAccount s= sBs.Find(a => a.AccountNumber == accno);
            //s.CurrentBalance = s.CurrentBalance+float.Parse(amt.ToString());
            //SBTransaction t = sBt.Find(a => a.AccountNumber == accno);
            //t.TransactionId = count + 1;
            //t.TransactionDate = DateTime.Now;
            //t.TransactionType = "Savings";
            //t.AccountNumber = accno;
            //t.Amount = float.Parse(amt.ToString());
            //sBt.Add(t);
            //return $"Your Deposit amount of {amt} has been to {accno}";
            #endregion
            try
            {
                con = GetConnection();
                cmd = new SqlCommand("select * from sbaccount where accno=@accno", con);
                cmd.Parameters.AddWithValue("accno", accno);
                dr = cmd.ExecuteReader();
                SBAccount sb = new SBAccount();
                while (dr.Read())
                {
                    sb.CurrentBalance = float.Parse(dr[3].ToString());
                }
                dr.Close();
                float updateammt = sb.CurrentBalance + float.Parse(amt.ToString());
                SqlCommand newcmmd = new SqlCommand("update sbaccount set balance=@updateammt where accno=@accno", con);
                newcmmd.Parameters.AddWithValue("updateammt", updateammt);
                newcmmd.Parameters.AddWithValue("accno", accno);
                newcmmd.ExecuteNonQuery();
                SqlCommand transcmd = new SqlCommand("insert into sbtransaction(transdate,accno,amt,transtype) values(@date,@accno,@amt,@type)", con);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                transcmd.Parameters.AddWithValue("date", date);
                transcmd.Parameters.AddWithValue("accno", accno);
                transcmd.Parameters.AddWithValue("amt", amt);
                transcmd.Parameters.AddWithValue("type", "Deposit");
                transcmd.ExecuteNonQuery();
                return $"Your Deposit amount of {amt} has been to {accno}\nYour current balance is {updateammt}";
            }
            catch(SqlException s)
            {
                return s.Message;
            }
        }
        public string WithdrawAmount(int accno, decimal amt)
        {
            #region
            //try
            //{
            //    SBAccount s = sBs.Find(a => a.AccountNumber == accno);
            //    if (s.CurrentBalance < float.Parse(amt.ToString()))
            //    {
            //        throw new InsufficientBalance($"Your Account Balance is less than {amt}");
            //    }
            //    else
            //    {
            //        s.CurrentBalance = s.CurrentBalance - float.Parse(amt.ToString());
            //        SBTransaction t = sBt.Find(a => a.AccountNumber == accno);
            //        t.TransactionId = count + 1;
            //        t.TransactionDate = DateTime.Now;
            //        t.TransactionType = "Savings";
            //        t.AccountNumber = accno;
            //        t.Amount = float.Parse(amt.ToString());
            //        sBt.Add(t);
            //        return $"Withdrawal Amount of {amt} has been successful";
            //    }
            //}
            //catch(InsufficientBalance i)
            //{
            //    return (i.Message);
            //}
            //catch(Exception e)
            //{
            //    return (e.Message);
            //}
            #endregion
            try
            {
                con = GetConnection();
                cmd = new SqlCommand("select * from sbaccount where accno=@accno", con);
                cmd.Parameters.AddWithValue("accno", accno);
                dr = cmd.ExecuteReader();
                SBAccount sb = new SBAccount();
                while (dr.Read())
                {
                    sb.CurrentBalance = float.Parse(dr[3].ToString());
                }
                dr.Close();
                if (sb.CurrentBalance < float.Parse(amt.ToString()))
                {
                    throw new InsufficientBalance($"Your Account Balance is less than {amt}");
                }
                else
                {
                    float updateammt = sb.CurrentBalance - float.Parse(amt.ToString());
                    SqlCommand newcmmd = new SqlCommand("update sbaccount set balance=@updateammt where accno=@accno", con);
                    newcmmd.Parameters.AddWithValue("updateammt", updateammt);
                    newcmmd.Parameters.AddWithValue("accno", accno);
                    newcmmd.ExecuteNonQuery();
                    SqlCommand transcmd = new SqlCommand("insert into sbtransaction(transdate,accno,amt,transtype) values(@date,@accno,@amt,@type)",con);
                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    transcmd.Parameters.AddWithValue("date",date);
                    transcmd.Parameters.AddWithValue("accno", accno);
                    transcmd.Parameters.AddWithValue("amt", amt);
                    transcmd.Parameters.AddWithValue("type", "Withdraw");
                    transcmd.ExecuteNonQuery();
                    return $"Withdrawal Amount of {amt} has been successful\nYour current balance is {updateammt}";
                }
            }
            catch (SqlException s)
            {
                return s.Message;
            }
            catch(InsufficientBalance i)
            {
                return i.Message;
            }
        }
        public List<SBTransaction> GetTransactions(int accno)
        {
            //return sBt.FindAll(a=>a.AccountNumber==accno);
            con = GetConnection();
            cmd = new SqlCommand("select * from sbtransaction where accno=@accno",con);
            cmd.Parameters.AddWithValue("accno", accno);
            dr=cmd.ExecuteReader();
            while(dr.Read())
            {
                SBTransaction st = new SBTransaction();
                st.TransactionId = Convert.ToInt32(dr[0]);
                st.TransactionDate = Convert.ToDateTime(dr[1]);
                st.AccountNumber = Convert.ToInt32(dr[2]);
                st.Amount = float.Parse(dr[3].ToString());
                st.TransactionType = Convert.ToString(dr[4]);
                sBt.Add(st);
                for (int i=0;i<dr.FieldCount;i++)
                {
                    string var = Convert.ToString(dr[i]+" ");
                }
            }
            return sBt;
        }
    }
}
