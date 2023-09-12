using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class BookingDetail
    {

        public string booking_id { get; set; }
        public DateTime booking_datetime { get; set; }
        public string customer_group_name { get; set; }
        public string customer_name { get; set; }
        public string payment_mode_name { get; set; }
        public string STN { get; set; }
        public string SCCPN { get; set; }
        public string DTN { get; set; }
        public string DCCPN { get; set; }
        public string status_name { get; set; }
        public string amount_tax { get; set; }
        public string tax_rate { get; set; }
        public string amount_wr { get; set; }
        public string amount_cod { get; set; }
        public string fs_rate { get; set; }
        public string amount_fs { get; set; }
        public string backup_terminal_name { get; set; }
        public string tweight { get; set; }
        public string tpieces { get; set; }
        public string tqty { get; set; }
        public string sender_name { get; set; }
        public string sender_contact1 { get; set; }
        public string sender_address1 { get; set; }
        public string sender_cnic { get; set; }
        public string receiver_name { get; set; }
        public string receiver_contact1 { get; set; }
        public string receiver_address1 { get; set; }
        public string receiver_cnic { get; set; }
        public string AdduserID { get; set; }

        public string AdduserName { get; set; }
        public string CODMode { get; set; }
        public string ReimbursementStatus { get; set; }
        public string invoice_no { get; set; }
        public string HDAmount { get; set; }


        public List<customerDetails> customerDetails { get; set; }
        public List<senderDetails> senderDetails { get; set; }
        public List<ReceiverDetail> ReceiverDetail { get; set; }

        public List<Orderinformation> Orderinformation { get; set; }
        public List<itemdetail> itemdetail { get; set; }
        public List<BookingstatusDetail> BookingstatusDetail { get; set; }
        public List<HDTrackingDetail> HDTrackingDetail { get; set; }






    }

    public class customerDetails
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }

    }
    public class senderDetails
    {
        public string Person { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Origion { get; set; }
        public string Address { get; set; }


    }
    public class ReceiverDetail
    {
        public string Name { get; set; }
        public string Des { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

    }
    public class Orderinformation
    {
        public DateTime BkDate { get; set; }
        public string SType { get; set; }
        public string Description { get; set; }
        public string weight { get; set; }
        public string Quantity { get; set; }
        public string SrvCh { get; set; }
        public string COD { get; set; }
        public string Instruction { get; set; }

    }

    public class itemdetail
    {
        public string SRNo { get; set; }
        public string CNNo { get; set; }
        public string BarCode { get; set; }
        public string Category { get; set; }
        public string Item { get; set; }
        public string Packing { get; set; }
        public string Weight { get; set; }
        public string Qty { get; set; }
        public string Unit { get; set; }
        public string HDStatus { get; set; }
        public string CODCollectionStatus { get; set; }


    }
    public class BookingstatusDetail
    {
        public string BarCode { get; set; }
        public string CNNO { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string AddUser { get; set; }
    }
    public class HDTrackingDetail
    {
        public string BarCode { get; set; }
        public string CNNO { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Source { get; set; }
        public string AddUser { get; set; }
        public string CourierPh { get; set; }
        public string Remarks { get; set; }
        public string Reciver { get; set; }
        public string ReciverPh { get; set; }
        public string RecCnic { get; set; }
        public string SheetInfo { get; set; }
        public string Relation { get; set; }

    }

}
