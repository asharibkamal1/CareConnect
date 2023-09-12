using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PLIC_Web_Poratal.Models
{
    public class PremiumVoucher
    {
        public int iid { get; set; }
        public string vouchertype { get; set; }
        public string PolicyNumber { get; set; }
        public string FieldUnitName { get; set; }
        public string Origin { get; set; }
        public int RegionID { get; set; }
        public Int64 SerialNo { get; set; }
        public string LIR { get; set; }

        public string Name { get; set; }
        public string FHName { get; set; }
        public string PolicyTypeCode { get; set; }
        public string PolicyTerm { get; set; }
        public bool  IsPaid { get; set; }
        public bool _IsPaid{ get; set; }
        public bool  IsCancelled { get; set; }
        public bool IsDefence { get; set; }

        public string VoucherNo { get; set; }
        public string PremiumMode { get; set; }
        public Int64 SumAssured { get; set; }
        public int Age { get; set; }
        public int OriginalID { get; set; }
        public int SeqNo { get; set; }
        public Int64 BookNo { get; set; }
        public Int64 Amount { get; set; }
        public int CreatedUserID { get; set; }
        public string ReceiptNo { get; set; }
        public int CancelUserID { get; set; }
        public int ReceiptEnterUserID { get; set; }

        //[DataType(DataType.Date)]
        //public DateTime CreatedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]

        public DateTime RiskDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{yyyy/MM/dd}")]
        public DateTime ReceiptEnterDate { get; set; }


        public string CancelDate { get; set; }
        //public string FieldUnitName { get; set; }
    }
}
