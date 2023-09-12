using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PLIC_Web_Poratal.Models
{
    public class PrintPremiumVoucher
    {
        public string PolicyNo { get; set; }
        public string Name { get; set; }
        public string CNIC { get; set; }
        public string PremiumMode { get; set; }
        public Int32 BookNo { get; set; }
        public string VoucherNo { get; set; }
        public double Amount { get; set; }
        public string PolicyYear { get; set; }
        public string ABL_AccountNo { get; set; }
        public string UBL_AccountNo { get; set; }
        public string Is_ABLAccount { get; set; }
        public string Is_UBLAccount { get; set; }
        public string P_YearType { get; set; }
        public string RegionName { get; set; }
        public double? Rebate { get; set; }
        public double? AmountWithinDueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate{ get; set; }
    }
}
