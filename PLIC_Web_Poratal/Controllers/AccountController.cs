using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using PLIC_Web_Poratal.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace PLIC_Web_Poratal.Controllers
{
    public class AccountController : Controller
    {
        private readonly INotyfService _notyf;

        public AccountController(INotyfService notyf)
        {
            _notyf = notyf;
        }

        db _db = new db();
        EncDec encDec = new EncDec();
        string cnic;
        string mobile;
        string password;
        string sqlquery = "select top 1 LoginId,Password from CRM_SYS_Users where LoginId=@LoginId and Password=@Password";
        //List<Policy> policies = new List<Policy>();
        //public static OTP otp = new OTP();
        string otp;
        db db = new db();
        //string connection = db.GetConfiguration().GetConnectionString("DefaultConnection");
        //private IConfiguration Configuration;
        //SqlConnection conn = new SqlConnection(db.GetConfiguration().GetConnectionString("DefaultConnection"));
        //SqlCommand comm = new SqlCommand();
        //SqlDataReader sdr;
        //-----------------------CustomerSignUp-------------------------------------------
        public IActionResult CustomerSignUp()
        {
            return View();

        }

        //-----------------------AdminSignUp-------------------------------------------
        public IActionResult AdminSignUp()
        {
            return View();

        }
        //----------------------Login----------------------------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }

        //----------------------AdminLogin-------------------------------------------
        [HttpGet]
        public IActionResult Admin()
        {

            return View();
        }

        //-----------------------------CustomerSignUp----------------------------------
        [HttpPost]
        public IActionResult CustomerSignUp(CustomerSignUp customerSignUp)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    string cnicNo = customerSignUp.CNIC;
                    string mobileNo = customerSignUp.MobileNo;

                    //if (acc.MobileNo.Length != null)
                    if (mobileNo.Length == 11 && cnicNo.Length == 15)
                    {

                        SqlConnection conn = new SqlConnection(db.GetConfiguration().GetConnectionString("DefaultConnection"));
                        //string sqlquery = "select top 1 NIC,Phone from Insurant where NIC=@CNIC and Phone=@MobileNo";
                        string sqlquery = "select top 1 NIC,Phone from Insurant where NIC='" + cnicNo + "' and Phone = '" + mobileNo + "'";

                        conn.Close();
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sqlquery, conn);
                        SqlDataReader sdr = comm.ExecuteReader();

                        while (sdr.Read())
                        {
                            HttpContext.Session.SetString("Customer_CNIC", cnicNo.Trim());
                            HttpContext.Session.SetString("CustomerMobile", mobileNo.Trim());
                        }

                        if (!sdr.HasRows)
                        {
                            conn.Close();
                            SqlConnection conn1 = new SqlConnection(db.GetConfiguration().GetConnectionString("DefaultConnection"));

                            string sqlquery1 = "select top 1 NIC,Phone from Insurant where NIC='" + cnicNo + "'";
                            SqlCommand comm1 = new SqlCommand(sqlquery1, conn1);
                            conn1.Open();
                            SqlDataReader sdr1 = comm1.ExecuteReader();
                            while (sdr1.Read())
                            {
                                string mobile = sdr1.GetValue(1).ToString();
                            }
                            if (!sdr1.HasRows)
                            {
                                conn1.Close();
                                ViewBag.message = "CNIC";
                                //ModelState.Clear();
                                return View();
                            }
                            else if (mobile != mobileNo)
                            {
                                conn1.Close();
                                HttpContext.Session.SetString("CNICno", cnicNo.Trim());
                                ViewBag.message = "MobileNo";
                                //ModelState.Clear();
                                return View();
                            }
                            ViewBag.message = "InCorrectFormate";
                            //ModelState.Clear();
                            return View();
                        }
                        else
                        {
                            conn.Close();

                            otp = GenerateOTP();

                            HttpContext.Session.SetString("OTP", otp);
                            string phone = HttpContext.Session.GetString("CustomerMobile");
                            //string phone = "03435847729";
                            string mobilenumber = "92" + phone.Substring(phone.Length - 10);
                            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "Your OTP For PLICL Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PCLSDKJPISWUWDHDPLIC&password=b63b122c528f54df4a0446b6bab05515&sender=8576&apikey=1094cbd4841bb89b173098af5bed0e60&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "Your OTP For PLIC Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");

                            request.Method = "GET";

                            WebResponse response = request.GetResponse();
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                var ApiStatus = reader.ReadToEnd();
                                if (ApiStatus == "Your Message is sent, MSISDN 1, Cost 1")
                                {
                                    _db.SmsLog(phone, 1, "OTP for web portal");
                                    ModelState.Clear();
                                    _notyf.Success("OTP Is Sended to Your Number Successfully!");
                                    return RedirectToAction("OTP", "Account");
                                }
                                else
                                {
                                    ViewBag.message = "Error";
                                    return View();
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.message = "InCorrectFormate";
                        return View();
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }



        //-----------------------------Customer Registration----------------------------------
        [HttpPost]
        public IActionResult CustomerRegistration(CustomerPassword customerPassword)
        {

            try
            {
                if (HttpContext.Session.GetString("Customer_NIC") != "" && HttpContext.Session.GetString("Customer_NIC") != null)
                {
                    string password = encDec.EncodePassword(customerPassword.Password);
                    if (ModelState.IsValid)
                    {
                        DataSet ds = new DataSet();
                        string cnicNo = HttpContext.Session.GetString("Customer_CNIC");
                        string mobileNo = HttpContext.Session.GetString("CustomerMobile");
                        //-----Checking Duplication------------

                        string sqlquery = "select top 1 CNIC from WebPortalCustomerLogin where CNIC='" + cnicNo + "' and PhoneNo = '" + mobileNo + "'";
                        using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                        {
                            SqlCommand cmd = new SqlCommand(sqlquery, conn);
                            conn.Open();
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                sda.Fill(ds);
                            }
                            conn.Close();
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                            {
                                SqlCommand cmd = new SqlCommand("Update WebPortalCustomerLogin set Password='" + password + "'  where CNIC='" + cnicNo + "'and PhoneNo = '" + mobileNo + "'", conn); /* and VoucherNumber='"+ voucharno +"'*/
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                            ViewBag.message = "Update";
                            return View();
                        }
                        else
                        {
                            string res = _db.InsertCustomerLoginData(cnicNo, mobileNo, password);
                            ViewBag.message = res;
                            return View();
                        }
                    }
                    return View();
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {

                throw;
            }
        }

        //--------------------------------Verify Customer OTP--------------------------------------------
        [HttpPost]
        public IActionResult VerifyCustomerOTP(OTP code, string value)
        {
            //HttpContext.Session.SetString("CustomerNIC", HttpContext.Session.GetString("CustomerCNIC"));
            //return View("~/Views/Account/CustomerRegistration.cshtml");
            if (code.otp == HttpContext.Session.GetString("OTP"))
            {
                if (HttpContext.Session.GetString("Customer_CNIC") != "" && HttpContext.Session.GetString("Customer_CNIC") != null)
                {
                    HttpContext.Session.SetString("Customer_NIC", HttpContext.Session.GetString("Customer_CNIC"));

                    return View("~/Views/Account/CustomerRegistration.cshtml");
                }
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError("Wrong OTP", "Wrong Code. Please enter correct code again.");
                return View("~/Views/Account/OTP.cshtml", otp);
            }
        }

        //----------------------Admin Login-------------------------------------------
        [HttpPost]
        public IActionResult Admin(AdminLogin adminLogin)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (adminLogin.Password.Length >= 8)
                    {
                        DataSet ds = new DataSet();
                        int userId;
                        string userName;
                        string phoneNo;
                        int circleId;
                        int regionId;
                        int fieldUnitId;
                        int groupId;
                        string password;
                        int sysadmin;

                        using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                        {
                            SqlCommand cmd = new SqlCommand("select USERID,UserName,PhoneNo,isnull(CircleId,0),isnull(RegionId,0),isnull(FieldUnitId,0),isnull(GroupId,0),isnull(Password,''),isnull(SuperAdmin,0) from WebPortalAdminLogin where UserName = '" + adminLogin.UserName.Trim() + "' and IsActive=1", conn);
                            //cmd.Parameters.AddWithValue("IID", iid);
                            conn.Open();
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                sda.Fill(ds);
                            }
                            conn.Close();
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            userId = Convert.ToInt32(dt.Rows[0][0]);
                            userName = dt.Rows[0][1].ToString();
                            phoneNo = dt.Rows[0][2].ToString();
                            circleId = Convert.ToInt32(dt.Rows[0][3]);
                            regionId = Convert.ToInt32(dt.Rows[0][4]);
                            fieldUnitId = Convert.ToInt32(dt.Rows[0][5]);
                            groupId = Convert.ToInt32(dt.Rows[0][6]);
                            password = encDec.DecodePassword(dt.Rows[0][7].ToString());
                            sysadmin = Convert.ToInt32(dt.Rows[0][8]);

                            if (adminLogin.Password == password)
                            {
                                HttpContext.Session.SetInt32("AdminId", userId);
                                HttpContext.Session.SetString("UserName", userName);
                                HttpContext.Session.SetString("PhoneNo", phoneNo);
                                HttpContext.Session.SetInt32("CircleId", circleId);
                                HttpContext.Session.SetInt32("RegionId", regionId);
                                HttpContext.Session.SetInt32("FieldUnitId", fieldUnitId);
                                HttpContext.Session.SetInt32("GroupId", groupId);
                                HttpContext.Session.SetInt32("Sysadmin", sysadmin);

                                HttpContext.Session.SetString("AdminName", HttpContext.Session.GetString("UserName"));
                                ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                                ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                                AdminLoggingHistory(userId);
                                ModelState.Clear();
                                return RedirectToAction("AdminDashboard", "Admin");
                            }
                            else
                            {
                                ViewBag.message = "failed";
                                ModelState.Clear();
                                return View();
                            }

                        }
                        //else if (ds.Tables[0].Rows.Count == 0)
                        //{
                        //    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                        //    {
                        //        SqlCommand cmd = new SqlCommand("select Password from WebPortalAdminLogin where UserName = '" + adminLogin.UserName.Trim() + "' and IsActive=1", conn);
                        //        //cmd.Parameters.AddWithValue("IID", iid);
                        //        conn.Open();
                        //        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        //        {
                        //            sda.Fill(ds);
                        //        }
                        //        conn.Close();
                        //    }
                        //    if (ds.Tables[0].Rows.Count > 0)
                        //    {
                        //        DataTable dt = ds.Tables[0];
                        //        string password = dt.Rows[0][0].ToString();
                        //        if (password != adminLogin.Password)
                        //        {
                        //            ViewBag.message = "failed";
                        //            ModelState.Clear();
                        //            return View();
                        //        }
                        //    }
                        //}
                        _notyf.Information("Your username and password is incorrect.");
                        return View();
                    }
                    _notyf.Information("Please enter correct password.");
                    return View();
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //----------------------Admin SignUp-------------------------------------------
        [HttpPost]
        public IActionResult AdminSignUp(AdminSignUp adminSignUp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (adminSignUp.PhoneNo.Length == 11)
                    {
                        DataSet ds = new DataSet();
                        int userId;
                        string userName;
                        string phoneNo;

                        using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                        {
                            SqlCommand cmd = new SqlCommand("select USERID,UserName,PhoneNo from WebPortalAdminLogin where UserName = '" + adminSignUp.UserName.Trim() + "' and PhoneNo = '" + adminSignUp.PhoneNo + "' and IsActive=1", conn);
                            //cmd.Parameters.AddWithValue("IID", iid);
                            conn.Open();
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                sda.Fill(ds);
                            }
                            conn.Close();
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            userId = Convert.ToInt32(dt.Rows[0][0]);
                            userName = dt.Rows[0][1].ToString();
                            phoneNo = dt.Rows[0][2].ToString();
                            HttpContext.Session.SetInt32("AdminId", userId);
                            HttpContext.Session.SetString("User_Name", userName);
                            HttpContext.Session.SetString("PhoneNo", phoneNo);

                            otp = GenerateOTP();

                            HttpContext.Session.SetString("AdminOTP", otp);
                            string phone = HttpContext.Session.GetString("PhoneNo");
                            string mobilenumber = "92" + phone.Substring(phone.Length - 10);
                            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + " Your OTP For PLIC WebPortal Login is:  " + HttpContext.Session.GetString("AdminOTP") + "");
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PCLSDKJPISWUWDHDPLIC&password=b63b122c528f54df4a0446b6bab05515&sender=8576&apikey=1094cbd4841bb89b173098af5bed0e60&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + " Your OTP For PLIC WebPortal Login is:  " + HttpContext.Session.GetString("AdminOTP") + "");

                            request.Method = "GET";

                            WebResponse response = request.GetResponse();
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                var ApiStatus = reader.ReadToEnd();
                                if (ApiStatus == "Your Message is sent, MSISDN 1, Cost 1")
                                {
                                    _db.SmsLog(phone, 1, "OTP for web portal");
                                    ModelState.Clear();
                                    _notyf.Success("OTP Is Sended to Your Number Successfully!");
                                    return View("~/Views/Account/AdminOTP.cshtml");
                                }
                                else
                                {
                                    _notyf.Error("Oop! Some things went wrong.");
                                    return View();
                                }
                            }
                        }
                        _notyf.Information("UserName Or Mobile number is incorrect or do no exist.");
                        ModelState.Clear();
                        return View();
                    }
                    _notyf.Information("Please Enter Mobile number in correct formate.");
                    return View();
                }
                _notyf.Information("Please enter user name and mobile number.");
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-----------------------------AdminPassword----------------------------------
        [HttpPost]
        public IActionResult AdminPassword(CustomerPassword customerPassword)
        {

            try
            {
                if (HttpContext.Session.GetString("Admin_Name") != "" && HttpContext.Session.GetString("Admin_Name") != null)
                {
                    string password = encDec.EncodePassword(customerPassword.Password);
                    if (ModelState.IsValid)
                    {
                        DataSet ds = new DataSet();
                        string userName = HttpContext.Session.GetString("User_Name");
                        string mobileNo = HttpContext.Session.GetString("PhoneNo");
                        //-----Checking Duplication------------

                        string sqlquery = "select top 1 UserName from WebPortalAdminLogin where UserName='" + userName + "' and PhoneNo = '" + mobileNo + "'";
                        using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                        {
                            SqlCommand cmd = new SqlCommand(sqlquery, conn);
                            conn.Open();
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                sda.Fill(ds);
                            }
                            conn.Close();
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                            {
                                SqlCommand cmd = new SqlCommand("Update WebPortalAdminLogin set Password='" + password + "'  where UserName='" + userName + "'and PhoneNo = '" + mobileNo + "'", conn); /* and VoucherNumber='"+ voucharno +"'*/
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                            ViewBag.message = "Update";
                            return View();
                        }
                    }
                    return View();
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {

                throw;
            }
        }

        //---------------------- AdminOTP Verification--------------------------------
        [HttpPost]
        public IActionResult AdminOTPVerification(OTP otpCode)
        {
            //HttpContext.Session.SetString("AdminName", HttpContext.Session.GetString("UserName"));
            //return View("~/Views/Account/AdminPassword.cshtml");
            if (otpCode.otp == HttpContext.Session.GetString("AdminOTP"))
            {
                if (HttpContext.Session.GetString("User_Name") != "" && HttpContext.Session.GetString("User_Name") != null)
                {
                    int adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    HttpContext.Session.SetString("Admin_Name", HttpContext.Session.GetString("User_Name"));

                    return View("~/Views/Account/AdminPassword.cshtml");

                }
                return RedirectToAction("Admin", "Account");
            }
            else
            {
                _notyf.Error("OTP Code is Incorrect.");
                return View("~/Views/Account/AdminOTP.cshtml");
            }
        }

        //-----------------------Resend Admin OTP--------------------------------------
        public ActionResult ResendAdminOTP()
        {
            otp = GenerateOTP();

            //TempData["OTP"] = otp.otp;
            HttpContext.Session.SetString("AdminOTP", otp);
            string phone = HttpContext.Session.GetString("PhoneNo");
            string mobilenumber = "92" + phone.Substring(phone.Length - 10);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "Your OTP For PLIC WebPortal Login is:  " + HttpContext.Session.GetString("AdminOTP") + "");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PCLSDKJPISWUWDHDPLIC&password=b63b122c528f54df4a0446b6bab05515&sender=8576&apikey=1094cbd4841bb89b173098af5bed0e60&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "Your OTP For PLIC WebPortal Login is:  " + HttpContext.Session.GetString("AdminOTP") + "");

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://brandyourtext.com/sms/api/send?username=Carbon8&password=786786&mask=Alert Pak&mobile=" + HttpContext.Session.GetString("Mobile") + "&message="+ "Your OTP For PLICL Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://brandyourtext.com/sms/api/send?username=Carbon8&password=786786&mask=Alert Pak&mobile=" + HttpContext.Session.GetString("Mobile") + "&message=" + HttpContext.Session.GetString("OTP") + "");
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var ApiStatus = reader.ReadToEnd();
                if (ApiStatus == "Your Message is sent, MSISDN 1, Cost 1")
                {
                    //_notyf.Success("Success Notification");
                    _db.SmsLog(phone, 1, "OTP for web portal");
                    _notyf.Success("OTP Is Re-Sended to Your Number Successfully!");
                    return View("~/Views/Account/AdminOTP.cshtml");
                }
                else
                {
                    ModelState.AddModelError("NotFound", "Could not send the OTP. Please make sure your mobile number is in the correct format. i-e 03001111111 (without '-')");
                    return View("Login");
                }
            }
        }

        //-----------------------Admin Logout-------------------------------------------
        public IActionResult AdminLogout()
        {
            int adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
            AdminLogOutHistory(adminId);
            HttpContext.Session.Clear();
            return RedirectToAction("Admin", "Account");
        }

        //-----------------------Login2-------------------------------------------------
        [HttpGet]
        public IActionResult Login2()
        {
            return RedirectToAction("Login", "Account");

        }

        //------------------------Logout------------------------------------------------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        //--------------------------OTP--------------------------------------------------
        [HttpGet]
        public IActionResult OTP()
        {
            return View();

        }

        //----------------------------PreLogin Complaint----------------------------------
        [HttpGet]
        public IActionResult PreLoginComplaint()
        {
            if (HttpContext.Session.GetString("CNICno") != null && HttpContext.Session.GetString("CNICno") != "")
            {
                PreLoginComplaint preLoginComplaint = new PreLoginComplaint();
                List<Region> regions = new List<Region>();
                preLoginComplaint.cnic = HttpContext.Session.GetString("CNICno");
                preLoginComplaint.regions = _db.GetRegions();
                return View(preLoginComplaint);
            }
            return RedirectToAction("Login", "Account");
        }

        //-----------------------------PreLogin Compliant----------------------------------
        [HttpPost]
        public IActionResult PreLoginComplaint(PreLoginComplaint preLoginComplaint)
        {
            List<Region> regions = new List<Region>();
            try
            {
                if (ModelState.IsValid)
                {
                    if (preLoginComplaint.cnic.Length == 15 && preLoginComplaint.mobileNumber.Length == 11)
                    {
                        //-------------------------------------------------------------------------------------------
                        DataSet ds = new DataSet();

                        //-----Checking Duplication------------

                        string sqlquery = "select top 1 NIC,Phone from Insurant where NIC='" + preLoginComplaint.cnic + "' and Phone = '" + preLoginComplaint.mobileNumber + "'";
                        using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                        {
                            SqlCommand cmd = new SqlCommand(sqlquery, conn);
                            conn.Open();
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                sda.Fill(ds);
                            }
                            conn.Close();
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.message = "DataExist";
                            //PreLoginComplaint preLoginComplaint = new PreLoginComplaint();

                            preLoginComplaint.regions = _db.GetRegions();
                            return View(preLoginComplaint);
                        }
                        //-------------------------------------------------------------------------------------------
                        else
                        {
                            //--------------------------------Checking Duplication in PortalPreLoginComplaint table------------------------------------------
                            string sqlqry = "select top 1 CNIC,MobileNo from PortalPreLoginComplaint where CNIC='" + preLoginComplaint.cnic + "' and MobileNo = '" + preLoginComplaint.mobileNumber + "'";
                            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                            {
                                SqlCommand cmd = new SqlCommand(sqlqry, conn);
                                conn.Open();
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    sda.Fill(ds);
                                }
                                conn.Close();
                            }
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                ViewBag.message = "ComplaintExist";
                                //PreLoginComplaint preLoginComplaint = new PreLoginComplaint();

                                preLoginComplaint.regions = _db.GetRegions();
                                return View(preLoginComplaint);
                            }
                            else
                            {
                                string resp = _db.insertPreLoginComplaint(preLoginComplaint);

                                string phone = preLoginComplaint.mobileNumber;
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
                            }
                        }
                    }
                    else
                    {
                        ViewBag.message = "Faild";
                    }
                    preLoginComplaint.regions = _db.GetRegions();
                    return View(preLoginComplaint);
                }
                preLoginComplaint.regions = _db.GetRegions();
                return View(preLoginComplaint);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-----------------------------Generate OTP-----------------------------------------
        public string GenerateOTP()
        {
            string _numbers = "0123456789";
            Random random = new Random();
            StringBuilder builder = new StringBuilder(6);
            string numberAsString = "";
            //int numberAsNumber = 0;

            for (var i = 0; i < 6; i++)
            {
                builder.Append(_numbers[random.Next(0, _numbers.Length)]);
            }

            numberAsString = builder.ToString();
            //numberAsNumber = int.Parse(numberAsString);
            return numberAsString;
        }

        //------------------------------Resend OTP-------------------------------------------
        public ActionResult ResendOTP()
        {
            otp = GenerateOTP();

            //TempData["OTP"] = otp.otp;
            HttpContext.Session.SetString("OTP", otp);
            string phone = HttpContext.Session.GetString("CustomerMobile");
            string mobilenumber = "92" + phone.Substring(phone.Length - 10);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "Your OTP For PLIC Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PCLSDKJPISWUWDHDPLIC&password=b63b122c528f54df4a0446b6bab05515&sender=8576&apikey=1094cbd4841bb89b173098af5bed0e60&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "Your OTP For PLIC Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://brandyourtext.com/sms/api/send?username=Carbon8&password=786786&mask=Alert Pak&mobile=" + HttpContext.Session.GetString("Mobile") + "&message="+ "Your OTP For PLICL Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://brandyourtext.com/sms/api/send?username=Carbon8&password=786786&mask=Alert Pak&mobile=" + HttpContext.Session.GetString("Mobile") + "&message=" + HttpContext.Session.GetString("OTP") + "");
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var ApiStatus = reader.ReadToEnd();
                if (ApiStatus == "Your Message is sent, MSISDN 1, Cost 1")
                {
                    //_notyf.Success("Success Notification");
                    _db.SmsLog(phone, 1, "OTP for web portal");
                    _notyf.Success("OTP Is Re-Sended to Your Number Successfully!");
                    return View("~/Views/Account/OTP.cshtml");
                }
                else
                {
                    ModelState.AddModelError("NotFound", "Could not send the OTP. Please make sure your mobile number is in the correct format. i-e 03001111111 (without '-')");
                    return View("Login");
                }
            }
        }

        //------------------------------Get OTP-----------------------------------------------
       // [HttpGet]
        public string GetOtp(string ID)
        {

            string LoginId;
            string UserName;
            string password;
            string RoleID;
            string[] dataID = ID.Split('ǁ');
            //return "/Account/OTP";

            Account acc = new Account { UserName = dataID[0], Password = dataID[1] };

            if (acc.UserName == null)
            {
                acc.UserName = " ";
            }
            if (acc.Password == null)
            {
                acc.Password = " ";
            }

            //if (acc.MobileNo.Length != null)
            if (acc.Password.Length >= 3 && acc.UserName.Length > 3)
            {
                string connectionString = db.GetConfiguration().GetConnectionString("DefaultConnection");
                string storedProcedureName = "GetUserByLoginId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (SqlCommand comm = new SqlCommand(storedProcedureName, conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the stored procedure
                        comm.Parameters.AddWithValue("@LoginId", acc.UserName);

                        using (SqlDataReader sdr = comm.ExecuteReader())
                        {
                            if (sdr.Read())
                            {
                                LoginId = sdr["userid"].ToString();
                                UserName = sdr["LoginId"].ToString();
                                password = sdr["Password"].ToString();
                                RoleID = sdr["role_id"].ToString();

                                if (password == acc.Password)
                                {
                                    HttpContext.Session.SetString("LoginId", LoginId);
                                    HttpContext.Session.SetString("UserName", UserName);
                                    HttpContext.Session.SetString("Password", acc.Password);
                                    HttpContext.Session.SetString("RoleID", RoleID);

                                    ViewData["RoleID"] = RoleID;
                                    ViewBag.Role = RoleID;

                                    var viewModel = new CareConnect.Models.UserViewModel
                                    {
                                        LoginId = LoginId,
                                        UserName = UserName,
                                        Password = password,
                                        RoleID = RoleID
                                    };

                                    ViewBag.RoleID = HttpContext.Session.GetString("RoleID");
                                    LoggingHistory(LoginId, RoleID, password);
                                    return "/Home/Menu";
                                }
                                else
                                {
                                    return "Password";
                                }
                            }
                            else
                            {
                                return "LoginId"; // No matching user found
                            }
                        }
                    }
                }



                //string connectionString = db.GetConfiguration().GetConnectionString("DefaultConnection");
                //string sqlquery = "select top 1 userid,LoginId,Password,role_id from CRM_SYS_Users where LoginId='" + acc.UserName + "'";

                //using (SqlConnection conn = new SqlConnection(connectionString))
                //{
                //    if (conn.State != ConnectionState.Open)
                //        conn.Open();

                //    using (SqlCommand comm = new SqlCommand(sqlquery, conn))
                //    {
                //        using (SqlDataReader sdr = comm.ExecuteReader())
                //        {
                //            while (sdr.Read())
                //            {
                //                LoginId = sdr.GetValue(0).ToString();
                //                UserName = sdr.GetValue(1).ToString();
                //                password = sdr.GetValue(2).ToString();
                //                RoleID = sdr.GetValue(3).ToString();

                //                if (password == acc.Password)
                //                {
                //                    HttpContext.Session.SetString("LoginId", LoginId);
                //                    HttpContext.Session.SetString("UserName", acc.UserName);
                //                    HttpContext.Session.SetString("Password", acc.Password);
                //                    HttpContext.Session.SetString("RoleID", RoleID);

                //                    if (!sdr.HasRows)
                //                    {
                //                        return "LoginId";
                //                    }
                //                    else
                //                    {
                //                        if (password == acc.Password)
                //                        {
                //                            HttpContext.Session.SetString("LoginId", HttpContext.Session.GetString("LoginId"));
                //                            LoginId = HttpContext.Session.GetString("LoginId");
                //                            UserName = HttpContext.Session.GetString("UserName");
                //                            password = HttpContext.Session.GetString("Password");
                //                            RoleID = HttpContext.Session.GetString("RoleID");
                //                            ViewData["RoleID"] = RoleID;
                //                            ViewBag.Role = RoleID;

                //                            var viewModel = new CareConnect.Models.UserViewModel
                //                            {
                //                                LoginId = LoginId,
                //                                UserName = UserName,
                //                                Password = password,
                //                                RoleID = RoleID
                //                            };

                //                            // TempData["UserViewModel"] = viewModel;
                //                             ViewBag.RoleID = HttpContext.Session.GetString("RoleID");
                //                            LoggingHistory(LoginId, RoleID, password);
                //                            return "/Home/Menu";
                //                        }
                //                        return "Password";
                //                    }

                //                }

                //            }
                //            return "Password";
                //        }

                //    }

                //}

            }
            else
            {
                return "";
            }
        }


        [HttpPost]
        public IActionResult Verify(Account acc)
        {

            if (acc.LoginId == null)
            {
                acc.LoginId = " ";
            }
            if (acc.Password == null)
            {
                acc.Password = " ";
            }

            //if (acc.MobileNo.Length != null)
            if (acc.Password.Length == 11)
            {

                SqlConnection conn = new SqlConnection(db.GetConfiguration().GetConnectionString("DefaultConnection"));
                string sqlquery = "select top 1 NIC,Phone from Insurant where NIC=@CNIC and Phone=@MobileNo";
                //string sqlquery = "select NIC, Phone from Insurant where NIC=@CNIC and Phone=@MobileNo ";
                conn.Open();
                SqlCommand comm = new SqlCommand(sqlquery, conn);
                comm.Parameters.AddWithValue("@CNIC", acc.LoginId);      //"36502-8724972-1"
                comm.Parameters.AddWithValue("@MobileNo", acc.Password);  //"03026928084"
                SqlDataReader sdr = comm.ExecuteReader();

                while (sdr.Read())
                {
                    //CNIC = sdr.GetValue(0).ToString();
                    //Mob = sdr.GetValue(1).ToString();
                    //CNIC = CNIC.TrimEnd();
                    //policy.Add(sdr["PolicyNo"].ToString());

                    // HttpContext.Session.SetString("CNIC", acc.CNIC.Trim());
                    HttpContext.Session.SetString("NIC", acc.LoginId.Trim());
                    HttpContext.Session.SetString("Mobile", acc.Password.Trim());
                }

                //var count = (int)comm.ExecuteScalar();
                //ViewData["Total Data"] = count;

                //if(sdr.Read())
                //{

                //}

                if (!sdr.HasRows)
                {
                    //ViewBag.Message = string.Format("Record Not Found. Please enter correct credentials. If Error remains contact your nearby PLIC branch");
                    //ModelState.AddModelError(nameof(Account.CNIC), "Please Enter Correct CNIC");
                    //ModelState.AddModelError(nameof(Account.MobileNo), "Please Enter Correct Mobile Number");
                    ModelState.AddModelError("NotFound", "Record Not Found. Please enter correct credentials. If Error remains contact your nearby PLIC branch");
                    conn.Close();
                    return View("Login");
                }
                else
                {
                    conn.Close();
                    //----------------------------------------------------------------------------------------
                    //                Private Sub cmdAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAdd.Click
                    //    'AddInsurant()

                    //    'If Len(txtPhoneNo.Text) = 11 And Microsoft.VisualBasic.Left(Convert.ToString(txtPhoneNo.Text), 2) = "03" Then
                    //    Dim strCommand As String
                    //    strCommand = "INSERT into smsLog (IID, PhoneNo, smsType, Text, EntryDate) Values (" & IID & ",'" & txtPhoneNo.Text & "'," & 1 & ",'" & "Thank you for choosing Postal Life Insurrance." & "','" & GetServerDateTime() & "')"
                    //    'executeQuery(strCommand, CommandType.Text)
                    //    'Dim webClient As New System.Net.WebClient
                    //    'Dim m As Match = Regex.Match(webClient.DownloadString("http://icworldsms.com:82/Service.asmx/Login?UserName=PLI&Password=pli123456&ClientID=839890"), "(?<=<SessionID>)(.*?)(?=</SessionID>)")
                    //    'If m.Success Then
                    //    '    Dim result As String = webClient.DownloadString("http://icworldsms.com:82/Service.asmx/SendSMS?SessionID=" + m.Groups(1).Value.ToString + "&CompaignName=Insurant&MobileNo=" + txtPhoneNo.Text + "&MaskName=PLI&Message=Thank you for choosing Postal Life Insurrance.&MessageType=English")
                    //    'Else
                    //    '    Dim result As String = webClient.DownloadString("http://221.132.117.58:7700/sendsms_url.html?Username=03028501395&Password=123.123&From=PLI&To=" + txtPhoneNo.Text + "&Message=Thank you for choosing Postal Life Insurrance.")
                    //    'End If
                    //    ServicePointManager.Expect100Continue = True
                    //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                    //        Dim message As String = "Thank you for choosing Postal Life Insurrance."
                    //    'Dim request As HttpWebRequest = WebRequest.Create("https://brandyourtext.com/sms/api/send?username=Pli&password=786786&mask=PLI&mobile=" + txtPhoneNo.Text.ToString() + "&message=" + message.ToString() + "")
                    //    Dim phone As String = "03138665908"
                    //    'Dim mobilenumber As String = phone.Substring(phone.Length - 10)
                    //    Dim mobilenumber As String = "92" & phone.Substring(phone.Length - 10)
                    //    Dim request As HttpWebRequest = WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + message.ToString() + "");
                    //    'Dim request As HttpWebRequest = WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&phone=923485100157&type=English&message=MESSAGE")

                    //    request.Method = "GET"
                    //        Dim response As WebResponse = request.GetResponse()
                    //        ' MessageBox.Show((CType(response, HttpWebResponse)).StatusDescription)
                    //        Using dataStream As Stream = response.GetResponseStream()
                    //            Dim reader As StreamReader = New StreamReader(dataStream)
                    //            Dim responseFromServer As String = reader.ReadToEnd()
                    //            response.Close()
                    //            If responseFromServer = "sms sent successfully" Then
                    //            strCommand = "INSERT into smsLog (IID, PhoneNo, smsType, Text, EntryDate) Values (" & IID & ",'" & txtPhoneNo.Text & "'," & 1 & ",'" & "Thank you for choosing Postal Life Insurrance." & "','" & GetServerDateTime() & "')"
                    //            executeQuery(strCommand, CommandType.Text)
                    //        End If
                    //    End Using
                    //    'End If

                    //End Sub
                    //----------------------------------------------------------------------------------------


                    otp = GenerateOTP();


                    //TempData["OTP"] = otp.otp;
                    HttpContext.Session.SetString("OTP", otp);
                    string phone = HttpContext.Session.GetString("Mobile");
                    string mobilenumber = "92" + phone.Substring(phone.Length - 10);
                    //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "Your OTP For PLIC Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PCLSDKJPISWUWDHDPLIC&password=b63b122c528f54df4a0446b6bab05515&sender=8576&apikey=1094cbd4841bb89b173098af5bed0e60&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "Your OTP For PLIC Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");

                    //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://brandyourtext.com/sms/api/send?username=Carbon8&password=786786&mask=Alert Pak&mobile=" + HttpContext.Session.GetString("Mobile") + "&message="+ "Your OTP For PLICL Customer WebPortal Login is:  " + HttpContext.Session.GetString("OTP") + "");

                    //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://brandyourtext.com/sms/api/send?username=Carbon8&password=786786&mask=Alert Pak&mobile=" + HttpContext.Session.GetString("Mobile") + "&message=" + HttpContext.Session.GetString("OTP") + "");
                    request.Method = "GET";

                    WebResponse response = request.GetResponse();
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var ApiStatus = reader.ReadToEnd();
                        if (ApiStatus == "Your Message is sent, MSISDN 1, Cost 1")
                        {
                            _db.SmsLog(phone, 1, "OTP for web portal");
                            return View("~/Views/Account/OTP.cshtml");
                        }
                        else
                        {
                            ModelState.AddModelError("NotFound", "Could not send the OTP. Please make sure your mobile number is in the correct format. i-e 03001111111 (without '-')");
                            return View("Login");
                        }
                    }

                    //return View("~/Views/Account/OTP.cshtml");
                    //List<Policy> policies = new List<Policy>();
                    //              Done in db.cs
                    /*
                    string policyQuery = "select PolicyNo from Insurant where NIC=@CNIC and Phone=@MobileNo";
                    //string sqlquery = "select count(*) from Insurant";
                    conn.Open();
                    SqlCommand command = new SqlCommand(policyQuery, conn);
                    command.Parameters.AddWithValue("@CNIC", acc.CNIC);      //"36502-8724972-1"
                    command.Parameters.AddWithValue("@MobileNo", acc.MobileNo);  //"03026928084"
                    SqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        Policy pol = new Policy();
                        pol.policy = dr.GetValue(0).ToString();
                        //poll = pol.policy;
                        //policy = dr.GetValue(0).ToString();
                        //policies.Add(dr.GetValue(0).ToString());  //sdr["PolicyNo"]
                        policies.Add(pol);
                    }
                    //List<PremiumVoucher> vouchers = db.GetAllVouchers().ToList();

                    //IEnumerable<Policy> policies = new List<Policy>();

                    //policies = db.GetAllPolicies(CNIC, Mob).ToList();

                    return View("~/Views/Home/Policies.cshtml", policies);

                    */
                }
            }
            else
            {
                ModelState.AddModelError("NotFound", "Please Enter your mobile number in the correct format. i-e 03001111111 (without '-')");
                ModelState.AddModelError("NotFound", "Record Not Found. Please enter correct credentials. If Error remains contact your nearby PLIC branch");
                return View("Login");
            }
        }

        //--------------------------------Verify OTP--------------------------------------------
        [HttpPost]
        public IActionResult VerifyOTP(OTP code, string value)
        {


            if (code.otp == HttpContext.Session.GetString("OTP"))
            {

                List<Policy1> policies = new List<Policy1>();
                SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection"));
                //string policyQuery = "select PolicyNo,IID from Insurant where NIC=@CNIC and Phone=@MobileNo";
                //string sqlquery = "select count(*) from Insurant";

                if (HttpContext.Session.GetString("NIC") != "" && HttpContext.Session.GetString("NIC") != null)
                {
                    HttpContext.Session.SetString("CNIC", HttpContext.Session.GetString("NIC"));
                    SqlCommand command = new SqlCommand("SP_Select_Insurant_Info", conn);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("CNIC", HttpContext.Session.GetString("CNIC"));      //"36502-8724972-1"
                    command.Parameters.AddWithValue("MobileNo", HttpContext.Session.GetString("Mobile"));  //"03026928084"

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
                    LoggingHistory(HttpContext.Session.GetString("CNIC"), HttpContext.Session.GetString("CNIC"), HttpContext.Session.GetString("Mobile"));
                    return View("~/Views/Home/Policy.cshtml", policies);
                }
                // return View("~/Views/Account/Login.cshtml");
                return RedirectToAction("Login", "Account");

                //// changed by Aizaz and Imran Dated 17 Feb 22
                //return View("~/Views/Home/Menu.cshtml");
            }
            else
            {
                ModelState.AddModelError("Wrong OTP", "Wrong Code. Please enter correct code again.");
                return View("~/Views/Account/OTP.cshtml", otp);
            }
        }

        ////--------------------------------SMS Log------------------------------------------------
        //public void SmsLog(string phone,int smsType,string text)
        //{
        //    db _db = new db();
        //    using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
        //    {
        //        SqlCommand cmd = new SqlCommand("SP_Insert_smsLog_For_WebPortal", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("phone", phone);
        //        cmd.Parameters.AddWithValue("smsType", smsType);
        //        cmd.Parameters.AddWithValue("text", text);
        //        conn.Close();
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //    }
        //}

        //--------------------------------Logging History-----------------------------------------
        public void LoggingHistory(string LoginId, string roleid, string Password)
        {
            db _db = new db();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_Insert_Logging_For_cc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("LoginId", LoginId);
                cmd.Parameters.AddWithValue("Password", Password);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        //-------------------------------Admin Logging History------------------------------------
        public void AdminLoggingHistory(int adminId)
        {
            db _db = new db();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_Insert_AdminLogging_For_WebPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("adminId", adminId);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        //-------------------------------Admin LogOut History--------------------------------------
        public void AdminLogOutHistory(int adminId)
        {
            if (adminId != 0)
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Insert_AdminLogout_History", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("adminId", adminId);
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        //public IActionResult GetPolicyNo(string cnicNo)
        //{
        //    try
        //    {
        //        if (HttpContext.Session.GetString("CNICno") != "" && HttpContext.Session.GetString("CNICno") != null)
        //        {
        //            List<Policy> policies = new List<Policy>();                    //postComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
        //            policies = _db.GetPolicyNo(cnicNo);

        //            return Json(policies);
        //        }
        //        return Json("UserUnAuthorized");
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}
    }
}
