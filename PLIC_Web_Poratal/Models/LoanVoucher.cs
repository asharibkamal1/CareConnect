using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class LoanVoucher
    {
        public string voucherType { get; set; }
        public string voucherNumber { get; set; }
        public int seqNo { get; set; }
        public double principalLoanAmount { get; set; }
        public double interest { get; set; }
        public double paymentAmount { get; set; }
        public string dueDate { get; set; }
        public string name { get; set; }
        public string cnic { get; set; }
        public string loanAccountNo { get; set; }
        public int InstallmentNo { get; set; }
        public Boolean isPaid { get; set; }
        public string fieldUnitName { get; set; }
        public string origin { get; set; }
        public int bookNo { get; set; }
        public string policyNo { get; set; }
        public string RePaymentMonth { get; set; }
        public double bearingAmount { get; set; }
        public double monthlyInterest { get; set; }
        public double OutstandingBalance { get; set; }
        public double monthlyInterestNew { get; set; }
        public double monthWiseIntrerestNew { get; set; }
    }
}
