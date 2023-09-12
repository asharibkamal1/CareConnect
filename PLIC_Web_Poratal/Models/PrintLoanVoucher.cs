using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class PrintLoanVoucher
    {
        public string voucherNumber { get; set; }
        public double paymentAmount { get; set; }
        public string dueDate { get; set; }
        public string name { get; set; }
        public string cnic { get; set; }
        public string loanAccountNo { get; set; }
        public int InstallmentNo { get; set; }
        public string origin { get; set; }
        public int bookNo { get; set; }
        public string policyNo { get; set; }
        public string RePaymentMonth { get; set; }
        public double OutstandingBalance { get; set; }
        public string ABL_AccountNo { get; set; }
        public string UBL_AccountNo { get; set; }
        public string Is_ABLAccount { get; set; }
        public string Is_UBLAccount { get; set; }
    }
}
