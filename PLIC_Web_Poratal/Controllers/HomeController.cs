using CareConnect.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using PLIC_Web_Poratal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Controllers
{
    public class HomeController : Controller
    {

        //private readonly ILogger<HomeController> _logger;

        db _db = new db();
        string sqlquery = "select top 1 LoginId,Password from CRM_SYS_Users where LoginId=@LoginId and password=@Password";

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public ActionResult PrintPremiumVoucher(string VoucherNo, string iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                List<PrintPremiumVoucher> voucherdetail = _db.GetVoucherDetail(VoucherNo, iid).ToList();
                //PrintPremiumVoucher voucherdetail = _db.GetVoucherDetail(VoucherNo,iid);
                return View("~/Views/Home/PrintPremium.cshtml", voucherdetail);
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }

        public ActionResult PrintLoanVoucher(string VoucherNo, string iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                PrintLoanVoucher voucherDetail = new PrintLoanVoucher();
                voucherDetail = _db.GetLoanVoucherDetail(VoucherNo, iid);
                return View("~/Views/Home/PrintLoanVoucher.cshtml", voucherDetail);
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }


        public ActionResult PrintfirstPremiumVoucher(string VoucherNo, string iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                List<PrintPremiumVoucher> voucherdetail = _db.GetVoucherDetail(VoucherNo, iid).ToList();
                //PrintPremiumVoucher voucherdetail = _db.GetVoucherDetail(VoucherNo,iid);
                return View("~/Views/Home/PrintfirstPremium.cshtml", voucherdetail);
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }

        //Changes Done by Aizaz & Imran  Dated(2/16/2022)
        public ActionResult Index(string policy, int iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                // changed by Aizaz and Imran Dated 17 Feb 22
                List<Policy> policies = new List<Policy>();
                List<PremiumVoucher> voucherlist = new List<PremiumVoucher>();

                HttpContext.Session.SetInt32("iid", iid);
                voucherlist = _db.GetAllVouchers(policy).ToList();
                //if(voucherlist.Count==0)
                //{
                //    return View("~/Views/Home/Policy.cshtml", policies);
                //}
                ViewBag.message = "Voucher not found against policy no: ";
                ViewBag.pol = policy;
                return View(voucherlist);
            }
            return RedirectToAction("Login", "Account");
            //return View("~/Views/Account/Login.cshtml");
        }


        public ActionResult PostLoginComplaint(int iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                PostLoginComplaint postLoginComplaint = new PostLoginComplaint();
                postLoginComplaint = _db.GetPostComplaintsData(iid);
                postLoginComplaint.mobileNumber = HttpContext.Session.GetString("Mobile");
                return View(postLoginComplaint);
            }
            return RedirectToAction("Login", "Account");
            //return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public IActionResult InsertPostLoginComplaint(PostLoginComplaint postLoginComplaint)
        {
            try
            {
                //postLoginComplaint.complaintOrigin = "From Portal";
                if (ModelState.IsValid)
                {

                    postLoginComplaint.cnicNo = HttpContext.Session.GetString("CNIC");

                    string resp = _db.insertPostLoginComplaint(postLoginComplaint);

                    string fileName = null;
                    if (postLoginComplaint.documents != null && postLoginComplaint.documents.Count > 0)
                    {
                        foreach (var file in postLoginComplaint.documents)
                        {
                            string path = Path.Combine(Directory.GetCurrentDirectory(), "C://Resource/WebPortalDocuments");
                            fileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            string fileNameWithPath = Path.Combine(path, fileName);

                            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            _db.InsertUploadDocumnets(resp, fileName);
                        }
                    }

                    string complaintType = postLoginComplaint.complaintType;
                    string phone = postLoginComplaint.mobileNumber;
                    string mobilenumber = "92" + phone.Substring(phone.Length - 10);
                    //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "معزز کسٹمر%0aآپ کی شکایت موصول ہو چکی ہے۔%0aآپ کی شکایت نمبر " + resp + " ہے.%0aپوسٹل لائف انشورنس کمپنی");
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PCLSDKJPISWUWDHDPLIC&password=b63b122c528f54df4a0446b6bab05515&sender=8576&apikey=1094cbd4841bb89b173098af5bed0e60&type=Urdu&message=MESSAGE&phone=" + mobilenumber + "&message=" + "معزز کسٹمر%0aآپ کی شکایت موصول ہو چکی ہے۔%0aآپ کی شکایت نمبر " + resp + " ہے.%0aپوسٹل لائف انشورنس کمپنی");

                    request.Method = "GET";

                    WebResponse response = request.GetResponse();
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var ApiStatus = reader.ReadToEnd();
                        if (ApiStatus == "Your Message is sent, MSISDN 1, Cost 2")
                        {
                            _db.SmsLog(phone, 2, "Complaint registration message");
                        }
                    }
                    ViewBag.message = "OK";
                    ModelState.Clear();
                    return View("~/Views/Home/PostLoginComplaint.cshtml");
                }
                return View("~/Views/Home/PostLoginComplaint.cshtml");
            }
            catch (Exception)
            {

                throw;
            }

        }



        public ActionResult FirstPremium(string policy, int iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                // changed by Aizaz and Imran Dated 17 Feb 22
                List<Policy> policies = new List<Policy>();
                List<PremiumVoucher> voucherlist = new List<PremiumVoucher>();

                HttpContext.Session.SetInt32("iid", iid);
                voucherlist = _db.GetAllFirstPremiumVouchers(policy).ToList();
                ViewBag.message = "Voucher not found against policy no: ";
                ViewBag.pol = policy;
                return View(voucherlist);
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }
        public ActionResult LatePayment(string policy, int iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                // changed by Aizaz and Imran Dated 17 Feb 22
                List<Policy> policies = new List<Policy>();

                List<PremiumVoucher> voucherlist = new List<PremiumVoucher>();

                HttpContext.Session.SetInt32("iid", iid);
                voucherlist = _db.GetAllLatePaymentVouchers(policy).ToList();
                ViewBag.sucess = "SS";
                ViewBag.message = "Voucher not found against policy no: ";
                ViewBag.pol = policy;
                return View(voucherlist);
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }



        public ActionResult ShortPayment(string policy, int iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                List<Policy> policies = new List<Policy>();
                List<PremiumVoucher> voucherlist = new List<PremiumVoucher>();

                HttpContext.Session.SetInt32("iid", iid);
                voucherlist = _db.GetAllShortPaymentVouchers(policy).ToList();
                ViewBag.sucess = "SS";
                ViewBag.message = "Voucher not found against policy no: ";
                ViewBag.pol = policy;
                return View(voucherlist);

            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");

            // changed by Aizaz and Imran Dated 17 Feb 22
        }

        public ActionResult LoanVoucher(string policy, int iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                List<Policy> policies = new List<Policy>();
                List<LoanVoucher> loanVoucherlist = new List<LoanVoucher>();

                HttpContext.Session.SetInt32("iid", iid);
                loanVoucherlist = _db.GetLoanVouchers(iid).ToList();
                ViewBag.sucess = "SS";
                ViewBag.message = "Loan Voucher not found against policy no: ";
                ViewBag.pol = policy;
                return View(loanVoucherlist);

            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");

            // changed by Aizaz and Imran Dated 17 Feb 22
        }

        public ActionResult Information(int iid)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {

                ViewModel viewObj = new ViewModel();
                viewObj = _db.GetInformation(iid);
                return View(viewObj);
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }



        public IActionResult Menu()
        {

            //string password = HttpContext.Session.GetString("Password");
            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));

            if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
            {
                var viewModel = TempData["UserViewModel"] as UserViewModel;
                // Check if the viewModel is not null before using it
                if (viewModel != null)
                {
                    // Access properties of viewModel
                    string loginId = viewModel.LoginId;
                    string userName = viewModel.UserName;
                    string password = viewModel.Password;
                    string roleId = viewModel.RoleID;

                    // Rest of your code using the viewModel...
                }

                string RoleID = HttpContext.Session.GetString("RoleID");
                string UserName = HttpContext.Session.GetString("UserName");

                ViewData["RoleID"] = RoleID;


                ViewBag.UserName = UserName;
                // ViewBag.RoleID = RoleID;

                conn.Close();



                return View("~/Views/Home/LoginPage.cshtml", viewModel);


            }


            return RedirectToAction("Login", "Account");

        }


        [HttpGet]
        public IActionResult GetChartData()
        {
            try
            {

                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    //DataSet dataSet = new DataSet();
                    //DataSet dt2 = new DataSet();
                    //DataSet dt3 = new DataSet();
                    SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();


                        using (SqlCommand command = new SqlCommand("sp_careconnect_get_dashboard", conn))

                        {
                            command.CommandType = CommandType.StoredProcedure;

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                // Create separate lists for the two charts
                                var chartDataAll = new List<ChartData>();
                                var chartDatComplaint = new List<ChartData>();
                                var chartDataInfo = new List<ChartData>();
                                var chartDataServiceRequest = new List<ChartData>();
                                var chartDataregion = new List<ChartData>();
                                var chartDatatickettype = new List<ChartData>();
                                var chartDatatotalcount = new List<ChartData>();
                                var chartDatainprogress = new List<ChartData>();
                                var chartDataproducts = new List<ChartData>();


                                // Assuming the second query returns columns named TotalTicket, CategoryDescription, and IssueTypeDescription as well
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var categoryDescription = reader["CategoryDescription"].ToString();


                                    chartDataAll.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        CategoryDescription = categoryDescription,

                                    });
                                }
                                // Move to the next result set for the second query
                                reader.NextResult();
                                // Assuming the second query returns columns named TotalTicket, CategoryDescription, and IssueTypeDescription as well
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var IssueTypeDescription = reader["IssueTypeDescription"].ToString();


                                    chartDatComplaint.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        IssueTypeDescription = IssueTypeDescription,

                                    });
                                }
                                // Move to the next result set for the second query
                                reader.NextResult();
                                // Assuming the second query returns columns named TotalTicket, CategoryDescription, and IssueTypeDescription as well
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var IssueTypeDescription = reader["IssueTypeDescription"].ToString();


                                    chartDataInfo.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        IssueTypeDescription = IssueTypeDescription,

                                    });
                                }
                                // Move to the next result set for the second query
                                reader.NextResult();
                                // Assuming the second query returns columns named TotalTicket, CategoryDescription, and IssueTypeDescription as well
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var IssueTypeDescription = reader["IssueTypeDescription"].ToString();


                                    chartDataServiceRequest.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        IssueTypeDescription = IssueTypeDescription,

                                    });
                                }
                                reader.NextResult();



                                while (reader.Read())
                                {
                                    var region_name = reader["CategoryDescription"].ToString();
                                    var Percentage = reader["Percentage"].ToString();


                                    chartDataregion.Add(new ChartData
                                    {
                                        region_name = region_name,
                                        Percentage = Percentage,

                                    });
                                }


                                reader.NextResult();

                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var Ticket_Type = reader["Ticket_Type"].ToString();


                                    chartDatatickettype.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        TicketType = Ticket_Type,

                                    });
                                }

                                reader.NextResult();
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalCount"];
                                    // var Ticket_Type = reader["Ticket_Type"].ToString();


                                    chartDatatotalcount.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        //TicketType = Ticket_Type,

                                    });
                                }



                                reader.NextResult();
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["InProgress"];
                                    //var Ticket_Type = reader["Ticket_Type"].ToString();


                                    chartDatainprogress.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        // TicketType = Ticket_Type,

                                    });
                                }




                                reader.NextResult();
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var Products = reader["Product"].ToString();


                                    chartDataproducts.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        Products = Products,

                                    });
                                }

                                var jsonData = new
                                {
                                    chartDataAll = chartDataAll,
                                    chartDatComplaint = chartDatComplaint,
                                    chartDataInfo = chartDataInfo,
                                    chartDataServiceRequest = chartDataServiceRequest,
                                    chartDataregion = chartDataregion,
                                    chartDatatickettype = chartDatatickettype,
                                    chartDatatotalcount = chartDatatotalcount,
                                    chartDatainprogress = chartDatainprogress,
                                    chartDataproducts = chartDataproducts
                                };

                                return Json(jsonData);
                            }
                        }
                    }
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { errorMessage = ex.Message });


            }
        }


        [HttpGet]
        public IActionResult GetChartData2(DateTime datefrom, DateTime dateto)
        {
            try
            {

                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    //DataSet dataSet = new DataSet();
                    //DataSet dt2 = new DataSet();
                    //DataSet dt3 = new DataSet();
                    SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();


                        using (SqlCommand command = new SqlCommand("sp_careconnect_get_dashboard_New", conn))

                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@datefrom", datefrom);
                            command.Parameters.AddWithValue("@dateto", dateto);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                // Create separate lists for the two charts
                                var chartDataAll = new List<ChartData>();
                                var chartDatComplaint = new List<ChartData>();
                                var chartDataInfo = new List<ChartData>();
                                var chartDataServiceRequest = new List<ChartData>();
                                var chartDataregion = new List<ChartData>();
                                var chartDatatickettype = new List<ChartData>();
                                var chartDatatotalcount = new List<ChartData>();
                                var chartDatainprogress = new List<ChartData>();
                                var chartDataproducts = new List<ChartData>();
                                var chartDataopsclosed = new List<ChartData>();
                                var chartDataclosed = new List<ChartData>();


                                // Assuming the second query returns columns named TotalTicket, CategoryDescription, and IssueTypeDescription as well
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var categoryDescription = reader["CategoryDescription"].ToString();


                                    chartDataAll.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        CategoryDescription = categoryDescription,

                                    });
                                }
                                // Move to the next result set for the second query
                                reader.NextResult();
                                // Assuming the second query returns columns named TotalTicket, CategoryDescription, and IssueTypeDescription as well
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var IssueTypeDescription = reader["IssueTypeDescription"].ToString();


                                    chartDatComplaint.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        IssueTypeDescription = IssueTypeDescription,

                                    });
                                }
                                // Move to the next result set for the second query
                                reader.NextResult();
                                // Assuming the second query returns columns named TotalTicket, CategoryDescription, and IssueTypeDescription as well
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var IssueTypeDescription = reader["IssueTypeDescription"].ToString();


                                    chartDataInfo.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        IssueTypeDescription = IssueTypeDescription,

                                    });
                                }
                                // Move to the next result set for the second query
                                reader.NextResult();
                                // Assuming the second query returns columns named TotalTicket, CategoryDescription, and IssueTypeDescription as well
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var IssueTypeDescription = reader["IssueTypeDescription"].ToString();


                                    chartDataServiceRequest.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        IssueTypeDescription = IssueTypeDescription,

                                    });
                                }
                                reader.NextResult();



                                while (reader.Read())
                                {
                                    var region_name = reader["CategoryDescription"].ToString();
                                    var Percentage = reader["Percentage"].ToString();


                                    chartDataregion.Add(new ChartData
                                    {
                                        region_name = region_name,
                                        Percentage = Percentage,

                                    });
                                }


                                reader.NextResult();

                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var Ticket_Type = reader["Ticket_Type"].ToString();


                                    chartDatatickettype.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        TicketType = Ticket_Type,

                                    });
                                }

                                reader.NextResult();
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalCount"];
                                    // var Ticket_Type = reader["Ticket_Type"].ToString();


                                    chartDatatotalcount.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        //TicketType = Ticket_Type,

                                    });
                                }



                                reader.NextResult();
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["InProgress"];
                                    //var Ticket_Type = reader["Ticket_Type"].ToString();


                                    chartDatainprogress.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        // TicketType = Ticket_Type,

                                    });
                                }




                                reader.NextResult();
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["TotalTicket"];
                                    var Products = reader["Product"].ToString();


                                    chartDataproducts.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        Products = Products,

                                    });
                                }

                                reader.NextResult();
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["OPSClosedTotalTicket"];
                                    //var Ticket_Type = reader["Ticket_Type"].ToString();


                                    chartDataopsclosed.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        // TicketType = Ticket_Type,

                                    });
                                }
                                reader.NextResult();
                                while (reader.Read())
                                {
                                    var TotalTicket = (int)reader["ClosedTotalTicket"];
                                    //var Ticket_Type = reader["Ticket_Type"].ToString();


                                    chartDataclosed.Add(new ChartData
                                    {
                                        TotalTicket = TotalTicket,
                                        // TicketType = Ticket_Type,

                                    });
                                }

                                var jsonData = new
                                {
                                    chartDataAll = chartDataAll,
                                    chartDatComplaint = chartDatComplaint,
                                    chartDataInfo = chartDataInfo,
                                    chartDataServiceRequest = chartDataServiceRequest,
                                    chartDataregion = chartDataregion,
                                    chartDatatickettype = chartDatatickettype,
                                    chartDatatotalcount = chartDatatotalcount,
                                    chartDatainprogress = chartDatainprogress,
                                    chartDataproducts = chartDataproducts,
                                    chartDataopsclosed = chartDataopsclosed,
                                    chartDataclosed = chartDataclosed
                                };

                                return Json(jsonData);
                            }
                        }
                    }
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { errorMessage = ex.Message });


            }
        }


        [HttpGet]
        public ActionResult GetTrackingDetails(string tracking,bool isCheckboxChecked)
        {

            try
            {
                string consignmentNumber = "";
                DataSet dataSet = new DataSet();
                DataSet dataSet1 = new DataSet();
                DataSet dataSet2 = new DataSet();
                SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));
                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
                {
                    conn.Open();
                    conn1.Open();





                    string LoginId = HttpContext.Session.GetString("LoginId");
                    string RoleID = HttpContext.Session.GetString("RoleID");
                    string trackingid = tracking;



                    if (isCheckboxChecked)

                    {
                        

                        if (conn1.State != ConnectionState.Open)
                            conn1.Open();

                        SqlCommand command3 = new SqlCommand("sp_careconnect_Get_ConsignmentNo_By_TicketID", conn1);
                        command3.CommandType = CommandType.StoredProcedure;
                        command3.Parameters.AddWithValue("@TicketID", tracking);

                        using (SqlDataReader reader = command3.ExecuteReader())
                        {
                            // Check if there are rows and retrieve the consignment number
                            if (reader.Read())
                            {
                                // Replace "ColumnName" with the actual column name that holds the consignment number
                                consignmentNumber = reader["ConsignmentNo"].ToString();

                                
                            }
                        }

                    }

                    //string Password = HttpContext.Session.GetString("Password");
                    InsertTrackingHistory(LoginId, trackingid);





                    SqlCommand command = new SqlCommand("sp_careconnect_Get_Tracking", conn);
                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Track_History_By_CNSGNO", conn1);
                    SqlCommand command2 = new SqlCommand("sp_GetTicketHistoryBYCNSGNO", conn1);
                    command.CommandType = CommandType.StoredProcedure;
                    command1.CommandType = CommandType.StoredProcedure;
                    command2.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);
                    SqlDataAdapter dataAdapter2 = new SqlDataAdapter(command2);

                    if (isCheckboxChecked)
                    {
                        command.Parameters.AddWithValue("@bookingNumber", consignmentNumber);
                        command1.Parameters.AddWithValue("@CNSGNO", consignmentNumber);
                        command2.Parameters.AddWithValue("@CNSGNO", consignmentNumber);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@bookingNumber", tracking);
                        command1.Parameters.AddWithValue("@CNSGNO", tracking);
                        command2.Parameters.AddWithValue("@CNSGNO", tracking);
                    }
              
                
                    dataAdapter.Fill(dataSet);
                    dataAdapter1.Fill(dataSet1);
                    dataAdapter2.Fill(dataSet2);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        BookingDetail = dataSet,
                        TrackingHistory = dataSet1,
                        TicketDetails = dataSet2

                    };

                    ViewBag.TrackingData = dataSet;

                    ViewBag.RoleId = HttpContext.Session.GetString("RoleID");
                    return PartialView("_TrackingDetails", model);
                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult GetTicketDetails(string ticketno)
        {

            try
            {

                DataSet dataSet1 = new DataSet();
                DataSet dataSet2 = new DataSet();


                SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));
                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
                {
                    DataSet dataSet5 = new DataSet();
                    conn.Open();
                    conn1.Open();

                    string LoginId = HttpContext.Session.GetString("LoginId");
                    string ticketid = ticketno;
                    SqlCommand command3 = new SqlCommand("SP_Get_ConsignmentNo_By_TicketNO", conn1);
                    SqlCommand command5 = new SqlCommand("sp_careconnect_Get_Ticket_DropDownData", conn1);
                    command5.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter dataAdapter5 = new SqlDataAdapter(command5);
                    dataAdapter5.Fill(dataSet5);
                    command3.CommandType = CommandType.StoredProcedure;
                    command3.Parameters.AddWithValue("@TicketID", ticketid);
                    string consignmentNumber;

                    // Execute the stored procedure and store the result in the variable
                    object result = command3.ExecuteScalar();

                    consignmentNumber = result.ToString(); // Replace 'int' with the appropriate data type



                    // SqlCommand command = new SqlCommand("sp_careconnect_Get_Tracking", conn);
                    SqlCommand command = new SqlCommand("sp_careconnect_Get_Tracking", conn);
                    SqlCommand command1 = new SqlCommand("sp_GetRequestDetails", conn1);
                    SqlCommand command2 = new SqlCommand("sp_GetTicketHistory", conn1);
                    SqlCommand command4 = new SqlCommand("sp_careconnect_Get_Ticket_Status", conn1);


                    command.CommandType = CommandType.StoredProcedure;
                    command1.CommandType = CommandType.StoredProcedure;
                    command2.CommandType = CommandType.StoredProcedure;
                    command4.CommandType = CommandType.StoredProcedure;






                    //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);
                    SqlDataAdapter dataAdapter2 = new SqlDataAdapter(command2);
                    SqlDataAdapter dataAdapter4 = new SqlDataAdapter(command4);


                    // command.Parameters.AddWithValue("@bookingNumber", tracking);
                    command.Parameters.AddWithValue("@bookingNumber", consignmentNumber);
                    command1.Parameters.AddWithValue("@TicketID", ticketid);
                    command2.Parameters.AddWithValue("@TicketID", ticketid);

                    DataSet dataSet3 = new DataSet();
                    DataSet dataSet4 = new DataSet();
                    //dataAdapter.Fill(dataSet);
                    dataAdapter.Fill(dataSet3);
                    dataAdapter1.Fill(dataSet1);
                    dataAdapter2.Fill(dataSet2);
                    dataAdapter4.Fill(dataSet4);

                    var userId = HttpContext.Session.GetString("LoginId"); // Change this to your actual session key
                    var userRoleId = HttpContext.Session.GetString("RoleID"); // Change this to your actual session key

                    //var currentUser = new UserModel
                    //{
                    //    UserId = userId,
                    //    RoleId = userRoleId
                    //};

                    TicketDetailsViewModel model = new TicketDetailsViewModel
                    {
                        BookingDetail = dataSet3,
                        TicketDetails = dataSet1,
                        TicketHistory = dataSet2,
                        TicketStatus = dataSet4,
                        userid = userId,
                        roleid = userRoleId,
                        TicketType = dataSet5

                    };
                    // Replace with your tracking number variable or value
                    ViewData["TicketNo"] = ticketid;
                    string category = "";

                    if (dataSet1.Tables.Count > 0)
                    {
                        DataTable dataTable = dataSet1.Tables[0]; // Assuming the first table in dataSet1 contains the data

                        // Check if the DataTable has at least one row
                        if (dataTable.Rows.Count > 0)
                        {
                            // Access the "Category" value from the first row directly
                            category = dataTable.Rows[0]["Category"].ToString();
                        }
                    }

                    ViewData["TicketCategory"] = category;
                    ViewBag.TrackingData = dataSet1;



                    DataSet dataSet = dataSet1; // Replace with your code to obtain the DataSet object

                    string dataSetJson = JsonConvert.SerializeObject(dataSet);

                    HttpContext.Session.SetString("Data1", dataSetJson);
                    string RoleID = HttpContext.Session.GetString("RoleID");
                    string UserName = HttpContext.Session.GetString("UserName");

                    ViewData["RoleID"] = RoleID;


                    ViewBag.UserName = UserName;

                    //HttpContext.Session["Data1"] = dataSet1;
                    //Session["Data1"] = dataSet1;
                    //HttpContext.Session["dasd"] = dataSet1;
                    //Session["Data1"] = dataSet1;
                    return View("TicketDetails", model);
                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void InsertTrackingHistory(string LoginId, string trackingid)
        {
            db _db = new db();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_Insert_Tracking_History", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("LoginId", LoginId);
                cmd.Parameters.AddWithValue("TrackingID", trackingid);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public ActionResult GetTrackingGenerateDetails(string tracking)
        {

            try
            {
                DataSet dataSet = new DataSet();
                DataSet dataSet1 = new DataSet();
                DataSet dataSet2 = new DataSet();


                SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));
                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {
                    conn.Open();
                    conn1.Open();

                    SqlCommand command = new SqlCommand("sp_careconnect_Get_Tracking_Generate", conn);
                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Ticket_DropDownData", conn1);

                    command.CommandType = CommandType.StoredProcedure;
                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);


                    command.Parameters.AddWithValue("@bookingNumber", tracking);
                    dataAdapter.Fill(dataSet);
                    dataAdapter1.Fill(dataSet1);


                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        BookingDetail = dataSet,
                        TicketType = dataSet1,
                        TicketCatType = dataSet1,
                        PriorityDS = dataSet1,
                        CityDS = dataSet1,
                    };
                    //ViewBag.TrackingData = model;
                    string trackingNumbernew = tracking; // Replace with your tracking number variable or value
                    ViewData["TrackingNumber"] = trackingNumbernew;
                    return PartialView("_TrackingGenerateDetails", model);
                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult GetTicketSearchDetails()
        {

            try
            {
                DataSet dataSet = new DataSet();
                DataSet dataSet1 = new DataSet();
                DataSet dataSet2 = new DataSet();


                SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));
                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {
                    conn.Open();
                    conn1.Open();

                    SqlCommand command = new SqlCommand("sp_careconnect_Get_Tracking_Generate", conn);
                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Ticket_DropDownData", conn1);

                    command.CommandType = CommandType.StoredProcedure;
                    command1.CommandType = CommandType.StoredProcedure;

                    //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);


                    //command.Parameters.AddWithValue("@bookingNumber", tracking);
                    //dataAdapter.Fill(dataSet);
                    dataAdapter1.Fill(dataSet1);


                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        //BookingDetail = dataSet,
                        //TicketType = dataSet1,
                        TicketCatType = dataSet1,
                        //PriorityDS = dataSet1,
                        //CityDS = dataSet1,
                    };
                    //ViewBag.TrackingData = model;
                    //string trackingNumbernew = tracking; // Replace with your tracking number variable or value
                    //ViewData["TrackingNumber"] = trackingNumbernew;
                    return PartialView("_TicketSearchCatagory", model);
                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }


        [HttpGet]
        public JsonResult GetConsignmentNumberByTicketID(string ticketid)
        {
            try
            {
                // Create a variable to store the consignment number
                string consignmentNumber = "";

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    SqlCommand command = new SqlCommand("sp_careconnect_Get_ConsignmentNo_By_TicketID", conn);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TicketID", ticketid);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if there are rows and retrieve the consignment number
                        if (reader.Read())
                        {
                            // Replace "ColumnName" with the actual column name that holds the consignment number
                            consignmentNumber = reader["ConsignmentNo"].ToString();
                        }
                    }
                }

                return Json(new { ConsignmentNo = consignmentNumber });
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                return Json(new { Error = ex.Message }); // You can return an error message if needed
            }
        }




        public ActionResult GetSubcategories(string category)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Ticket_type_description_By_ID", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    command1.Parameters.AddWithValue("@category_Id", category);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        TicketIssueTypeDescription = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_SubCategory", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult GetSubcategoriesCRM(string category)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Ticket_type_description_By_ID_CRM", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    command1.Parameters.AddWithValue("@category_Id", category);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        TicketIssueTypeDescription = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_SubCategory", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult GetSubtermianl(string terminalid)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Sub_Terminal_by_Terminal_ID", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    string type = "ALL";

                    command1.Parameters.AddWithValue("@terminal_id", terminalid);
                    command1.Parameters.AddWithValue("@type", type);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        Sub_Terminal = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_SubTerminal", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult GetSubtermianlFRN(string terminalid)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Sub_Terminal_by_Terminal_ID", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    string type = "FRN";




                    command1.Parameters.AddWithValue("@terminal_id", terminalid);
                    command1.Parameters.AddWithValue("@type", type);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        Sub_Terminal = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_SubTerminal", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult GetSubtermianlAGT(string terminalid)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Sub_Terminal_by_Terminal_ID", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);
                    string type = "AGT";
                   

                    command1.Parameters.AddWithValue("@terminal_id", terminalid);
                    command1.Parameters.AddWithValue("@type", type);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        Sub_Terminal = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_SubTerminal", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult GetSubcategories2(string category)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Ticket_type_description_By_ID", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    command1.Parameters.AddWithValue("@category_Id", category);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        TicketIssueTypeDescription = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_IssueTypeDescription", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult GetSubcategoriesServiceRequest(string category)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Ticket_type_description_By_ID_ServiceRequest", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    command1.Parameters.AddWithValue("@category_Id", category);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        TicketIssueTypeDescription = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_IssueTypeDescription", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }



        public ActionResult Get2SubcategoriesInfo(string subcategory)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {

                    if (conn1.State != ConnectionState.Open)
                        conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_SubCategory_By_ID_Info", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    command1.Parameters.AddWithValue("@subcategory_Id", subcategory);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        InfoSubcategory = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_SubCategoryInfo", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult GetCities(string ticketstatus)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Cities_By_Ticket_Status_ID", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    command1.Parameters.AddWithValue("@ticket_status_Id", ticketstatus);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        CityDS = dataSet
                    };

                    //ViewData["SubCatagory"] = dataSet;
                    //ViewBag["SubCatagory"] = dataSet;
                    return PartialView("_CitiesDropdownView", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult GetRegion(string CityID)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Region_By_ID", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    command1.Parameters.AddWithValue("@City_Id", CityID);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        RegionDS = dataSet
                    };

                    return PartialView("_CityRegion", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult GetCategory(string Barcode)
        {

            try
            {
                DataSet dataSet = new DataSet();




                SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));


                {

                    conn1.Open();


                    SqlCommand command1 = new SqlCommand("sp_careconnect_Get_Category_By_Barcode", conn1);

                    command1.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command1);

                    command1.Parameters.AddWithValue("@Barcode", Barcode);

                    dataAdapter1.Fill(dataSet);

                    TrackingGenerateViewModel model = new TrackingGenerateViewModel
                    {
                        Category = dataSet
                    };

                    return PartialView("_BarcodeCategory", model);


                }
            }




            catch (Exception ex)
            {

                throw ex;
            }
        }





        public IActionResult Tracking()
        {
            string password = HttpContext.Session.GetString("Password");
            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));

            if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
            {
                string RoleID = HttpContext.Session.GetString("RoleID");
                ViewData["RoleID"] = RoleID;
                ViewBag.Role = RoleID;
                //string RoleID = HttpContext.Session.GetString("RoleID");
                string UserName = HttpContext.Session.GetString("UserName");

                ViewData["RoleID"] = RoleID;


                ViewBag.UserName = UserName;

                conn.Close();
                return View("~/Views/Home/Tracking.cshtml", policies);
            }

            return RedirectToAction("Login", "Account");

        }


        public IActionResult SearchTicket()
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    string RoleID = HttpContext.Session.GetString("RoleID");
                    string UserName = HttpContext.Session.GetString("UserName");
                    ViewData["RoleID"] = RoleID;
                    ViewBag.UserName = UserName;
                    DataSet dataSet = new DataSet();
                    DataSet dataSet1 = new DataSet();
                    DataSet dataSet2 = new DataSet();
                    SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));

                    {
                        conn1.Open();
                        SqlCommand command = new SqlCommand("sp_careconnect_Get_Ticket_DropDownData", conn1);
                        command.CommandType = CommandType.StoredProcedure;
                        //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command);
                        //command.Parameters.AddWithValue("@bookingNumber", tracking);
                        //dataAdapter.Fill(dataSet);
                        dataAdapter1.Fill(dataSet1);
                        TrackingGenerateViewModel model = new TrackingGenerateViewModel
                        {
                            //BookingDetail = dataSet,
                            //TicketType = dataSet1,
                            TicketCatType = dataSet1,
                            //PriorityDS = dataSet1,
                            //CityDS = dataSet1,
                        };
                        //ViewBag.TrackingData = model;
                        //string trackingNumbernew = tracking; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;
                        //return PartialView("_TicketSearchCatagory", model);
                        return View("~/Views/Home/SearchTicket.cshtml", model);
                    }
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {

                throw ex;
            }







                //return View("~/Views/Home/SearchTicket.cshtml", model);

                // return View("~/Views/Home/SearchTicket.cshtml");
          

        }


        public IActionResult ReportCRM()
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    string RoleID = HttpContext.Session.GetString("RoleID");
                    string UserName = HttpContext.Session.GetString("UserName");
                    ViewData["RoleID"] = RoleID;
                    ViewBag.UserName = UserName;
                    DataSet dataSet = new DataSet();
                    DataSet dataSet1 = new DataSet();
                    DataSet dataSet2 = new DataSet();
                    SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));

                    {
                        conn1.Open();
                        SqlCommand command = new SqlCommand("sp_careconnect_Get_Ticket_DropDownData", conn1);
                        command.CommandType = CommandType.StoredProcedure;
                        //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command);
                        //command.Parameters.AddWithValue("@bookingNumber", tracking);
                        //dataAdapter.Fill(dataSet);
                        dataAdapter1.Fill(dataSet1);
                        TrackingGenerateViewModel model = new TrackingGenerateViewModel
                        {
                            //BookingDetail = dataSet,
                            //TicketType = dataSet1,
                            TicketCatType = dataSet1,
                            //PriorityDS = dataSet1,
                            //CityDS = dataSet1,
                        };
                        //ViewBag.TrackingData = model;
                        //string trackingNumbernew = tracking; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;
                        //return PartialView("_TicketSearchCatagory", model);
                        return View("~/Views/Home/ReportCRM.cshtml", model);
                    }
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {

                throw ex;
            }







            //return View("~/Views/Home/SearchTicket.cshtml", model);

            // return View("~/Views/Home/SearchTicket.cshtml");


        }



        public IActionResult ReportTerminalAddress()
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    string RoleID = HttpContext.Session.GetString("RoleID");
                    string UserName = HttpContext.Session.GetString("UserName");
                    ViewData["RoleID"] = RoleID;
                    ViewBag.UserName = UserName;
                    DataSet dataSet = new DataSet();
                    DataSet dataSet1 = new DataSet();
                    DataSet dataSet2 = new DataSet();
                    SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection"));

                    {
                        conn1.Open();
                        SqlCommand command = new SqlCommand("sp_careconnect_Get_ALL_Terminal", conn1);
                        command.CommandType = CommandType.StoredProcedure;
                        //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command);
                        //command.Parameters.AddWithValue("@bookingNumber", tracking);
                        //dataAdapter.Fill(dataSet);
                        dataAdapter1.Fill(dataSet1);
                        TrackingGenerateViewModel model = new TrackingGenerateViewModel
                        {
                            //BookingDetail = dataSet,
                            //TicketType = dataSet1,
                            AllTerminals = dataSet1,
                            //PriorityDS = dataSet1,
                            //CityDS = dataSet1,
                        };
                        //ViewBag.TrackingData = model;
                        //string trackingNumbernew = tracking; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;
                        //return PartialView("_TicketSearchCatagory", model);
                        return View("~/Views/Home/ReportTerminalAddress.cshtml", model);
                    }
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {

                throw ex;
            }







            //return View("~/Views/Home/SearchTicket.cshtml", model);

            // return View("~/Views/Home/SearchTicket.cshtml");


        }
        public IActionResult ServiceRequest()
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    string RoleID = HttpContext.Session.GetString("RoleID");
                    string UserName = HttpContext.Session.GetString("UserName");
                    ViewData["RoleID"] = RoleID;
                    ViewBag.UserName = UserName;
                    DataSet dataSet = new DataSet();
                    DataSet dataSet1 = new DataSet();
                    DataSet dataSet2 = new DataSet();
                    SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));

                    {
                        conn1.Open();
                        SqlCommand command = new SqlCommand("sp_careconnect_Get_Ticket_DropDownData", conn1);
                        command.CommandType = CommandType.StoredProcedure;
                        //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command);
                        //command.Parameters.AddWithValue("@bookingNumber", tracking);
                        //dataAdapter.Fill(dataSet);
                        dataAdapter1.Fill(dataSet1);
                        TrackingGenerateViewModel model = new TrackingGenerateViewModel
                        {
                            //BookingDetail = dataSet,
                            //TicketType = dataSet1,
                            TicketCatType = dataSet1,
                            //PriorityDS = dataSet1,
                            //CityDS = dataSet1,
                        };
                        //ViewBag.TrackingData = model;
                        //string trackingNumbernew = tracking; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;
                        //return PartialView("_TicketSearchCatagory", model);
                        return View("~/Views/Home/ServiceRequest.cshtml", model);
                    }
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {

                throw ex;
            }







            //return View("~/Views/Home/SearchTicket.cshtml", model);

            // return View("~/Views/Home/SearchTicket.cshtml");


        }




        public IActionResult Report()
        {
            // HttpContext.Session.Clear();
            // Route for Reneval Vouchers
            // changed by Aizaz and Imran Dated 17 Feb 22
            string password = HttpContext.Session.GetString("Password");
            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
            // string policyQuery = "select PolicyNo,IID from Insurant where NIC=@CNIC and Phone=@MobileNo";
            //string sqlquery = "select count(*) from Insurant";
            if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
            {

                conn.Close();
                //string trackingNumbernew = trackingNumber; // Replace with your tracking number variable or value
                //ViewData["TrackingNumber"] = trackingNumbernew;

                string RoleID = HttpContext.Session.GetString("RoleID");
                string UserName = HttpContext.Session.GetString("UserName");

                ViewData["RoleID"] = RoleID;


                ViewBag.UserName = UserName;
                //ViewBag.UserName = UserName;



                return View("~/Views/Home/ReportData.cshtml");
            }

            return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }


        public IActionResult ReportSMS()
        {
            // HttpContext.Session.Clear();
            // Route for Reneval Vouchers
            // changed by Aizaz and Imran Dated 17 Feb 22
            string password = HttpContext.Session.GetString("Password");
            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
            // string policyQuery = "select PolicyNo,IID from Insurant where NIC=@CNIC and Phone=@MobileNo";
            //string sqlquery = "select count(*) from Insurant";
            if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
            {

                conn.Close();
                //string trackingNumbernew = trackingNumber; // Replace with your tracking number variable or value
                //ViewData["TrackingNumber"] = trackingNumbernew;

                string RoleID = HttpContext.Session.GetString("RoleID");
                string UserName = HttpContext.Session.GetString("UserName");

                ViewData["RoleID"] = RoleID;


                ViewBag.UserName = UserName;
                //ViewBag.UserName = UserName;



                return View("~/Views/Home/ReportSMS.cshtml");
            }

            return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }

        public IActionResult ReportClose()
        {
            // HttpContext.Session.Clear();
            // Route for Reneval Vouchers
            // changed by Aizaz and Imran Dated 17 Feb 22
            string password = HttpContext.Session.GetString("Password");
            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
            // string policyQuery = "select PolicyNo,IID from Insurant where NIC=@CNIC and Phone=@MobileNo";
            //string sqlquery = "select count(*) from Insurant";
            if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
            {

                conn.Close();
                //string trackingNumbernew = trackingNumber; // Replace with your tracking number variable or value
                //ViewData["TrackingNumber"] = trackingNumbernew;

                string RoleID = HttpContext.Session.GetString("RoleID");
                string UserName = HttpContext.Session.GetString("UserName");

                ViewData["RoleID"] = RoleID;


                ViewBag.UserName = UserName;
                //ViewBag.UserName = UserName;



                return View("~/Views/Reports/ReportClose.cshtml");
            }

            return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }
        public IActionResult TicketGenerate(string trackingNumber)
        {
            // HttpContext.Session.Clear();
            // Route for Reneval Vouchers
            // changed by Aizaz and Imran Dated 17 Feb 22
            string password = HttpContext.Session.GetString("Password");
            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
            // string policyQuery = "select PolicyNo,IID from Insurant where NIC=@CNIC and Phone=@MobileNo";
            //string sqlquery = "select count(*) from Insurant";
            if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
            {
                if (HttpContext.Session.GetString("RoleID") == "4")
                {
                    TempData["AlertMessage"] = "Your Are Not Authorize to Generate a Ticket.!";
                    string url = $"/Home/Tracking";
                    return Redirect(url);
                }


                else
                {
                    conn.Close();
                    string trackingNumbernew = trackingNumber; // Replace with your tracking number variable or value
                    ViewData["TrackingNumber"] = trackingNumbernew;
                    string RoleID = HttpContext.Session.GetString("RoleID");
                    string UserName = HttpContext.Session.GetString("UserName");

                    ViewData["RoleID"] = RoleID;


                    ViewBag.UserName = UserName;


                    return View("~/Views/Home/TicketGenerate.cshtml");
                }

            }

            return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }



        //public IActionResult Ticketdetail(string ticketno)
        //{
        //    // HttpContext.Session.Clear();
        //    // Route for Reneval Vouchers
        //    // changed by Aizaz and Imran Dated 17 Feb 22
        //    string password = HttpContext.Session.GetString("Password");
        //    List<Policy1> policies = new List<Policy1>();
        //    SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
        //    // string policyQuery = "select PolicyNo,IID from Insurant where NIC=@CNIC and Phone=@MobileNo";
        //    //string sqlquery = "select count(*) from Insurant";
        //    if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
        //    {

        //        conn.Close();
        //        string ticketnumber = ticketno; // Replace with your tracking number variable or value
        //        ViewData["TicketNo"] = ticketnumber;
        //        return View("~/Views/Home/TicketDetails.cshtml");
        //    }

        //    return RedirectToAction("Login", "Account");
        //    // return View("~/Views/Account/Login.cshtml");
        //}


        [HttpPost]
        public ActionResult CreateTicket(CreateTicketModel createTicketModel)
        {
            SqlTransaction _Transaction = null;
            try
            {

                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("InsertTicket", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlTransaction transaction = conn.BeginTransaction();
                        cmd.Transaction = transaction;
                        cmd.Parameters.AddWithValue("@UserId", HttpContext.Session.GetString("LoginId"));
                        cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@Complainer", createTicketModel.Name);
                        cmd.Parameters.AddWithValue("@ComplainerCell", createTicketModel.Contact);

                        cmd.Parameters.AddWithValue("@Gender", createTicketModel.Gender);

                        cmd.Parameters.AddWithValue("@TicketTypeId", createTicketModel.tickettype);
                        cmd.Parameters.AddWithValue("@Barcode", createTicketModel.barcode);
                        cmd.Parameters.AddWithValue("@BarcodeCategory", createTicketModel.barcodecategoryname);
                        cmd.Parameters.AddWithValue("@ticketcategory", createTicketModel.ticketcatagory);
                        cmd.Parameters.AddWithValue("@IssueTypeId", createTicketModel.ticketsubcatagory);
                        if (createTicketModel.ticketcatagory==6)
                        {
                            cmd.Parameters.AddWithValue("@SMSAllow", false);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@SMSAllow", createTicketModel.issendsms);
                        }

                        
                        cmd.Parameters.AddWithValue("@Priority", createTicketModel.priority);
                        cmd.Parameters.AddWithValue("@CityId", createTicketModel.city);
                        cmd.Parameters.AddWithValue("@RegionId", createTicketModel.region);
                        cmd.Parameters.AddWithValue("@CnsgNo", createTicketModel.cgnno);
                        cmd.Parameters.AddWithValue("@Product", createTicketModel.Product);
                        cmd.Parameters.AddWithValue("@AccountNo", createTicketModel.accountno);

                        cmd.Parameters.AddWithValue("@Agent", createTicketModel.agent);
                        cmd.Parameters.AddWithValue("@AgentReference", createTicketModel.agentref);
                        cmd.Parameters.AddWithValue("@BookingDate", createTicketModel.BookingDate);
                        cmd.Parameters.AddWithValue("@Sender", createTicketModel.sender);
                        cmd.Parameters.AddWithValue("@SenderCompany", createTicketModel.sendercompany);
                        cmd.Parameters.AddWithValue("@SenderReference", createTicketModel.senderref);
                        cmd.Parameters.AddWithValue("@SenderPhone", createTicketModel.senderphone);
                        cmd.Parameters.AddWithValue("@SenderAddress", createTicketModel.senderaddress);
                        cmd.Parameters.AddWithValue("@Origin", createTicketModel.Origin);
                        cmd.Parameters.AddWithValue("@Origin_Desc", createTicketModel.Origin_Desc);
                        cmd.Parameters.AddWithValue("@Receiver", createTicketModel.receiver);
                        cmd.Parameters.AddWithValue("@ReceiverCompany", createTicketModel.receivercompany);
                        cmd.Parameters.AddWithValue("@ReceiverPhone", createTicketModel.receiverphone);
                        cmd.Parameters.AddWithValue("@ReceiverAddress", createTicketModel.receiveraddress);
                        cmd.Parameters.AddWithValue("@Destination", createTicketModel.Destination);
                        cmd.Parameters.AddWithValue("@Destination_Desc", createTicketModel.Destination_Desc);
                        cmd.Parameters.AddWithValue("@PaymentMode", createTicketModel.Payment_Mode);
                        cmd.Parameters.AddWithValue("@Weight", createTicketModel.Weight);
                        cmd.Parameters.AddWithValue("@Pieces", createTicketModel.Quantity);
                        cmd.Parameters.AddWithValue("@IsClosed", 0);
                        cmd.Parameters.AddWithValue("@EmailAddress", createTicketModel.Name);
                        cmd.Parameters.AddWithValue("@Remarks", createTicketModel.Remarks);
                        cmd.Parameters.AddWithValue("@CreatedOn", DateAndTime.Today);

                        // Add the @ticketExists output parameter
                        SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        ticketExistsParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketExistsParam);

                        // Add the @TicketID output parameter
                        SqlParameter ticketIDParam = new SqlParameter("@TicketID", SqlDbType.Int);
                        ticketIDParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketIDParam);
                        //SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        //ticketExistsParam.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(ticketExistsParam);
                        // conn.Close();
                        //conn.Open();


                        cmd.ExecuteNonQuery();




                        // Check if the ticket already exists in your database
                        //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;

                    


                        bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;
                        int ticketID = (int)cmd.Parameters["@TicketID"].Value;
                        string Complainer = (string)cmd.Parameters["@Complainer"].Value;
                        string ComplainerCell = (string)cmd.Parameters["@ComplainerCell"].Value;

                        // Display the appropriate message based on the ticket existence
                        if (ticketExists)
                        {

                            // Ticket already exists
                            return Json(new { success = false, message = "Ticket already Created." });
                        }
                        else
                        {
                            if (createTicketModel.issendsms == false || createTicketModel.ticketcatagory == 6)
                            {
                                int ticketno1 = ticketID; // Replace with your tracking number variable or value
                                int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                                ViewData["TicketUpdateNo"] = ticketupdateno;
                                transaction.Commit();

                                //ViewData["TicketNo"] = ticketno1;
                                return Json(new { success = true, data = ticketupdateno, message = "Ticket Created Successfully not send sms." });
                            }


                            string result = "";
                            string pattern = @"^92\d{10}$";

                            string complainerCell = ComplainerCell;

                            if (Regex.IsMatch(complainerCell, pattern))
                            {
                                string strPost = "";
                                // strPost = "loginId=923114814965&loginPassword=Zong@123&Destination=" + complainerCell + "&Mask=Daewoo-Exp&Message=Test SMS!!! SMS&UniCode=0&ShortCodePrefered=n";


                                string loginId = "923114814965";
                                string loginPassword = "Zong@123";
                                string complainerCellno = complainerCell; // Replace with the actual recipient's phone number
                                string complainer = Complainer; // Replace with the actual complainer's name
                                string complaint = createTicketModel.ticketcatagoryName; // Replace with the actual complaint
                                int ticketId = ticketID; // Replace with the actual ticket ID

                                string SMSmessage = $"Dear {complainer},\nYour {complaint} with Ticket ID {ticketId} has been registered. Track your shipment https://fastex.pk or Dial 042111007009";

                                strPost = $"loginId={loginId}&loginPassword={loginPassword}&Destination={complainerCellno}&Mask=Daewoo-Exp&Message={SMSmessage}&UniCode=0&ShortCodePrefered=n";

                                // Valid contact number


                                // Testing API


                                //strPost = "loginId=923114814965&loginPassword=Zong@123&Destination=" + complainerCell + "&Mask=Daewoo-Exp&Message=Test &UniCode=0&ShortCodePrefered=n";

                                StreamWriter myWriter = null;
                                ServicePointManager.SecurityProtocol = ((SecurityProtocolType)(3072));
                                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://cbs.zong.com.pk/reachrestapi/home/SendQuickSMS?");
                                objRequest.Method = "POST";
                                objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
                                objRequest.ContentType = "application/x-www-form-urlencoded";
                                try
                                {
                                    myWriter = new StreamWriter(objRequest.GetRequestStream());
                                    myWriter.Write(strPost);
                                }
                                catch (Exception e)
                                {
                                }
                                finally
                                {
                                    myWriter.Close();
                                }



                                try
                                {

                                    string dataSetJson = HttpContext.Session.GetString("Data1");

                                    //DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(dataSetJson);

                                    string myValue = ViewBag.TrackingData;


                                    //DataTable table = dataSet.Tables["Table"];
                                    //DataRow row = table.Rows[0];

                                    //// Retrieve values from the row and store them in variables
                                    //int ticketId1 = Convert.ToInt32(row["TicketId"]);
                                    //long ConsignmentNo = Convert.ToInt32(row["CnsgNo"]);
                                    //string category = row["Category"].ToString();
                                    //string issueType = row["IssueType"].ToString();
                                    //string Origin = row["Origin"].ToString();
                                    //string Destination = row["Destination"].ToString();



                                    string subject;
                                    subject = "  Care Connect: Ticket: " + ticketID + " - Consignment #: " + createTicketModel.cgnno + " - Category: " + createTicketModel.ticketcatagoryName + " Issue Type: " + createTicketModel.ticketsubcatagoryName + " - Origin: " + createTicketModel.Origin + " - Destination: " + createTicketModel.Destination + " ";
                                    //string EmailBody = "<html><body><h1>Email Content</h1><p>This is the content of my email.</p></body></html>";

                                    //Ticket: -Consignment NO: -Category:      Issue Type:       -Origin:      -Destination:   


                                    string textBody = " <table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr bgcolor='#4da6ff'><td><b>Ticked ID</b></td> <td> <b> Consignment #</b> </td><td> <b> Category </b> </td> <td> <b> Issue Type</b> </td> <td> <b> Origin</b> </td>  <td> <b> Destination</b> </td>  </tr>";

                                    textBody += "<tr><td>" + ticketID + "</td><td> " + createTicketModel.cgnno + "</td> <td> " + createTicketModel.ticketcatagoryName + "</td>  <td> " + createTicketModel.ticketsubcatagoryName + "</td>  <td> " + createTicketModel.Origin + "</td>  <td> " + createTicketModel.Destination + "</td> </tr>";
                                    textBody += "</table> ";


                                    textBody += "</br><table><tr><td> <b> Agent Remarks:  </b>" + createTicketModel.Remarks + "<tr><td> </table> \n" +
                                            "</br><table> <tr><td>\n" +
                                            "Kindly Login Care Connect Portal URL. http://careconnect.daewoo.net.pk/ </br></br></td></tr>\n" +

                                           "<tr><td>\n" +
                                           "Best regards,</br> </td></tr>\n" +
                                           "<tr><td>\n" +
                                           "MIS Department</br> </td></tr>\n" +

                                           "<tr style=font-size:10px><td>\n" +
                                           "<b>Note:This is system generated Email.</b></br> </td></tr>\n" +
                                           " </table>";

                                    if (IsValidEmail(createTicketModel.UserEmail) && IsValidEmail(createTicketModel.UserEmail))
                                    {
                                        // Call your send email function
                                        SendEmail(createTicketModel.UserEmail, createTicketModel.UserEmail, subject, textBody);
                                    }
                                    else
                                    {
                                        // Show an alert indicating that one or both email addresses are not valid
                                        // You can use your preferred method to display the alert (e.g., SweetAlert, JavaScript alert, etc.)
                                        //  return Content("<script>alert('One or both email addresses are not valid.');</script>");


                                        return Json(new { success = false, message = "Email Address is Not Valid." });
                                    }



                                    //  SendEmail(EmailTicketModel.emailto, EmailTicketModel.emailcc, "Email From Care Connect", EmailTicketModel.remarks);




                                    //SqlTransaction transaction1 = conn.BeginTransaction();
                                    SqlCommand cmd1 = new SqlCommand("sp_Ticket_Email", conn);
                                    cmd1.CommandType = CommandType.StoredProcedure;
                                    cmd1.Transaction = transaction;
                                    cmd1.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                    //cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                                    cmd1.Parameters.AddWithValue("@Comment", createTicketModel.Remarks);
                                    cmd1.Parameters.AddWithValue("@TicketId", ticketID);

                                    cmd1.Parameters.AddWithValue("@Activity", "Auto-Email");

                                    // conn1.Close();
                                    //conn1.Open();
                                    cmd1.ExecuteNonQuery();





                                    // Make the HTTP request to the SMS API
                                    //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("your_sms_api_url_here");
                                    //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

                                    // Check the HTTP status code to determine the result
                                    if (objResponse.StatusCode == HttpStatusCode.OK)
                                    {
                                        using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                                        {
                                            string result1 = sr.ReadToEnd();
                                            string[] responseParts = result1.Split('|');


                                            string statusCode = responseParts[0];
                                            string message = responseParts[1];

                                            // Show appropriate message based on the response
                                            if (statusCode == "200" || statusCode == "0")
                                            {
                                                int ticketno1 = ticketID; // Replace with your tracking number variable or value
                                                ViewData["TicketNo"] = ticketno1;
                                                string issendsms = "1";


                                                SqlCommand cmd2 = new SqlCommand("sp_Insert_SMS", conn);
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Transaction = transaction;
                                                cmd2.Parameters.AddWithValue("@issendsms", issendsms);
                                                cmd2.Parameters.AddWithValue("@TicketId", ticketID);
                                                cmd2.Parameters.AddWithValue("@Remarks", SMSmessage);
                                                cmd2.Parameters.AddWithValue("@Message", message);
                                                cmd2.Parameters.AddWithValue("@MobileNo", complainerCell);
                                                cmd2.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                                cmd2.Parameters.AddWithValue("@ErrorCode", statusCode);
                                                //conn1.Close();
                                                //conn1.Open();
                                                cmd2.ExecuteNonQuery();




                                                int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                                                ViewData["TicketUpdateNo"] = ticketupdateno;
                                                transaction.Commit();

                                                // Ticket inserted successfully
                                                return Json(new { success = true, data = ticketupdateno, message = "Ticket has been Created Successfully." });



                                            }
                                            else
                                            {

                                                int ticketno1 = ticketID; // Replace with your tracking number variable or value
                                                ViewData["TicketNo"] = ticketno1;
                                                string issendsms = "0";

                                                SqlCommand cmd3 = new SqlCommand("sp_Insert_SMS", conn);
                                                cmd1.CommandType = CommandType.StoredProcedure;

                                                cmd3.Parameters.AddWithValue("@issendsms", issendsms);
                                                cmd3.Parameters.AddWithValue("@TicketId", ticketID);
                                                cmd3.Parameters.AddWithValue("@Message", message);
                                                cmd3.Parameters.AddWithValue("@MobileNo", complainerCell);
                                                cmd3.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                                cmd3.Parameters.AddWithValue("@ErrorCode", statusCode);
                                                cmd3.ExecuteNonQuery();
                                                int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                                                ViewData["TicketUpdateNo"] = ticketupdateno;

                                                // Ticket inserted successfully
                                                return Json(new { success = false, data = ticketupdateno, message = $"Ticket Created Successfully!! SMS not sent. Error: {message}" });

                                            }
                                        }
                                    }

                                    else
                                    {


                                    }
                                    objResponse.Close();
                                }
                                catch (WebException ex)
                                {
                                    transaction.Rollback();
                                    Console.WriteLine($"Error occurred while sending SMS: {ex.Message}");
                                    return Json(new { success = false, message = $"Error occurred while sending SMS: {ex.Message}" });
                                }
                                finally
                                {
                                    conn.Close();
                                }
                            }
                            else
                            {
                                return Json(new { success = false, message = "Invalid Mobile No." });
                            }
                            int ticketno = ticketID; // Replace with your tracking number variable or value
                            ViewData["TicketNo"] = ticketno;
                            return Json(new { success = true, data = ticketno, message = "Ticket created successfully." });
                        }
                    }
                }
                return RedirectToAction("Login", "Account");

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Error creating ticket.", error = ex.Message });
            }

        }


        [HttpPost]
        public ActionResult CreateTicketServiceRequest(CreateTicketModel createTicketModel)
        {
            SqlTransaction _Transaction = null;
            try
            {

                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        conn.Open();
                   

                        SqlCommand cmd = new SqlCommand("InsertTicket_Service_Request", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlTransaction transaction = conn.BeginTransaction();
                        cmd.Transaction = transaction;
                        cmd.Parameters.AddWithValue("@UserId", HttpContext.Session.GetString("LoginId"));
                        cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@Complainer", createTicketModel.Name);
                        cmd.Parameters.AddWithValue("@ComplainerCell", createTicketModel.Contact);

                        cmd.Parameters.AddWithValue("@Gender", createTicketModel.Gender);

                        cmd.Parameters.AddWithValue("@TicketTypeId", createTicketModel.tickettype);
                        cmd.Parameters.AddWithValue("@Barcode", createTicketModel.barcode);
                        cmd.Parameters.AddWithValue("@ticketcategory", createTicketModel.ticketcatagory);
                        cmd.Parameters.AddWithValue("@IssueTypeId", createTicketModel.ticketsubcatagory);
                        cmd.Parameters.AddWithValue("@infosubcategory", createTicketModel.infosubcatagoryName);

                        cmd.Parameters.AddWithValue("@SMSAllow", createTicketModel.issendsms);
                        cmd.Parameters.AddWithValue("@Priority", createTicketModel.priority);
                        cmd.Parameters.AddWithValue("@CityId", createTicketModel.city);
                        cmd.Parameters.AddWithValue("@RegionId", createTicketModel.region);
                        cmd.Parameters.AddWithValue("@CnsgNo", createTicketModel.cgnno);
                        cmd.Parameters.AddWithValue("@Product", createTicketModel.Product);
                        cmd.Parameters.AddWithValue("@AccountNo", createTicketModel.accountno);

                        cmd.Parameters.AddWithValue("@Agent", createTicketModel.agent);
                        cmd.Parameters.AddWithValue("@AgentReference", createTicketModel.agentref);
                        cmd.Parameters.AddWithValue("@BookingDate", null);
                        cmd.Parameters.AddWithValue("@Sender", createTicketModel.sender);
                        cmd.Parameters.AddWithValue("@SenderCompany", createTicketModel.sendercompany);
                        cmd.Parameters.AddWithValue("@SenderReference", createTicketModel.senderref);
                        cmd.Parameters.AddWithValue("@SenderPhone", createTicketModel.senderphone);
                        cmd.Parameters.AddWithValue("@SenderAddress", createTicketModel.senderaddress);
                        cmd.Parameters.AddWithValue("@Origin", createTicketModel.Origin);
                        cmd.Parameters.AddWithValue("@Origin_Desc", createTicketModel.Origin_Desc);
                        cmd.Parameters.AddWithValue("@Receiver", createTicketModel.receiver);
                        cmd.Parameters.AddWithValue("@ReceiverCompany", createTicketModel.receivercompany);
                        cmd.Parameters.AddWithValue("@ReceiverPhone", createTicketModel.receiverphone);
                        cmd.Parameters.AddWithValue("@ReceiverAddress", createTicketModel.receiveraddress);
                        cmd.Parameters.AddWithValue("@Destination", createTicketModel.Destination);
                        cmd.Parameters.AddWithValue("@Destination_Desc", createTicketModel.Destination_Desc);
                        cmd.Parameters.AddWithValue("@PaymentMode", createTicketModel.Payment_Mode);
                        cmd.Parameters.AddWithValue("@Weight", createTicketModel.Weight);
                        cmd.Parameters.AddWithValue("@Pieces", createTicketModel.Quantity);
                        cmd.Parameters.AddWithValue("@IsClosed", 1);
                        cmd.Parameters.AddWithValue("@EmailAddress", createTicketModel.Name);
                        cmd.Parameters.AddWithValue("@Remarks", createTicketModel.Remarks);
                        cmd.Parameters.AddWithValue("@CreatedOn", DateAndTime.Today);

                        // Add the @ticketExists output parameter
                        SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        ticketExistsParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketExistsParam);

                        // Add the @TicketID output parameter
                        SqlParameter ticketIDParam = new SqlParameter("@TicketID", SqlDbType.Int);
                        ticketIDParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketIDParam);
                        //SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        //ticketExistsParam.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(ticketExistsParam);
                        // conn.Close();
                        //conn.Open();


                        cmd.ExecuteNonQuery();




                        // Check if the ticket already exists in your database
                        //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;




                        bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;
                        int ticketID = (int)cmd.Parameters["@TicketID"].Value;
                        //string Complainer = (string)cmd.Parameters["@Complainer"].Value;
                        //string ComplainerCell = (string)cmd.Parameters["@ComplainerCell"].Value;

                        // Display the appropriate message based on the ticket existence
                        if (ticketExists)
                        {

                            // Ticket already exists
                            return Json(new { success = false, message = "Ticket already Created." });
                        }
                        else
                        {
                            if (createTicketModel.issendsms == false)
                            {
                                int ticketno1 = ticketID; // Replace with your tracking number variable or value
                                int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                                ViewData["TicketUpdateNo"] = ticketupdateno;
                                transaction.Commit();

                                //ViewData["TicketNo"] = ticketno1;
                                return Json(new { success = true, data = ticketupdateno, message = "Service Request Created Successfully." });
                            }


                            //string result = "";
                            //string pattern = @"^92\d{10}$";

                            //string complainerCell = ComplainerCell;

                            //if (Regex.IsMatch(complainerCell, pattern))
                            //{
                            //    string strPost = "";
                            //    // strPost = "loginId=923114814965&loginPassword=Zong@123&Destination=" + complainerCell + "&Mask=Daewoo-Exp&Message=Test SMS!!! SMS&UniCode=0&ShortCodePrefered=n";


                            //    string loginId = "923114814965";
                            //    string loginPassword = "Zong@123";
                            //    string complainerCellno = complainerCell; // Replace with the actual recipient's phone number
                            //    string complainer = Complainer; // Replace with the actual complainer's name
                            //    string complaint = createTicketModel.ticketcatagoryName; // Replace with the actual complaint
                            //    int ticketId = ticketID; // Replace with the actual ticket ID

                            //    string SMSmessage = $"Dear {complainer},\nYour {complaint} with Ticket ID {ticketId} has been registered. Track your shipment https://fastex.pk or Dial 042111007009";

                            //    strPost = $"loginId={loginId}&loginPassword={loginPassword}&Destination={complainerCellno}&Mask=Daewoo-Exp&Message={SMSmessage}&UniCode=0&ShortCodePrefered=n";

                            //    // Valid contact number


                            //    // Testing API


                            //    //strPost = "loginId=923114814965&loginPassword=Zong@123&Destination=" + complainerCell + "&Mask=Daewoo-Exp&Message=Test &UniCode=0&ShortCodePrefered=n";

                            //    StreamWriter myWriter = null;
                            //    ServicePointManager.SecurityProtocol = ((SecurityProtocolType)(3072));
                            //    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://cbs.zong.com.pk/reachrestapi/home/SendQuickSMS?");
                            //    objRequest.Method = "POST";
                            //    objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
                            //    objRequest.ContentType = "application/x-www-form-urlencoded";
                            //    try
                            //    {
                            //        myWriter = new StreamWriter(objRequest.GetRequestStream());
                            //        myWriter.Write(strPost);
                            //    }
                            //    catch (Exception e)
                            //    {
                            //    }
                            //    finally
                            //    {
                            //        myWriter.Close();
                            //    }



                            //    try
                            //    {

                            //        string dataSetJson = HttpContext.Session.GetString("Data1");

                            //        //DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(dataSetJson);

                            //        string myValue = ViewBag.TrackingData;


                            //        //DataTable table = dataSet.Tables["Table"];
                            //        //DataRow row = table.Rows[0];

                            //        //// Retrieve values from the row and store them in variables
                            //        //int ticketId1 = Convert.ToInt32(row["TicketId"]);
                            //        //long ConsignmentNo = Convert.ToInt32(row["CnsgNo"]);
                            //        //string category = row["Category"].ToString();
                            //        //string issueType = row["IssueType"].ToString();
                            //        //string Origin = row["Origin"].ToString();
                            //        //string Destination = row["Destination"].ToString();



                            //        string subject;
                            //        subject = "  Care Connect: Ticket: " + ticketID + " - Consignment #: " + createTicketModel.cgnno + " - Category: " + createTicketModel.ticketcatagoryName + " Issue Type: " + createTicketModel.ticketsubcatagoryName + " - Origin: " + createTicketModel.Origin + " - Destination: " + createTicketModel.Destination + " ";
                            //        //string EmailBody = "<html><body><h1>Email Content</h1><p>This is the content of my email.</p></body></html>";

                            //        //Ticket: -Consignment NO: -Category:      Issue Type:       -Origin:      -Destination:   


                            //        string textBody = " <table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr bgcolor='#4da6ff'><td><b>Ticked ID</b></td> <td> <b> Consignment #</b> </td><td> <b> Category </b> </td> <td> <b> Issue Type</b> </td> <td> <b> Origin</b> </td>  <td> <b> Destination</b> </td>  </tr>";

                            //        textBody += "<tr><td>" + ticketID + "</td><td> " + createTicketModel.cgnno + "</td> <td> " + createTicketModel.ticketcatagoryName + "</td>  <td> " + createTicketModel.ticketsubcatagoryName + "</td>  <td> " + createTicketModel.Origin + "</td>  <td> " + createTicketModel.Destination + "</td> </tr>";
                            //        textBody += "</table> ";


                            //        textBody += "</br><table><tr><td> <b> Agent Remarks:  </b>" + createTicketModel.Remarks + "<tr><td> </table> \n" +
                            //                "</br><table> <tr><td>\n" +
                            //                "Kindly Login Care Connect Portal URL. https://localhost:44371/ </br></br></td></tr>\n" +

                            //               "<tr><td>\n" +
                            //               "Best regards,</br> </td></tr>\n" +
                            //               "<tr><td>\n" +
                            //               "MIS Department</br> </td></tr>\n" +

                            //               "<tr style=font-size:10px><td>\n" +
                            //               "<b>Note:This is system generated Email.</b></br> </td></tr>\n" +
                            //               " </table>";

                            //        if (IsValidEmail(createTicketModel.UserEmail) && IsValidEmail(createTicketModel.UserEmail))
                            //        {
                            //            // Call your send email function
                            //            SendEmail(createTicketModel.UserEmail, createTicketModel.UserEmail, subject, textBody);
                            //        }
                            //        else
                            //        {
                            //            // Show an alert indicating that one or both email addresses are not valid
                            //            // You can use your preferred method to display the alert (e.g., SweetAlert, JavaScript alert, etc.)
                            //            //  return Content("<script>alert('One or both email addresses are not valid.');</script>");


                            //            return Json(new { success = false, message = "Email Address is Not Valid." });
                            //        }



                            //        //  SendEmail(EmailTicketModel.emailto, EmailTicketModel.emailcc, "Email From Care Connect", EmailTicketModel.remarks);




                            //        //SqlTransaction transaction1 = conn.BeginTransaction();
                            //        SqlCommand cmd1 = new SqlCommand("sp_Ticket_Email", conn);
                            //        cmd1.CommandType = CommandType.StoredProcedure;
                            //        cmd1.Transaction = transaction;
                            //        cmd1.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                            //        //cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                            //        cmd1.Parameters.AddWithValue("@Comment", createTicketModel.Remarks);
                            //        cmd1.Parameters.AddWithValue("@TicketId", ticketID);

                            //        cmd1.Parameters.AddWithValue("@Activity", "Auto-Email");

                            //        // conn1.Close();
                            //        //conn1.Open();
                            //        cmd1.ExecuteNonQuery();





                            //        // Make the HTTP request to the SMS API
                            //        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("your_sms_api_url_here");
                            //        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            //        HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

                            //        // Check the HTTP status code to determine the result
                            //        if (objResponse.StatusCode == HttpStatusCode.OK)
                            //        {
                            //            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                            //            {
                            //                string result1 = sr.ReadToEnd();
                            //                string[] responseParts = result1.Split('|');


                            //                string statusCode = responseParts[0];
                            //                string message = responseParts[1];

                            //                // Show appropriate message based on the response
                            //                if (statusCode == "200" || statusCode == "0")
                            //                {
                            //                    int ticketno1 = ticketID; // Replace with your tracking number variable or value
                            //                    ViewData["TicketNo"] = ticketno1;
                            //                    string issendsms = "1";


                            //                    SqlCommand cmd2 = new SqlCommand("sp_Insert_SMS", conn);
                            //                    cmd2.CommandType = CommandType.StoredProcedure;
                            //                    cmd2.Transaction = transaction;
                            //                    cmd2.Parameters.AddWithValue("@issendsms", issendsms);
                            //                    cmd2.Parameters.AddWithValue("@TicketId", ticketID);
                            //                    cmd2.Parameters.AddWithValue("@Remarks", SMSmessage);
                            //                    cmd2.Parameters.AddWithValue("@Message", message);
                            //                    cmd2.Parameters.AddWithValue("@MobileNo", complainerCell);
                            //                    cmd2.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                            //                    cmd2.Parameters.AddWithValue("@ErrorCode", statusCode);
                            //                    //conn1.Close();
                            //                    //conn1.Open();
                            //                    cmd2.ExecuteNonQuery();




                            //                    int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                            //                    ViewData["TicketUpdateNo"] = ticketupdateno;
                            //                    transaction.Commit();

                            //                    // Ticket inserted successfully
                            //                    return Json(new { success = true, data = ticketupdateno, message = "Ticket has been Created Successfully." });



                            //                }
                            //                else
                            //                {

                            //                    int ticketno1 = ticketID; // Replace with your tracking number variable or value
                            //                    ViewData["TicketNo"] = ticketno1;
                            //                    string issendsms = "0";

                            //                    SqlCommand cmd3 = new SqlCommand("sp_Insert_SMS", conn);
                            //                    cmd1.CommandType = CommandType.StoredProcedure;

                            //                    cmd3.Parameters.AddWithValue("@issendsms", issendsms);
                            //                    cmd3.Parameters.AddWithValue("@TicketId", ticketID);
                            //                    cmd3.Parameters.AddWithValue("@Message", message);
                            //                    cmd3.Parameters.AddWithValue("@MobileNo", complainerCell);
                            //                    cmd3.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                            //                    cmd3.Parameters.AddWithValue("@ErrorCode", statusCode);
                            //                    cmd3.ExecuteNonQuery();
                            //                    int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                            //                    ViewData["TicketUpdateNo"] = ticketupdateno;

                            //                    // Ticket inserted successfully
                            //                    return Json(new { success = false, data = ticketupdateno, message = $"Ticket Created Successfully!! SMS not sent. Error: {message}" });

                            //                }
                            //            }
                            //        }

                            //        else
                            //        {


                            //        }
                            //        objResponse.Close();
                            //    }
                            //    catch (WebException ex)
                            //    {
                            //        transaction.Rollback();
                            //        Console.WriteLine($"Error occurred while sending SMS: {ex.Message}");
                            //        return Json(new { success = false, message = $"Error occurred while sending SMS: {ex.Message}" });
                            //    }
                            //    finally
                            //    {
                            //        conn.Close();
                            //    }
                            //}
                            //else
                            //{
                            //    return Json(new { success = false, message = "Invalid Mobile No." });
                            //}
                            int ticketno = ticketID; // Replace with your tracking number variable or value
                            ViewData["TicketNo"] = ticketno;
                            return Json(new { success = true, data = ticketno, message = "Ticket created successfully." });
                        }
                    }
                }
                return RedirectToAction("Login", "Account");

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Error creating ticket.", error = ex.Message });
            }

        }



        [HttpPost]
        public ActionResult SearchTicketDetail(CreateTicketModel createTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_SearchRequest", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CnsgNo", createTicketModel.cgnno);
                        cmd.Parameters.AddWithValue("@RoleID", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@TicketID", createTicketModel.ticketid);
                        cmd.Parameters.AddWithValue("@Barcode", createTicketModel.barcode);
                        cmd.Parameters.AddWithValue("@CategoryID", createTicketModel.ticketcatagory);

                        cmd.Parameters.AddWithValue("@CityID", null);
                        cmd.Parameters.AddWithValue("@Origin", createTicketModel.Origin);
                        cmd.Parameters.AddWithValue("@Destination", createTicketModel.Destination);
                        cmd.Parameters.AddWithValue("@IssueTypeId", createTicketModel.ticketsubcatagory);
                        cmd.Parameters.AddWithValue("@CreatedFrom", createTicketModel.datefrom);
                        cmd.Parameters.AddWithValue("@CreatedTo", createTicketModel.dateto);
                        cmd.Parameters.AddWithValue("@UserId", HttpContext.Session.GetString("LoginId"));

                        conn.Close();
                        conn.Open();
                        // Execute the stored procedure and retrieve the results into a DataTable
                        DataSet ds = new DataSet();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }

                        conn.Close();

                        TrackingGenerateViewModel model = new TrackingGenerateViewModel
                        {
                            SearchTicketDetail = ds,
                        };

                        ViewBag.TrackingData = ds;
                        return PartialView("_TicketSearchDetail", model);


                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error creating ticket.", error = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult GetCRMReport(CreateTicketModel createTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("SP_CRM_Report", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CnsgNo", createTicketModel.cgnno);
                        cmd.Parameters.AddWithValue("@RoleID", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@TicketID", createTicketModel.ticketid);
                        cmd.Parameters.AddWithValue("@Barcode", createTicketModel.barcode);
                        cmd.Parameters.AddWithValue("@CategoryID", createTicketModel.ticketcatagory);

                        cmd.Parameters.AddWithValue("@CityID", null);
                        cmd.Parameters.AddWithValue("@Origin", createTicketModel.Origin);
                        cmd.Parameters.AddWithValue("@Destination", createTicketModel.Destination);
                        cmd.Parameters.AddWithValue("@IssueTypeId", createTicketModel.ticketsubcatagory);
                        cmd.Parameters.AddWithValue("@CreatedFrom", createTicketModel.datefrom);
                        cmd.Parameters.AddWithValue("@CreatedTo", createTicketModel.dateto);
                        cmd.Parameters.AddWithValue("@UserId", HttpContext.Session.GetString("LoginId"));

                        conn.Close();
                        conn.Open();
                        // Execute the stored procedure and retrieve the results into a DataTable
                        DataSet ds = new DataSet();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }

                        conn.Close();

                        TrackingGenerateViewModel model = new TrackingGenerateViewModel
                        {
                            CRMReportDetail = ds,
                        };

                        ViewBag.TrackingData = ds;
                        return PartialView("_CRMReportDetail", model);


                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error creating ticket.", error = ex.Message });
            }
        }


        [HttpGet]
        public ActionResult GetTerminalAddressReport(CreateTicketModel createTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("CARGOConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("SP_careconnect_TerminalAddress_Report", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@terminalID", createTicketModel.terminal);
                        cmd.Parameters.AddWithValue("@subterminalID", createTicketModel.subterminal);
                        cmd.Parameters.AddWithValue("@isAGTChecked", createTicketModel.isAGTChecked);
                        cmd.Parameters.AddWithValue("@isFRNChecked", createTicketModel.isFRNChecked);
                        conn.Close();
                        conn.Open();
                        // Execute the stored procedure and retrieve the results into a DataTable
                        DataSet ds = new DataSet();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }

                        conn.Close();

                        TrackingGenerateViewModel model = new TrackingGenerateViewModel
                        {
                            TerminalAddressReportDS = ds,
                        };

                        ViewBag.TrackingData = ds;
                        return PartialView("_TerminalAddressReportDetail", model);


                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error creating ticket.", error = ex.Message });
            }
        }




        public IActionResult GetReportDateWise(CreateTicketModel createTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_rpt_ticket_track_details", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@CreatedFrom", createTicketModel.datefrom);
                        cmd.Parameters.AddWithValue("@CreatedTo", createTicketModel.dateto);


                        conn.Close();
                        conn.Open();
                        // Execute the stored procedure and retrieve the results into a DataTable
                        DataSet ds = new DataSet();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }

                        conn.Close();

                        ReportViewModel model = new ReportViewModel
                        {
                            Report_rpt_ticketdetails = ds
                        };


                        string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        //string jsonResponse = await jsonResponse.Content.ReadAsStringAsync();
                        return Json(jsonResponse);

                        //return View("Report", model);

                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error creating ticket.", error = ex.Message });
            }
        }

        public IActionResult GetReportSMSDateWise(CreateTicketModel createTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_rpt_Ticket_SMS", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@CreatedFrom", createTicketModel.datefrom);
                        cmd.Parameters.AddWithValue("@CreatedTo", createTicketModel.dateto);


                        conn.Close();
                        conn.Open();
                        // Execute the stored procedure and retrieve the results into a DataTable
                        DataSet ds = new DataSet();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }

                        conn.Close();

                        ReportViewModel model = new ReportViewModel
                        {
                            Report_rpt_ticketdetails = ds
                        };


                        string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        //string jsonResponse = await jsonResponse.Content.ReadAsStringAsync();
                        return Json(jsonResponse);

                        //return View("Report", model);

                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error creating ticket.", error = ex.Message });
            }
        }

        public IActionResult GetReportCloseDateWise(CreateTicketModel createTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_rpt_Ticket_Close", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@CreatedFrom", createTicketModel.datefrom);
                        cmd.Parameters.AddWithValue("@CreatedTo", createTicketModel.dateto);


                        conn.Close();
                        conn.Open();
                        // Execute the stored procedure and retrieve the results into a DataTable
                        DataSet ds = new DataSet();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }

                        conn.Close();

                        ReportViewModel model = new ReportViewModel
                        {
                            Report_rpt_ticketdetails = ds
                        };


                        string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        //string jsonResponse = await jsonResponse.Content.ReadAsStringAsync();
                        return Json(jsonResponse);

                        //return View("Report", model);

                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error creating ticket.", error = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult UpdateTicket(UpdateTicketModel updateTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_Update_Ticket", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                        //cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@Comment", updateTicketModel.remarks);
                        cmd.Parameters.AddWithValue("@TicketId", updateTicketModel.ticketdid);

                        cmd.Parameters.AddWithValue("@Activity", updateTicketModel.activity);



                        // Add the @ticketExists output parameter
                        SqlParameter ticketExistsParam = new SqlParameter("@ticketUpdateExists", SqlDbType.Bit);
                        ticketExistsParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketExistsParam);

                        // Add the @TicketID output parameter
                        SqlParameter ticketIDParam = new SqlParameter("@TicketUpdateID", SqlDbType.Int);
                        ticketIDParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketIDParam);
                        //SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        //ticketExistsParam.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(ticketExistsParam);
                        conn.Close();
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Check if the ticket already exists in your database
                        //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;

                        bool ticketupdateExists = (bool)cmd.Parameters["@ticketUpdateExists"].Value;
                        int ticketID = (int)cmd.Parameters["@TicketUpdateID"].Value;


                        // Display the appropriate message based on the ticket existence
                        if (ticketupdateExists)
                        {

                            // Ticket already exists
                            return Json(new { success = false, message = "Ticket already Updated." });
                        }
                        else
                        {


                            int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                            ViewData["TicketUpdateNo"] = ticketupdateno;

                            // Ticket inserted successfully
                            return Json(new { success = true, data = ticketupdateno, message = "Ticket Updated successfully." });
                        }

                        // string trackingNumbernew = name; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;

                        // return View("~/Views/Home/TicketGenerate.cshtml");
                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error Updating ticket.", error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult TickedClosed(UpdateTicketModel updateTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_Ticket_Closed", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                        //cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@Comment", updateTicketModel.remarks);
                        cmd.Parameters.AddWithValue("@TicketId", updateTicketModel.ticketdid);
                        cmd.Parameters.AddWithValue("@City_id", updateTicketModel.city);

                        cmd.Parameters.AddWithValue("@Activity", updateTicketModel.activity);



                        // Add the @ticketExists output parameter
                        SqlParameter ticketExistsParam = new SqlParameter("@ticketUpdateExists", SqlDbType.Bit);
                        ticketExistsParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketExistsParam);

                        // Add the @TicketID output parameter
                        SqlParameter ticketIDParam = new SqlParameter("@TicketUpdateID", SqlDbType.Int);
                        ticketIDParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketIDParam);
                        //SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        //ticketExistsParam.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(ticketExistsParam);
                        conn.Close();
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Check if the ticket already exists in your database
                        //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;

                        bool ticketupdateExists = (bool)cmd.Parameters["@ticketUpdateExists"].Value;
                        int ticketID = (int)cmd.Parameters["@TicketUpdateID"].Value;


                        // Display the appropriate message based on the ticket existence
                        if (ticketupdateExists)
                        {

                            // Ticket already exists
                            return Json(new { success = false, message = "Ticket already Updated." });
                        }
                        else
                        {


                            int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                            ViewData["TicketUpdateNo"] = ticketupdateno;

                            // Ticket inserted successfully
                            return Json(new { success = true, data = ticketupdateno, message = "Ticket Closed by Operation successfully." });
                        }

                        // string trackingNumbernew = name; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;

                        // return View("~/Views/Home/TicketGenerate.cshtml");
                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error Updating ticket.", error = ex.Message });
            }
        }



        [HttpPost]
        public ActionResult TicketReopen(UpdateTicketModel updateTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_reopen_ticket", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", HttpContext.Session.GetString("LoginId"));
                        cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@Remarks", updateTicketModel.remarks);
                        cmd.Parameters.AddWithValue("@TicketID", updateTicketModel.ticketdid);
                        //cmd.Parameters.AddWithValue("@City_id", updateTicketModel.city);

                        // cmd.Parameters.AddWithValue("@Activity", updateTicketModel.activity);
                        cmd.Parameters.AddWithValue("@TicketTypeId", updateTicketModel.TicketTypeDropdownid);



                        // Add the @ticketExists output parameter
                        SqlParameter ticketExistsParam = new SqlParameter("@ticketUpdateExists", SqlDbType.Bit);
                        ticketExistsParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketExistsParam);

                        // Add the @TicketID output parameter
                        SqlParameter ticketIDParam = new SqlParameter("@TicketUpdateID", SqlDbType.Int);
                        ticketIDParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketIDParam);
                        //SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        //ticketExistsParam.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(ticketExistsParam);
                        conn.Close();
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Check if the ticket already exists in your database
                        //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;

                        bool ticketupdateExists = (bool)cmd.Parameters["@ticketUpdateExists"].Value;
                        int ticketID = (int)cmd.Parameters["@TicketUpdateID"].Value;


                        // Display the appropriate message based on the ticket existence
                        if (ticketupdateExists)
                        {

                            // Ticket already exists
                            return Json(new { success = false, message = "Ticket already Reopen." });
                        }
                        else
                        {





                            int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                            ViewData["TicketUpdateNo"] = ticketupdateno;



                            string result = "";
                            string pattern = @"^92\d{10}$";
                            string complainerCell = updateTicketModel.complainercell;
                            string SmsBody = "Your Ticket ID '" + updateTicketModel.ticketdid + "' has been Reopened. For more details, please call on 042-111-007-009. Thanks";
                            if (Regex.IsMatch(complainerCell, pattern))

                            {

                                string strPost = "";
                                strPost = "loginId=923114814965&loginPassword=Zong@123&Destination=" + complainerCell + "&Mask=Daewoo-Exp&Message=" + SmsBody + " &SMS&UniCode=0&ShortCodePrefered=n";

                                StreamWriter myWriter = null;
                                ServicePointManager.SecurityProtocol = ((SecurityProtocolType)(3072));
                                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://cbs.zong.com.pk/reachrestapi/home/SendQuickSMS?");
                                objRequest.Method = "POST";
                                objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
                                objRequest.ContentType = "application/x-www-form-urlencoded";
                                try
                                {
                                    myWriter = new StreamWriter(objRequest.GetRequestStream());
                                    myWriter.Write(strPost);
                                }
                                catch (Exception e)
                                {
                                }
                                finally
                                {
                                    myWriter.Close();
                                }

                                try
                                {


                                    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                                    if (objResponse.StatusCode == HttpStatusCode.OK) if (objResponse.StatusCode == HttpStatusCode.OK)
                                        {
                                            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                                            {
                                                string result1 = sr.ReadToEnd();
                                                string[] responseParts = result1.Split('|');


                                                string statusCode = responseParts[0];
                                                string message = responseParts[1];

                                                // Show appropriate message based on the response
                                                if (statusCode == "200" || statusCode == "0")
                                                {
                                                    string ticketno1 = updateTicketModel.ticketdid; // Replace with your tracking number variable or value
                                                    ViewData["TicketNo"] = ticketno1;
                                                    string issendsms = "1";
                                                    using (SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                                                    {
                                                        SqlCommand cmd1 = new SqlCommand("sp_Insert_SMS", conn1);
                                                        cmd1.CommandType = CommandType.StoredProcedure;

                                                        cmd1.Parameters.AddWithValue("@issendsms", issendsms);
                                                        cmd1.Parameters.AddWithValue("@Remarks", updateTicketModel.remarks);
                                                        cmd1.Parameters.AddWithValue("@TicketId", ticketno1);
                                                        cmd1.Parameters.AddWithValue("@Message", message);
                                                        cmd1.Parameters.AddWithValue("@MobileNo", complainerCell);
                                                        cmd1.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                                        cmd1.Parameters.AddWithValue("@ErrorCode", statusCode);
                                                        conn1.Close();
                                                        conn1.Open();
                                                        cmd1.ExecuteNonQuery();




                                                        string ticketupdateno1 = ticketno1; // Replace with your tracking number variable or value
                                                        ViewData["TicketUpdateNo"] = ticketupdateno1;

                                                        // Ticket inserted successfully
                                                        // return Json(new { success = true, data = ticketupdateno, message = "Send SMS successfully." });


                                                    }

                                                }
                                                else
                                                {

                                                    string ticketno1 = updateTicketModel.ticketdid; // Replace with your tracking number variable or value
                                                    ViewData["TicketNo"] = ticketno1;
                                                    string issendsms = "0";
                                                    using (SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                                                    {
                                                        SqlCommand cmd1 = new SqlCommand("sp_Insert_SMS", conn1);
                                                        cmd1.CommandType = CommandType.StoredProcedure;

                                                        cmd1.Parameters.AddWithValue("@issendsms", issendsms);
                                                        cmd1.Parameters.AddWithValue("@TicketId", ticketno1);
                                                        cmd1.Parameters.AddWithValue("@Message", message);
                                                        cmd1.Parameters.AddWithValue("@MobileNo", complainerCell);
                                                        cmd1.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                                        cmd1.Parameters.AddWithValue("@ErrorCode", statusCode);
                                                        conn1.Close();
                                                        conn1.Open();
                                                        cmd1.ExecuteNonQuery();




                                                        string ticketupdateno1 = ticketno1; // Replace with your tracking number variable or value
                                                        ViewData["TicketUpdateNo"] = ticketupdateno1;

                                                        // Ticket inserted successfully
                                                        return Json(new { success = false, data = ticketupdateno, message = $"SMS not sent. Error: {message}" });


                                                    }

                                                }

                                            }



                                        }





                                        else
                                        {


                                        }

                                    // Don't forget to close the response
                                    objResponse.Close();


                                }
                                catch (WebException ex)
                                {
                                    // Handle any exceptions that occurred during the HTTP request
                                    Console.WriteLine($"Error occurred while sending SMS: {ex.Message}");
                                    return Json(new { success = false, message = $"Error occurred while sending SMS: {ex.Message}" });
                                }


                            }
                            else
                            {
                                return Json(new { success = false, message = "Invalid Mobile No." });
                            }


                            // Ticket inserted successfully
                            return Json(new { success = true, data = ticketupdateno, message = "Ticket Reopen Successfully." });
                        }

                        // string trackingNumbernew = name; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;

                        // return View("~/Views/Home/TicketGenerate.cshtml");
                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error Updating ticket.", error = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult TicketClosing(UpdateTicketModel updateTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_Ticket_Closing", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                        //cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@Comment", updateTicketModel.remarks);
                        cmd.Parameters.AddWithValue("@TicketId", updateTicketModel.ticketdid);

                        cmd.Parameters.AddWithValue("@Activity", updateTicketModel.activity);



                        // Add the @ticketExists output parameter
                        SqlParameter ticketExistsParam = new SqlParameter("@ticketUpdateExists", SqlDbType.Bit);
                        ticketExistsParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketExistsParam);

                        // Add the @TicketID output parameter
                        SqlParameter ticketIDParam = new SqlParameter("@TicketUpdateID", SqlDbType.Int);
                        ticketIDParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketIDParam);
                        //SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        //ticketExistsParam.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(ticketExistsParam);
                        conn.Close();
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Check if the ticket already exists in your database
                        //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;

                        bool ticketupdateExists = (bool)cmd.Parameters["@ticketUpdateExists"].Value;
                        int ticketID = (int)cmd.Parameters["@TicketUpdateID"].Value;


                        // Display the appropriate message based on the ticket existence
                        if (ticketupdateExists)
                        {

                            // Ticket already exists
                            return Json(new { success = false, message = "Ticket already Closed." });
                        }
                        else
                        {


                            //using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))



                            int TicketClosingNo = ticketID; // Replace with your tracking number variable or value
                            ViewData["TicketClosingNo"] = TicketClosingNo;

                            // Ticket inserted successfully
                            return Json(new { success = true, data = TicketClosingNo, message = "Ticket Closed Successfully." });
                        }

                        // string trackingNumbernew = name; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;

                        // return View("~/Views/Home/TicketGenerate.cshtml");
                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error Closing Ticket.", error = ex.Message });
            }
        }

        public ActionResult getemailaddresses()
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_get_email_address", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Close();
                        conn.Open();


                        List<string> autocompleteData = new List<string>();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string email = reader.GetString(0); // Assuming the email address is in the first column of the result set
                                autocompleteData.Add(email);
                            }
                        }



                        // Ticket inserted successfully
                        return Json(new { success = true, data = autocompleteData, message = "Get Email successfully." });


                        // string trackingNumbernew = name; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;

                        // return View("~/Views/Home/TicketGenerate.cshtml");
                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new { success = false, message = "Error Updating ticket.", error = ex.Message });
            }
        }


        public bool IsValidEmail(string email)
        {
            // Regular expression pattern for email validation
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            // Split the email addresses by commas
            string[] emailAddresses = email.Split(',');

            // Check if each email address matches the pattern
            foreach (string emailAddress in emailAddresses)
            {
                if (!Regex.IsMatch(emailAddress.Trim(), pattern))
                {
                    return false;
                }
            }

            return true;
        }
        public ActionResult UpdateTicketEmail(EmailTicketModel EmailTicketModel)
        {
            try
            {
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    if (string.IsNullOrEmpty(EmailTicketModel.remarks))
                    {
                        // Call your send email function
                        return Json(new { success = false, message = "Remarks field is Empty." });
                    }
                    string dataSetJson = HttpContext.Session.GetString("Data1");

                    DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(dataSetJson);

                    string myValue = ViewBag.TrackingData;


                    DataTable table = dataSet.Tables["Table"];
                    DataRow row = table.Rows[0];

                    // Retrieve values from the row and store them in variables
                    int ticketId = Convert.ToInt32(row["TicketId"]);
                    long ConsignmentNo = Convert.ToInt32(row["CnsgNo"]);
                    string category = row["Category"].ToString();
                    string issueType = row["IssueType"].ToString();
                    string Origin = row["Origin"].ToString();
                    string Destination = row["Destination"].ToString();



                    string subject;
                    subject = "  Care Connect: Ticket: " + ticketId + " - Consignment #: " + ticketId + " - Category: " + category + " Issue Type: " + issueType + " - Origin: " + Origin + " - Destination: " + Destination + " ";
                    //string EmailBody = "<html><body><h1>Email Content</h1><p>This is the content of my email.</p></body></html>";

                    //Ticket: -Consignment NO: -Category:      Issue Type:       -Origin:      -Destination:   


                    string textBody = " <table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr bgcolor='#4da6ff'><td><b>Ticked ID</b></td> <td> <b> Consignment #</b> </td><td> <b> Category </b> </td> <td> <b> Issue Type</b> </td> <td> <b> Origin</b> </td>  <td> <b> Destination</b> </td>  </tr>";

                    textBody += "<tr><td>" + ticketId + "</td><td> " + ConsignmentNo + "</td> <td> " + category + "</td>  <td> " + issueType + "</td>  <td> " + Origin + "</td>  <td> " + Destination + "</td> </tr>";
                    textBody += "</table> ";


                    textBody += "</br><table><tr><td> <b> Agent Remarks:  </b>" + EmailTicketModel.remarks + "<tr><td> </table> \n" +
                            "</br><table> <tr><td>\n" +
                            "Kindly Login Care Connect Portal URL. http://careconnect.daewoo.net.pk/ </br></br></td></tr>\n" +

                           "<tr><td>\n" +
                           "Best regards,</br> </td></tr>\n" +
                           "<tr><td>\n" +
                           "MIS Department</br> </td></tr>\n" +

                           "<tr style=font-size:10px><td>\n" +
                           "<b>Note:This is system generated Email.</b></br> </td></tr>\n" +
                           " </table>";

                    if (IsValidEmail(EmailTicketModel.emailto) && IsValidEmail(EmailTicketModel.emailcc))
                    {
                        // Call your send email function
                        SendEmail(EmailTicketModel.emailto, EmailTicketModel.emailcc, subject, textBody);
                    }
                    else
                    {
                        // Show an alert indicating that one or both email addresses are not valid
                        // You can use your preferred method to display the alert (e.g., SweetAlert, JavaScript alert, etc.)
                        //  return Content("<script>alert('One or both email addresses are not valid.');</script>");


                        return Json(new { success = false, message = "Email Address is Not Valid." });
                    }



                    //  SendEmail(EmailTicketModel.emailto, EmailTicketModel.emailcc, "Email From Care Connect", EmailTicketModel.remarks);


                    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    {
                        SqlCommand cmd = new SqlCommand("sp_Update_Ticket", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                        //cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                        cmd.Parameters.AddWithValue("@Comment", EmailTicketModel.remarks);
                        cmd.Parameters.AddWithValue("@TicketId", EmailTicketModel.ticketdid);

                        cmd.Parameters.AddWithValue("@Activity", EmailTicketModel.activity);



                        // Add the @ticketExists output parameter
                        SqlParameter ticketExistsParam = new SqlParameter("@ticketUpdateExists", SqlDbType.Bit);
                        ticketExistsParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketExistsParam);

                        // Add the @TicketID output parameter
                        SqlParameter ticketIDParam = new SqlParameter("@TicketUpdateID", SqlDbType.Int);
                        ticketIDParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ticketIDParam);
                        //SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                        //ticketExistsParam.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(ticketExistsParam);
                        conn.Close();
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Check if the ticket already exists in your database
                        //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;

                        bool ticketupdateExists = (bool)cmd.Parameters["@ticketUpdateExists"].Value;
                        int ticketID = (int)cmd.Parameters["@TicketUpdateID"].Value;


                        // Display the appropriate message based on the ticket existence
                        if (ticketupdateExists)
                        {

                            // Ticket already exists
                            return Json(new { success = false, message = "Email Already Sent." });
                        }
                        else
                        {


                            int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                            ViewData["TicketUpdateNo"] = ticketupdateno;

                            // Ticket inserted successfully
                            return Json(new { success = true, data = ticketupdateno, message = "Email Send successfully." });
                        }

                        // string trackingNumbernew = name; // Replace with your tracking number variable or value
                        //ViewData["TrackingNumber"] = trackingNumbernew;

                        // return View("~/Views/Home/TicketGenerate.cshtml");
                    }
                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new
                {
                    success = false,
                    message = "Error Updating ticket.",
                    error = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<ActionResult> UpdateTicketSMSAsync(EmailTicketModel EmailTicketModel)
        {
            try
            {
                string result = "";
                //System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    if (string.IsNullOrEmpty(EmailTicketModel.remarks))
                    {
                        // Call your send email function
                        return Json(new { success = false, message = "Remarks field is Empty." });
                    }
                    string pattern = @"^92\d{10}$";



                    //string complainerCell = "0315-5896001";

                    //complainerCell = $"92{complainerCell.Replace("-", "").TrimStart('0')}";

                    string complainerCell = EmailTicketModel.complainercell;
                    if (Regex.IsMatch(complainerCell, pattern))
                    {
                        // Valid contact number


                        // Testing API

                        string strPost = "";
                        strPost = "loginId=923114814965&loginPassword=Zong@123&Destination=" + complainerCell + "&Mask=Daewoo-Exp&Message=" + EmailTicketModel.remarks + " &SMS&UniCode=0&ShortCodePrefered=n";

                        StreamWriter myWriter = null;
                        ServicePointManager.SecurityProtocol = ((SecurityProtocolType)(3072));
                        HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://cbs.zong.com.pk/reachrestapi/home/SendQuickSMS?");
                        objRequest.Method = "POST";
                        objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
                        objRequest.ContentType = "application/x-www-form-urlencoded";
                        try
                        {
                            myWriter = new StreamWriter(objRequest.GetRequestStream());
                            myWriter.Write(strPost);
                        }
                        catch (Exception e)
                        {
                        }
                        finally
                        {
                            myWriter.Close();
                        }

                        try
                        {

                            // Make the HTTP request to the SMS API
                            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("your_sms_api_url_here");
                            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

                            // Check the HTTP status code to determine the result
                            if (objResponse.StatusCode == HttpStatusCode.OK)
                            {
                                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                                {
                                    string result1 = sr.ReadToEnd();
                                    string[] responseParts = result1.Split('|');


                                    string statusCode = responseParts[0];
                                    string message = responseParts[1];

                                    // Show appropriate message based on the response
                                    if (statusCode == "200" || statusCode == "0")
                                    {
                                        string ticketno1 = EmailTicketModel.ticketdid; // Replace with your tracking number variable or value
                                        ViewData["TicketNo"] = ticketno1;
                                        string issendsms = "1";
                                        using (SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                                        {
                                            SqlCommand cmd1 = new SqlCommand("sp_Insert_SMS", conn1);
                                            cmd1.CommandType = CommandType.StoredProcedure;

                                            cmd1.Parameters.AddWithValue("@issendsms", issendsms);
                                            cmd1.Parameters.AddWithValue("@Remarks", EmailTicketModel.remarks);
                                            cmd1.Parameters.AddWithValue("@TicketId", ticketno1);
                                            cmd1.Parameters.AddWithValue("@Message", message);
                                            cmd1.Parameters.AddWithValue("@MobileNo", complainerCell);
                                            cmd1.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                            cmd1.Parameters.AddWithValue("@ErrorCode", statusCode);
                                            conn1.Close();
                                            conn1.Open();
                                            cmd1.ExecuteNonQuery();




                                            string ticketupdateno = ticketno1; // Replace with your tracking number variable or value
                                            ViewData["TicketUpdateNo"] = ticketupdateno;

                                            //Ticket inserted successfully
                                            return Json(new { success = true, data = ticketupdateno, message = "Send SMS successfully." });


                                        }



                                        //using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                                        //{
                                        //    SqlCommand cmd = new SqlCommand("sp_Update_Ticket", conn);
                                        //    //cmd.CommandType = CommandType.StoredProcedure;
                                        //    //cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                        //    ////cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                                        //    //cmd.Parameters.AddWithValue("@Comment", EmailTicketModel.remarks);
                                        //    //cmd.Parameters.AddWithValue("@TicketId", EmailTicketModel.ticketdid);

                                        //    //cmd.Parameters.AddWithValue("@Activity", EmailTicketModel.activity);



                                        //    //// Add the @ticketExists output parameter
                                        //    //SqlParameter ticketExistsParam = new SqlParameter("@ticketUpdateExists", SqlDbType.Bit);
                                        //    //ticketExistsParam.Direction = ParameterDirection.Output;
                                        //    //cmd.Parameters.Add(ticketExistsParam);

                                        //    //// Add the @TicketID output parameter
                                        //    //SqlParameter ticketIDParam = new SqlParameter("@TicketUpdateID", SqlDbType.Int);
                                        //    //ticketIDParam.Direction = ParameterDirection.Output;
                                        //    //cmd.Parameters.Add(ticketIDParam);
                                        //    ////SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                                        //    ////ticketExistsParam.Direction = ParameterDirection.Output;
                                        //    ////cmd.Parameters.Add(ticketExistsParam);
                                        //    //conn.Close();
                                        //    //conn.Open();
                                        //    //cmd.ExecuteNonQuery();

                                        //    // Check if the ticket already exists in your database
                                        //    //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;


                                        //    int ticketID = (int)cmd.Parameters["@TicketUpdateID"].Value;


                                        //    // Display the appropriate message based on the ticket existence



                                        //    int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                                        //    ViewData["TicketUpdateNo"] = ticketupdateno;

                                        //    // Ticket inserted successfully
                                        //    return Json(new { success = true, data = ticketupdateno, message = "SMS Send successfully." });


                                        //    // string trackingNumbernew = name; // Replace with your tracking number variable or value
                                        //    //ViewData["TrackingNumber"] = trackingNumbernew;

                                        //    // return View("~/Views/Home/TicketGenerate.cshtml");
                                        //}
                                    }
                                    else
                                    {

                                        string ticketno1 = EmailTicketModel.ticketdid; // Replace with your tracking number variable or value
                                        ViewData["TicketNo"] = ticketno1;
                                        string issendsms = "0";
                                        using (SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                                        {
                                            SqlCommand cmd1 = new SqlCommand("sp_Insert_SMS", conn1);
                                            cmd1.CommandType = CommandType.StoredProcedure;

                                            cmd1.Parameters.AddWithValue("@issendsms", issendsms);
                                            cmd1.Parameters.AddWithValue("@TicketId", ticketno1);
                                            cmd1.Parameters.AddWithValue("@Message", message);
                                            cmd1.Parameters.AddWithValue("@MobileNo", complainerCell);
                                            cmd1.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                            cmd1.Parameters.AddWithValue("@ErrorCode", statusCode);
                                            conn1.Close();
                                            conn1.Open();
                                            cmd1.ExecuteNonQuery();




                                            string ticketupdateno = ticketno1; // Replace with your tracking number variable or value
                                            ViewData["TicketUpdateNo"] = ticketupdateno;

                                            // Ticket inserted successfully
                                            return Json(new { success = false, data = ticketupdateno, message = $"SMS not sent. Error: {message}" });


                                        }

                                    }

                                }



                            }

                            else
                            {


                            }

                            // Don't forget to close the response
                            objResponse.Close();
                        }
                        catch (WebException ex)
                        {
                            // Handle any exceptions that occurred during the HTTP request
                            Console.WriteLine($"Error occurred while sending SMS: {ex.Message}");
                            return Json(new { success = false, message = $"Error occurred while sending SMS: {ex.Message}" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid Mobile No." });
                    }



                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new
                {
                    success = false,
                    message = "Error Updating ticket.",
                    error = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<ActionResult> UpdateTicketClosingAsync(EmailTicketModel EmailTicketModel)
        {
            try
            {
                string result = "";
                //System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                if (HttpContext.Session.GetString("LoginId") != "" && HttpContext.Session.GetString("LoginId") != null)
                {
                    if (string.IsNullOrEmpty(EmailTicketModel.remarks))
                    {
                        // Call your send email function
                        return Json(new { success = false, message = "Remarks field is Empty." });
                    }
                    string pattern = @"^92\d{10}$";




                    //string complainerCell = "0315-5896001";

                    //complainerCell = $"92{complainerCell.Replace("-", "").TrimStart('0')}";

                    string complainerCell = EmailTicketModel.complainercell;
                    if (Regex.IsMatch(complainerCell, pattern))
                    {
                        // Valid contact number


                        // Testing API

                        string strPost = "";
                        strPost = "loginId=923114814965&loginPassword=Zong@123&Destination=" + complainerCell + "&Mask=Daewoo-Exp&Message=" + EmailTicketModel.remarks + " &SMS&UniCode=0&ShortCodePrefered=n";

                        StreamWriter myWriter = null;
                        ServicePointManager.SecurityProtocol = ((SecurityProtocolType)(3072));
                        HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://cbs.zong.com.pk/reachrestapi/home/SendQuickSMS?");
                        objRequest.Method = "POST";
                        objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
                        objRequest.ContentType = "application/x-www-form-urlencoded";
                        try
                        {
                            myWriter = new StreamWriter(objRequest.GetRequestStream());
                            myWriter.Write(strPost);
                        }
                        catch (Exception e)
                        {
                        }
                        finally
                        {
                            myWriter.Close();
                        }

                        try
                        {

                            // Make the HTTP request to the SMS API
                            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("your_sms_api_url_here");
                            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

                            // Check the HTTP status code to determine the result
                            if (objResponse.StatusCode == HttpStatusCode.OK)
                            {
                                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                                {
                                    string result1 = sr.ReadToEnd();
                                    string[] responseParts = result1.Split('|');


                                    string statusCode = responseParts[0];
                                    string message = responseParts[1];

                                    // Show appropriate message based on the response
                                    if (statusCode == "200" || statusCode == "0")
                                    {
                                        string ticketno1 = EmailTicketModel.ticketdid; // Replace with your tracking number variable or value
                                        ViewData["TicketNo"] = ticketno1;
                                        string issendsms = "1";
                                        using (SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                                        {
                                            SqlCommand cmd1 = new SqlCommand("sp_Insert_SMS", conn1);
                                            cmd1.CommandType = CommandType.StoredProcedure;

                                            cmd1.Parameters.AddWithValue("@issendsms", issendsms);
                                            cmd1.Parameters.AddWithValue("@Remarks", EmailTicketModel.remarks);
                                            cmd1.Parameters.AddWithValue("@TicketId", ticketno1);
                                            cmd1.Parameters.AddWithValue("@Message", message);
                                            cmd1.Parameters.AddWithValue("@MobileNo", complainerCell);
                                            cmd1.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                            cmd1.Parameters.AddWithValue("@ErrorCode", statusCode);
                                            conn1.Close();
                                            conn1.Open();
                                            cmd1.ExecuteNonQuery();




                                            string ticketupdateno = ticketno1; // Replace with your tracking number variable or value
                                            ViewData["TicketUpdateNo"] = ticketupdateno;

                                            // Ticket inserted successfully
                                            // return Json(new { success = true, data = ticketupdateno, message = "Send SMS successfully." });


                                        }



                                        using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                                        {
                                            SqlCommand cmd = new SqlCommand("sp_Update_Ticket", conn);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                            //cmd.Parameters.AddWithValue("@UserRole", HttpContext.Session.GetString("RoleID"));
                                            cmd.Parameters.AddWithValue("@Comment", EmailTicketModel.remarks);
                                            cmd.Parameters.AddWithValue("@TicketId", EmailTicketModel.ticketdid);

                                            cmd.Parameters.AddWithValue("@Activity", EmailTicketModel.activity);



                                            // Add the @ticketExists output parameter
                                            SqlParameter ticketExistsParam = new SqlParameter("@ticketUpdateExists", SqlDbType.Bit);
                                            ticketExistsParam.Direction = ParameterDirection.Output;
                                            cmd.Parameters.Add(ticketExistsParam);

                                            // Add the @TicketID output parameter
                                            SqlParameter ticketIDParam = new SqlParameter("@TicketUpdateID", SqlDbType.Int);
                                            ticketIDParam.Direction = ParameterDirection.Output;
                                            cmd.Parameters.Add(ticketIDParam);
                                            //SqlParameter ticketExistsParam = new SqlParameter("@ticketExists", SqlDbType.Bit);
                                            //ticketExistsParam.Direction = ParameterDirection.Output;
                                            //cmd.Parameters.Add(ticketExistsParam);
                                            conn.Close();
                                            conn.Open();
                                            cmd.ExecuteNonQuery();

                                            // Check if the ticket already exists in your database
                                            //bool ticketExists = (bool)cmd.Parameters["@ticketExists"].Value;


                                            int ticketID = (int)cmd.Parameters["@TicketUpdateID"].Value;


                                            // Display the appropriate message based on the ticket existence



                                            int ticketupdateno = ticketID; // Replace with your tracking number variable or value
                                            ViewData["TicketUpdateNo"] = ticketupdateno;

                                            // Ticket inserted successfully
                                            return Json(new { success = true, data = ticketupdateno, message = "SMS Send successfully." });


                                            // string trackingNumbernew = name; // Replace with your tracking number variable or value
                                            //ViewData["TrackingNumber"] = trackingNumbernew;

                                            // return View("~/Views/Home/TicketGenerate.cshtml");
                                        }
                                    }
                                    else
                                    {

                                        string ticketno1 = EmailTicketModel.ticketdid; // Replace with your tracking number variable or value
                                        ViewData["TicketNo"] = ticketno1;
                                        string issendsms = "0";
                                        using (SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                                        {
                                            SqlCommand cmd1 = new SqlCommand("sp_Insert_SMS", conn1);
                                            cmd1.CommandType = CommandType.StoredProcedure;

                                            cmd1.Parameters.AddWithValue("@issendsms", issendsms);
                                            cmd1.Parameters.AddWithValue("@TicketId", ticketno1);
                                            cmd1.Parameters.AddWithValue("@Message", message);
                                            cmd1.Parameters.AddWithValue("@MobileNo", complainerCell);
                                            cmd1.Parameters.AddWithValue("@UserID", HttpContext.Session.GetString("LoginId"));
                                            cmd1.Parameters.AddWithValue("@ErrorCode", statusCode);
                                            conn1.Close();
                                            conn1.Open();
                                            cmd1.ExecuteNonQuery();




                                            string ticketupdateno = ticketno1; // Replace with your tracking number variable or value
                                            ViewData["TicketUpdateNo"] = ticketupdateno;

                                            // Ticket inserted successfully
                                            return Json(new { success = false, data = ticketupdateno, message = $"SMS not sent. Error: {message}" });


                                        }

                                    }

                                }



                            }

                            else
                            {


                            }

                            // Don't forget to close the response
                            objResponse.Close();
                        }
                        catch (WebException ex)
                        {
                            // Handle any exceptions that occurred during the HTTP request
                            Console.WriteLine($"Error occurred while sending SMS: {ex.Message}");
                            return Json(new { success = false, message = $"Error occurred while sending SMS: {ex.Message}" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid Mobile No." });
                    }



                }
                return RedirectToAction("Login", "Account");
                // Insert the record into the database using your preferred data access method (e.g., ADO.NET, Entity Framework, etc.)

                // Optionally, you can return a success response to the client

            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                return Json(new
                {
                    success = false,
                    message = "Error Updating ticket.",
                    error = ex.Message
                });
            }
        }
        public void SendEmail(string toEmail, string ccEmail, string subject, string EmailBody)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.daewoofastex.pk");

                mail.From = new MailAddress("careconnect@daewoofastex.pk");
                mail.To.Add(toEmail); // Set the 'To' email address
                mail.CC.Add("asharib.kamal@daewoo.com.pk"); // Set the 'CC' email address
                mail.CC.Add("amir.saleem@daewoo.com.pk"); // Set the 'CC' email address
                mail.IsBodyHtml = true;

                mail.Subject = subject; // Set the subject of the email

                mail.Body = EmailBody;



                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Timeout = int.MaxValue;
                //SmtpServer.Port = 25;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("careconnect@daewoofastex.pk", "Wateen@786786");
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail); // Send the email
            }
            catch (Exception ex)
            {
                throw ex;
                // Handle any exceptions that occur during the sending of the email
            }
        }



        public IActionResult Privacy()
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                return View("~/Views/Home/PrintPremium.cshtml");
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult FirstPremiumVoucher()
        {
            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
            // string policyQuery = "select PolicyNo from Insurant where NIC=@CNIC and Phone=@MobileNo";
            //string sqlquery = "select count(*) from Insurant";
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                SqlCommand command = new SqlCommand("SP_Select_Insurant_Info", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CNIC", HttpContext.Session.GetString("CNIC"));      //"36502-8724972-1"
                command.Parameters.AddWithValue("@MobileNo", HttpContext.Session.GetString("Mobile"));  //"03026928084"
                conn.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    // Policy pol = new Policy();
                    Policy1 pol = new Policy1();
                    pol.policy = dr.GetValue(0).ToString();
                    pol.iid = Convert.ToInt32(dr.GetValue(1).ToString());
                    pol.cnic = dr.GetValue(2).ToString();
                    pol.name = dr.GetValue(3).ToString();
                    pol.periodTo = DateTime.Parse(dr.GetValue(4).ToString());
                    pol.nextDue = DateTime.Parse(dr.GetValue(5).ToString());
                    pol.maturityDate = DateTime.Parse(dr.GetValue(6).ToString());
                    pol.monthlyPremium = dr.GetValue(7).ToString();
                    pol.premiumMode = dr.GetValue(8).ToString();
                    pol.policyStatus = dr.GetValue(9).ToString();
                    //pol.IID =Convert.ToInt32( dr.GetValue(0).ToString());
                    //poll = pol.policy;
                    //policy = dr.GetValue(0).ToString();
                    //policies.Add(dr.GetValue(0).ToString());  //sdr["PolicyNo"]
                    policies.Add(pol);
                }
                conn.Close();
                return View("~/Views/Home/FirstPremiumVoucherPolicies.cshtml", policies);
            }
            return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }

        public IActionResult LatePaymentVoucher()
        {

            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
            // string policyQuery = "select PolicyNo,IID from Insurant where NIC=@CNIC and Phone=@MobileNo";
            //string sqlquery = "select count(*) from Insurant";

            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                SqlCommand command = new SqlCommand("SP_Select_Insurant_Info", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CNIC", HttpContext.Session.GetString("CNIC"));      //"36502-8724972-1"
                command.Parameters.AddWithValue("@MobileNo", HttpContext.Session.GetString("Mobile"));  //"03026928084"
                conn.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    //Policy pol = new Policy();
                    Policy1 pol = new Policy1();
                    pol.policy = dr.GetValue(0).ToString();
                    pol.iid = Convert.ToInt32(dr.GetValue(1).ToString());
                    pol.cnic = dr.GetValue(2).ToString();
                    pol.name = dr.GetValue(3).ToString();
                    pol.periodTo = DateTime.Parse(dr.GetValue(4).ToString());
                    pol.nextDue = DateTime.Parse(dr.GetValue(5).ToString());
                    pol.maturityDate = DateTime.Parse(dr.GetValue(6).ToString());
                    pol.monthlyPremium = dr.GetValue(7).ToString();
                    pol.premiumMode = dr.GetValue(8).ToString();
                    pol.policyStatus = dr.GetValue(9).ToString();
                    //poll = pol.policy;
                    //policy = dr.GetValue(0).ToString();
                    //policies.Add(dr.GetValue(0).ToString());  //sdr["PolicyNo"]
                    policies.Add(pol);
                }
                conn.Close();
                return View("~/Views/Home/LatePaymentVoucherPolicies.cshtml", policies);
            }
            return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }



        public IActionResult ShortPaymentVoucher()
        {

            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
            // string policyQuery = "select PolicyNo from Insurant where NIC=@CNIC and Phone=@MobileNo";
            //string sqlquery = "select count(*) from Insurant";


            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                SqlCommand command = new SqlCommand("SP_Select_Insurant_Info", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CNIC", HttpContext.Session.GetString("CNIC"));      //"36502-8724972-1"
                command.Parameters.AddWithValue("@MobileNo", HttpContext.Session.GetString("Mobile"));  //"03026928084"
                conn.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    //Policy pol = new Policy();
                    Policy1 pol = new Policy1();
                    pol.policy = dr.GetValue(0).ToString();
                    pol.iid = Convert.ToInt32(dr.GetValue(1).ToString());
                    pol.cnic = dr.GetValue(2).ToString();
                    pol.name = dr.GetValue(3).ToString();
                    pol.periodTo = DateTime.Parse(dr.GetValue(4).ToString());
                    pol.nextDue = DateTime.Parse(dr.GetValue(5).ToString());
                    pol.maturityDate = DateTime.Parse(dr.GetValue(6).ToString());
                    pol.monthlyPremium = dr.GetValue(7).ToString();
                    pol.premiumMode = dr.GetValue(8).ToString();
                    pol.policyStatus = dr.GetValue(9).ToString();
                    //pol.IID =Convert.ToInt32( dr.GetValue(0).ToString());
                    //poll = pol.policy;
                    //policy = dr.GetValue(0).ToString();
                    //policies.Add(dr.GetValue(0).ToString());  //sdr["PolicyNo"]
                    policies.Add(pol);
                }
                conn.Close();
                return View("~/Views/Home/ShortPaymentVoucherPolicies.cshtml", policies);
            }
            return RedirectToAction("Login", "Account");
            //  return View("~/Views/Account/Login.cshtml");
        }

        public IActionResult LoanVoucherPolicies()
        {

            List<Policy1> policies = new List<Policy1>();
            SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
            // string policyQuery = "select PolicyNo from Insurant where NIC=@CNIC and Phone=@MobileNo";
            //string sqlquery = "select count(*) from Insurant";


            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                SqlCommand command = new SqlCommand("SP_Select_Insurant_Info", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CNIC", HttpContext.Session.GetString("CNIC"));      //"36502-8724972-1"
                command.Parameters.AddWithValue("@MobileNo", HttpContext.Session.GetString("Mobile"));  //"03026928084"

                conn.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    //Policy pol = new Policy();
                    Policy1 pol = new Policy1();
                    pol.policy = dr.GetValue(0).ToString();
                    pol.iid = Convert.ToInt32(dr.GetValue(1).ToString());
                    pol.cnic = dr.GetValue(2).ToString();
                    pol.name = dr.GetValue(3).ToString();
                    pol.periodTo = DateTime.Parse(dr.GetValue(4).ToString());
                    pol.nextDue = DateTime.Parse(dr.GetValue(5).ToString());
                    pol.maturityDate = DateTime.Parse(dr.GetValue(6).ToString());
                    pol.monthlyPremium = dr.GetValue(7).ToString();
                    pol.premiumMode = dr.GetValue(8).ToString();
                    pol.policyStatus = dr.GetValue(9).ToString();
                    //pol.IID =Convert.ToInt32( dr.GetValue(0).ToString());
                    //poll = pol.policy;
                    //policy = dr.GetValue(0).ToString();
                    //policies.Add(dr.GetValue(0).ToString());  //sdr["PolicyNo"]
                    policies.Add(pol);
                }
                conn.Close();
                return View("~/Views/Home/LoanVoucherPolicies.cshtml", policies);
            }
            return RedirectToAction("Login", "Account");
            //  return View("~/Views/Account/Login.cshtml");
        }
        public IActionResult BacktoDashbord()
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                return View("~/Views/Home/Menu.cshtml");
            }
            else
            {
                return RedirectToAction("Login", "Account");
                //  return View("~/Views/Account/Login.cshtml");

            }
        }

        public ActionResult generateVoucher(string option)
        {

            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                // changed by Aizaz and Imran Dated 17 Feb 22
                List<Policy> policies = new List<Policy>();
                string policy = "";
                string origion = "";
                List<PremiumVoucher> voucherlist = new List<PremiumVoucher>();
                List<LoanVoucher> loanVoucherList = new List<LoanVoucher>();
                int iid = (int)HttpContext.Session.GetInt32("iid");
                SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
                string policyQuery = "select PolicyNo,Origin from Insurant where IID=@iid";
                //string sqlquery = "select count(*) from Insurant";
                conn.Open();
                SqlCommand command = new SqlCommand(policyQuery, conn);
                command.Parameters.AddWithValue("@iid", (int)HttpContext.Session.GetInt32("iid"));      //"36502-8724972-1"
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    policy = dr.GetValue(0).ToString();
                    origion = dr.GetValue(1).ToString();
                }
                if (option == "All_Voucher")
                {
                    string dd = _db.GenerateVouchar(policy, iid);
                    ViewBag.sucess = dd;
                    voucherlist = _db.GetAllVouchers(policy).ToList();
                    ViewBag.message = "Voucher not found against policy no: ";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/Index.cshtml", voucherlist);
                }
                else if (option == "Late_Payment_Voucher")
                {
                    string dd = _db.GenerateLPVouchar(policy, iid);
                    ViewBag.sucess = dd;
                    voucherlist = _db.GetAllLatePaymentVouchers(policy).ToList();
                    ViewBag.message = "Voucher not found against policy no: ";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/LatePayment.cshtml", voucherlist);
                }
                else if (option == "Short_Payment_Voucher")
                {
                    string dd = _db.GenerateSPVouchar(policy, iid);
                    ViewBag.sucess = dd;
                    voucherlist = _db.GetAllShortPaymentVouchers(policy).ToList();
                    ViewBag.message = "Voucher not found against policy no: ";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/ShortPayment.cshtml", voucherlist);
                }
                else if (option == "First_Payment_Voucher")
                {
                    string dd = _db.GenerateVouchar(policy, iid);
                    voucherlist = _db.GetAllFirstPremiumVouchers(policy).ToList();
                    ViewBag.message = "Voucher not found against policy no: ";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/FirstPremium.cshtml", voucherlist);
                }
                else if (option == "Loan_Voucher")
                {
                    string dd = _db.GenerateLoanVouchar(iid, origion);
                    ViewBag.sucess = dd;
                    loanVoucherList = _db.GetLoanVouchers(iid).ToList();
                    ViewBag.message = "NotSanctioned";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/LoanVoucher.cshtml", loanVoucherList);
                }
                return RedirectToAction("Menu", "Home");
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }

        public ActionResult RegenerateVoucher(string option, string voucherno)
        {

            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                // changed by Aizaz and Imran Dated 17 Feb 22
                List<Policy> policies = new List<Policy>();
                string policy = "";
                List<PremiumVoucher> voucherlist = new List<PremiumVoucher>();
                int iid = (int)HttpContext.Session.GetInt32("iid");
                SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
                string policyQuery = "select PolicyNo from Insurant where IID=@iid";
                //string sqlquery = "select count(*) from Insurant";
                conn.Open();
                SqlCommand command = new SqlCommand(policyQuery, conn);
                command.Parameters.AddWithValue("@iid", (int)HttpContext.Session.GetInt32("iid"));      //"36502-8724972-1"
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    policy = dr.GetValue(0).ToString();
                }
                if (option == "All_Voucher")
                {
                    string dd = _db.RegenerateVouchar(policy, iid, option, voucherno);
                    ViewBag.sucess = dd;
                    voucherlist = _db.GetAllVouchers(policy).ToList();
                    ViewBag.message = "Voucher not found against policy no: ";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/Index.cshtml", voucherlist);
                }
                else if (option == "Late_Payment_Voucher")
                {
                    string dd = _db.RegenerateVouchar(policy, iid, option, voucherno);
                    ViewBag.sucess = dd;
                    voucherlist = _db.GetAllLatePaymentVouchers(policy).ToList();
                    ViewBag.message = "Voucher not found against policy no: ";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/LatePayment.cshtml", voucherlist);
                }
                else if (option == "Short_Payment_Voucher")
                {
                    string dd = _db.RegenerateVouchar(policy, iid, option, voucherno);
                    ViewBag.sucess = dd;
                    voucherlist = _db.GetAllShortPaymentVouchers(policy).ToList();
                    ViewBag.message = "Voucher not found against policy no: ";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/ShortPayment.cshtml", voucherlist);
                }
                else if (option == "First_Payment_Voucher")
                {
                    string dd = _db.GenerateVouchar(policy, iid);
                    voucherlist = _db.GetAllFirstPremiumVouchers(policy).ToList();
                    ViewBag.message = "Voucher not found against policy no: ";
                    ViewBag.pol = policy;
                    return View("~/Views/Home/FirstPremium.cshtml", voucherlist);
                }
                return RedirectToAction("Menu", "Home");
            }
            else
                return RedirectToAction("Login", "Account");
            // return View("~/Views/Account/Login.cshtml");
        }

        public ActionResult GetPostComplaintList()
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                string cnicNo = HttpContext.Session.GetString("CNIC");
                List<CustomerPostComplaints> customerPostComplaints = new List<CustomerPostComplaints>();
                customerPostComplaints = _db.GetPostComplaintList(cnicNo);
                return View("~/Views/Home/CustomerComplaintsList.cshtml", customerPostComplaints);
            }
            return RedirectToAction("Login", "Account");
            //return View("~/Views/Account/Login.cshtml");
        }

        public ActionResult SetCustomerComplaintStatus(int complaintId)
        {
            if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
            {
                string cnicNo = HttpContext.Session.GetString("CNIC");
                List<CustomerPostComplaints> customerPostComplaints = new List<CustomerPostComplaints>();
                string res = _db.SetCustomerComplaintStatus(complaintId);
                ViewBag.message = res;
                customerPostComplaints = _db.GetPostComplaintList(cnicNo);
                return View("~/Views/Home/CustomerComplaintsList.cshtml", customerPostComplaints);
            }
            return RedirectToAction("Login", "Account");
        }

        //-----------------------------ReopenComplaint------------------------------
        public IActionResult ReopenComplaint(CustomerPostComplaints customerPostComplaints)
        {
            try
            {
                if (HttpContext.Session.GetString("CNIC") != "" && HttpContext.Session.GetString("CNIC") != null)
                {

                    string resp = _db.ReopenComplaint(customerPostComplaints);

                    return Json("success");
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {

                throw;
            }

        }
    }

    internal class UserModel
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
    }
}
