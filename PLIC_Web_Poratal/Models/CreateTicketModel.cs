using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class CreateTicketModel
    {

        public string Name { get; set; }
        public string ticketsubcatagoryName { get; set; }
        public string infosubcatagoryName { get; set; }
        public string barcodecategoryname { get; set; }
        public string Contact { get; set; }
        public string Gender { get; set; }
        public string Remarks { get; set; }
        public int tickettype { get; set; }
        public int Opening_Type_Id { get; set; }
        public int ticketcatagory { get; set; }
        public string ticketcatagoryName { get; set; }
        public int ticketsubcatagory { get; set; }
        public int infosubcatagoryid { get; set; }
        public int region { get; set; }
        public int accountno { get; set; }
        public int agent { get; set; }
        public int agentref { get; set; }
        public int priority { get; set; }
        public string TicketPriorityName { get; set; }
        public int city { get; set; }
        public string locationname { get; set; }
        public int barcode { get; set; }
        public string gender { get; set; }
        public string cgnno { get; set; }
        public bool issendsms { get; set; }

        public DateTime BookingDate { get; set; }
        public string Payment_Mode { get; set; }
        public string sender { get; set; }
        public string sendercompany { get; set; }
        public string senderref { get; set; }
        public string senderphone { get; set; }
        public string senderaddress { get; set; }
        public string Product { get; set; }
        public string Origin { get; set; }
        public string Origin_Desc { get; set; }
        public string receiver { get; set; }
        public string receivercompany { get; set; }
        public string receiverphone { get; set; }
        public string receiveraddress { get; set; }
        public string Destination { get; set; }
        public string Destination_Desc { get; set; }
        public string Weight { get; set; }
       // public string BookingDate { get; set; }
        public string Quantity { get; set; }
        public string pieces { get; set; }

        public string ticketid { get; set; }
        public string datefrom { get; set; }
        public string dateto { get; set; }
        public string UserEmail { get; set; }
        public int terminal { get; set; }
        public int subterminal { get; set; }
        public bool isAGTChecked { get; set; }
        public bool isFRNChecked { get; set; }
     
        public string claimid { get; set; }
        public int customer_id {  get; set; }




    }
}
