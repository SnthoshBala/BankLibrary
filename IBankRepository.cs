

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLibrary
{
    public interface IBankRepository
    {
        string NewAccount(SBAccount acc);
        SBAccount GetAccountDetails(int accno);
        List<SBAccount> GetAllAccounts();
        string DepositAmount(int accno, decimal amt);
        string WithdrawAmount(int accno, decimal amt);
        List<SBTransaction> GetTransactions(int accno);
    }
}
