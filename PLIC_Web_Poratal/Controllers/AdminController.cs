using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PLIC_Web_Poratal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Controllers
{
    public class AdminController : Controller
    {

        db _db = new db();
        EncDec encDec = new EncDec();
        //-----------------------------Menu-----------------------------------------
        public IActionResult Menu()
        {
            return RedirectToAction("AdminDashboard", "Admin");
        }

        //-----------------------------PreComplaint-----------------------------------
        public IActionResult PreComplaint()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    //ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    List<PreComplaint> preComplaints = new List<PreComplaint>();
                    //CompliantViewModel compliantViewModelObj = new CompliantViewModel();
                    UserRights userRights = new UserRights();
                    userRights = GetRigths();
                    preComplaints = _db.GetPreComplaints(userRights);
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    return View(preComplaints);
                }
                return RedirectToAction("Admin", "Account");

            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Add Complaint----------------------------------------
        public IActionResult AddComplaint()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    //PostLoginComplaint postObj = new PostLoginComplaint();
                    //postObj = _db.GetFielUnit();
                    return View();
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Dashboard----------------------------------------
        public IActionResult AdminDashboard()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    ViewModel viewObj = new ViewModel();
                    UserRights userRights = new UserRights();
                    userRights = GetRigths();
                    viewObj = _db.GetTotalComplaints(userRights);
                    return View(viewObj);
                    //return View();
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------GetComplaintForwardigDetails----------------------------------------
        public IActionResult GetComplaintForwardigDetails()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    //ViewModel viewObj = new ViewModel();
                    //ViewForwardedComplaintDataModel viewForwardedComplaintDataModel = new ViewForwardedComplaintDataModel();
                    ViewBag.Data = "";
                    return View("~/Views/Admin/ComplaintForwardHistory.cshtml");
                    //return View();
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------GetComplaintForwardigDetails----------------------------------------
        [HttpPost]
        public IActionResult GetComplaintForwardigDetails(ViewForwardedComplaintDataModel viewForwardedComplaintDataModel)

        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    if (viewForwardedComplaintDataModel.complaintId != 0)
                    {
                        ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                        ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                        ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                        ViewForwardedComplaintDataModel viewForwardedComplaintDataModel1 = new ViewForwardedComplaintDataModel();
                        viewForwardedComplaintDataModel1 = _db.GetComplaintForwardigDetails(viewForwardedComplaintDataModel.complaintId);
                        ViewBag.Data = "OK";
                        return View("~/Views/Admin/ComplaintForwardHistory.cshtml", viewForwardedComplaintDataModel1);
                    }
                    ViewBag.Data = "";
                    ViewBag.CompalintId = "Empty";
                    return View("~/Views/Admin/ComplaintForwardHistory.cshtml");
                    //return View();
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------GetComplaintDetail----------------------------------------

        public IActionResult GetComplaintDetail(int complaintId)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    ComplaintDetailViewModel complaintDetailViewObj = new ComplaintDetailViewModel();
                    complaintDetailViewObj = _db.GetComplaintDetail(complaintId);
                    //for(int i=0; i < complaintDetailViewObj.complaintDocuments.Count;i++)
                    //{

                    //}/////////////////////////////////////////////////////////

                    ViewBag.Data = "";
                    return View("~/Views/Admin/ComplaintDetails.cshtml", complaintDetailViewObj);
                    //return View();
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------GetComplaintDetail----------------------------------------

        public FileResult DownloadFile(string fileName)
        {
            try
            {

                ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");

                /////////////////////////
                string path = Path.Combine(Directory.GetCurrentDirectory(), "C://Resource/WebPortalDocuments/") + fileName;
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "application/octet-stream", fileName);

            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Resolved PreComplaint List------------------------------
        public IActionResult ResolvedPreComplaintList()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    //ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    List<PreComplaint> preComplaints = new List<PreComplaint>();
                    UserRights userRights = new UserRights();
                    userRights = GetRigths();
                    preComplaints = _db.GetResolvedPreComplaints(userRights);
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    return View(preComplaints);
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------PostComplaint------------------------------
        public IActionResult PostComplaint()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    CompliantViewModel compliantViewModelObj = new CompliantViewModel();
                    UserRights userRights = new UserRights();
                    userRights = GetRigths();
                    compliantViewModelObj.postComplaints = _db.GetPostComplaints(userRights);
                    compliantViewModelObj.groups = _db.GetGroupList();
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    return View(compliantViewModelObj);
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Get Forwarded Complaints------------------------------
        public IActionResult GetForwardedComplaints()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    CompliantViewModel compliantViewModelObj = new CompliantViewModel();
                    UserRights userRights = new UserRights();
                    userRights = GetRigths();
                    compliantViewModelObj.postComplaints = _db.GetForwardedComplaints(userRights);
                    compliantViewModelObj.groups = _db.GetGroupList();
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    return View(compliantViewModelObj);
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Resolved PostComplaint List------------------------------
        public IActionResult ResolvedPostComplaintList()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    List<PostComplaint> postComplaints = new List<PostComplaint>();
                    UserRights userRights = new UserRights();
                    userRights = GetRigths();
                    postComplaints = _db.GetResolvedPostComplaints(userRights);
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    return View(postComplaints);
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }



        //-----------------------------Get Reopend Complaints------------------------------
        public IActionResult GetReopendComplaints()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    List<PostComplaint> postComplaints = new List<PostComplaint>();
                    UserRights userRights = new UserRights();
                    //List<Group> groups = new List<Group>();
                    userRights = GetRigths();
                    postComplaints = _db.GetReopendComplaints(userRights);
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    return View("~/Views/Admin/ReopendComplaints.cshtml", postComplaints);
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------My Register Complaints------------------------------
        public IActionResult MyRegisteredComplaints()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    List<PostComplaint> postComplaints = new List<PostComplaint>();
                    //UserRights userRights = new UserRights();
                    //userRights = GetRigths();
                    int adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    postComplaints = _db.GetMyRegisteredComplaints(adminId);
                    ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                    ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                    ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                    return View(postComplaints);
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Resolved PreComplaint1--------------------------------
        public IActionResult ResolvedPreComplaint1(int srNo, string resolveDate)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {

                    //string resp = _db.ResolvedPreComplaint(id);

                    return RedirectToAction("PreComplaint", "Admin");
                }
                return RedirectToAction("Admin", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Resolved PreComplaint---------------------------------
        public IActionResult ResolvedPreComplaint(PreComplaint preComplaint)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    preComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    string resp = _db.ResolvedPreComplaint(preComplaint);
                    string complaintId = Convert.ToString(preComplaint.complaintId);
                    string phone = preComplaint.newPhoneNo;
                    string mobilenumber = "92" + phone.Substring(phone.Length - 10);
                    //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "معزز کسٹمر%0aآپ کی شکایت نمبر " + complaintId + " حل ہو چکی ہے۔%0aآپ کے تعاون کا شکریہ %0aپوسٹل لائف انشورنس کمپنی");
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PCLSDKJPISWUWDHDPLIC&password=b63b122c528f54df4a0446b6bab05515&sender=8576&apikey=1094cbd4841bb89b173098af5bed0e60&type=Urdu&message=MESSAGE&phone=" + mobilenumber + "&message=" + "معزز کسٹمر%0aآپ کی شکایت نمبر " + complaintId + " حل ہو چکی ہے۔%0aآپ کے تعاون کا شکریہ %0aپوسٹل لائف انشورنس کمپنی");

                    request.Method = "GET";

                    WebResponse response = request.GetResponse();
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var ApiStatus = reader.ReadToEnd();
                        if (ApiStatus == "Your Message is sent, MSISDN 1, Cost 2")
                        {
                            _db.SmsLog(phone, 2, "Complaint resolution message");
                        }
                    }

                    return Json("success");
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Resolved PostComplaint------------------------------
        public IActionResult ResolvedPostComplaint(PostComplaint postComplaint)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    postComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    string resp = _db.ResolvedPostComplaint(postComplaint);
                    if (postComplaint.phoneNo.Length == 11)
                    {
                        string complaintId = Convert.ToString(postComplaint.complaintId);
                        string phone = postComplaint.phoneNo;
                        string mobilenumber = "92" + phone.Substring(phone.Length - 10);
                        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PLIKHYAMNAZALCIS&password=42207c1e1a53fc803b2cd2f65c09bfb&apikey=1d4fade70b3084dfa9fe4ff2b8f2540e&sender=PLICL&type=English&message=MESSAGE&phone=" + mobilenumber + "&message=" + "معزز کسٹمر%0aآپ کی شکایت نمبر " + complaintId + " حل ہو چکی ہے۔%0aآپ کے تعاون کا شکریہ %0aپوسٹل لائف انشورنس کمپنی");
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://sms.lrt.com.pk/api/sms-single-or-bulk-api.php?username=PCLSDKJPISWUWDHDPLIC&password=b63b122c528f54df4a0446b6bab05515&sender=8576&apikey=1094cbd4841bb89b173098af5bed0e60&type=Urdu&message=MESSAGE&phone=" + mobilenumber + "&message=" + "معزز کسٹمر%0aآپ کی شکایت نمبر " + complaintId + " حل ہو چکی ہے۔%0aآپ کے تعاون کا شکریہ %0aپوسٹل لائف انشورنس کمپنی");

                        request.Method = "GET";

                        WebResponse response = request.GetResponse();
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var ApiStatus = reader.ReadToEnd();
                            if (ApiStatus == "Your Message is sent, MSISDN 1, Cost 2")
                            {
                                _db.SmsLog(phone, 2, "Complaint resolution message");
                            }
                        }
                    }

                    return Json("success");
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }


        //-----------------------------Add Forworded Complaint------------------------------
        public IActionResult AddForwordedComplaint(PostComplaint postComplaint)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    postComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    string resp = _db.AddForwordedComplaint(postComplaint);
                    return Json(resp);
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }


        //-----------------------------Add Complaint-------------------------------------------
        [HttpPost]
        public IActionResult AddComplaint(PostLoginComplaint postLoginComplaint)
        {
            if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
            {
                try
                {

                    if (ModelState.IsValid)
                    {

                        postLoginComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                        string resp;
                        if (postLoginComplaint.complaintType == "Mobile Number Update")
                        {

                            resp = _db.insertMobileNumberComplaint(postLoginComplaint);
                            if (resp== "ComplaintExist")
                            {
                                ViewBag.message = "ComplaintExist";
                                ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                                ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                                ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                                return View("~/Views/Admin/AddComplaint.cshtml");
                            }
                        }
                        else
                        {
                            resp = _db.insertPostLoginComplaint(postLoginComplaint);

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
                        }


                        if (postLoginComplaint.mobileNumber.Length == 11)
                        {
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
                            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                            ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                            ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                            ViewBag.message = "OK";
                            ModelState.Clear();
                            return View("~/Views/Admin/AddComplaint.cshtml");
                            //return RedirectToAction("AddComplaint", "Admin");
                        }
                        ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                        ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                        ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                        ModelState.Clear();

                        return View("~/Views/Admin/AddComplaint.cshtml");
                        //return RedirectToAction("AddComplaint", "Admin");
                    }
                    else
                    {
                        //ModelState.Clear();
                        ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                        ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                        ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                        return View("~/Views/Admin/AddComplaint.cshtml");
                        //return RedirectToAction("AddComplaint", "Admin");
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("Admin", "Account");
        }

        //------------------------------Get Policy Number---------------------------------
        public IActionResult GetPolicyNo(string cnicNo)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    List<Policy> policies = new List<Policy>();                    //postComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    policies = _db.GetPolicyNo(cnicNo);

                    return Json(policies);
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //------------------------------Get Regions---------------------------------
        public IActionResult GetRegions(int circleId)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    List<Region> regions = new List<Region>();                    //postComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    regions = _db.GetRegions(circleId);

                    return Json(regions);
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //------------------------------Get FieldUnit List---------------------------------
        public IActionResult GetFielUnitList(int regionId)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    List<FieldUnits> fieldUnits = new List<FieldUnits>();                    //postComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    fieldUnits = _db.GetFielUnitList(regionId);

                    return Json(fieldUnits);
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Get AdminUsers--------------------------------
        public IActionResult GetAdminUsers(int regionId)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    List<SystemUser> systemUsers = new List<SystemUser>();                    //postComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    systemUsers = _db.GetAdminUsers(regionId);

                    return Json(systemUsers);
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //------------------------------Get FieldUnit---------------------------------
        public IActionResult GetFiedUnitAndMobileNumber(string policyNo)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    Details details = new Details();                    //postComplaint.adminId = Convert.ToInt32((HttpContext.Session.GetInt32("AdminId")));
                    details = _db.GetFiedUnitAndMobileNumber(policyNo);

                    return Json(details);
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //------------------------------Get Total Rsolved Complaints---------------------------------
        [HttpPost]
        public IActionResult GetTotalRsolvedComplaints(ViewDataModel viewDataModel)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {

                    ViewModel viewObj1 = new ViewModel();
                    if (viewDataModel.circleId == 0)
                    {
                        viewDataModel.circleId = Convert.ToInt32((HttpContext.Session.GetInt32("CircleId")));
                    }
                    viewObj1 = _db.GetTotalRsolvedComplaints(viewDataModel);

                    return Json(viewObj1);
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //------------------------------Get Circles---------------------------------
        public IActionResult GetCircles()
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    List<Circle> circles = new List<Circle>();
                    circles = _db.GetCircles();

                    return Json(circles);
                }
                return Json("UserUnAuthorized");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------CompalintsSummaryReporte------------------------------

        public IActionResult GetCompalintsSummaryReporte(ViewDataModel viewDataModel)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    ViewModel viewObj = new ViewModel();
                    DateTime myDateTime = DateTime.Now;

                    viewObj = _db.GetCompalintsSummaryReporte(viewDataModel);
                    viewObj.userName = HttpContext.Session.GetString("AdminName");
                    viewObj.dateTime = myDateTime.ToString("dd-MM-yyyy hh:mm:ss");

                    return Json(viewObj);
                }
                return Json("UserUnAuthorized");

            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------GetUserPerformanceSummary------------------------------

        public IActionResult GetUserPerformanceSummary(ViewDataModel viewDataModel)
        {
            try
            {
                if (HttpContext.Session.GetString("AdminName") != "" && HttpContext.Session.GetString("AdminName") != null)
                {
                    ViewModel viewObj = new ViewModel();
                    DateTime myDateTime = DateTime.Now;

                    viewObj = _db.GetUserPerformanceSummary(viewDataModel);
                    viewObj.userName = HttpContext.Session.GetString("AdminName");
                    viewObj.dateTime = myDateTime.ToString("dd-MM-yyyy hh:mm:ss");

                    return Json(viewObj);
                }
                return Json("UserUnAuthorized");

            }
            catch (Exception)
            {
                throw;
            }
        }


        //-----------------------------Add SysUser-----------------------------------
        [HttpGet]
        public IActionResult AddSysUser()
        {
            if (HttpContext.Session.GetInt32("Sysadmin") == 1 && HttpContext.Session.GetString("AdminName") != null)
            {
                ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                SystemUser systemUser = new SystemUser();
                //List<Circle> circles = new List<Circle>();
                //systemUser.circles = _db.GetCircles();

                systemUser.groups = _db.GetGroupList();
                return View(systemUser);
            }
            return RedirectToAction("Admin", "Account");
        }

        //-----------------------------Get SysUser-----------------------------------
        public IActionResult GetSysUsers()
        {
            if (HttpContext.Session.GetInt32("Sysadmin") == 1 && HttpContext.Session.GetString("AdminName") != null)
            {
                ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                //List<SystemUser> systemUsers = new List<SystemUser>();
                SystemUserViewModel systemUserViewModelObj = new SystemUserViewModel();
                systemUserViewModelObj.systemUsers = _db.GetSysUsers();
                systemUserViewModelObj.groups = _db.GetGroupList();
                for (int i = 0; i < systemUserViewModelObj.systemUsers.Count; i++)
                {
                    if (systemUserViewModelObj.systemUsers[i].AdminPassword == "")
                    {
                        systemUserViewModelObj.systemUsers[i].AdminPassword = "";
                    }
                    else
                    {
                        systemUserViewModelObj.systemUsers[i].AdminPassword = encDec.DecodePassword(systemUserViewModelObj.systemUsers[i].AdminPassword);
                    }
                }
                return View("~/Views/Admin/SysUsersList.cshtml", systemUserViewModelObj);
            }
            return RedirectToAction("Admin", "Account");
        }

        //-----------------------------Get Customer List-----------------------------------
        public IActionResult GetCustomerList()
        {
            if (HttpContext.Session.GetInt32("Sysadmin") == 1 && HttpContext.Session.GetString("AdminName") != null)
            {
                ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                List<Customer> customers = new List<Customer>();
                customers = _db.GetCustomer();
                for (int i = 0; i < customers.Count; i++)
                {

                    customers[i].password = encDec.DecodePassword(customers[i].password);

                    //customers[i].password = encDec.EncodePassword(customers[i].password);
                    //using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                    //{
                    //    string Sp = "";
                    //    Sp = "update WebPortalCustomerLogin set Password='" + customers[i].password + "' where Id='" + customers[i].id + "'";
                    //    SqlCommand cmd = new SqlCommand(Sp, conn);
                    //    conn.Close();
                    //    conn.Open();
                    //    cmd.ExecuteNonQuery();
                    //    conn.Close();

                    //}


                }
                return View("~/Views/Admin/CustomerList.cshtml", customers);
            }
            return RedirectToAction("Admin", "Account");
        }

        //-----------------------------UpdateSysUserInfo------------------------------

        public IActionResult UpdateSysUserInfo(SystemUser systemUser)
        {
            try
            {
                if (HttpContext.Session.GetInt32("Sysadmin") == 1 && HttpContext.Session.GetString("AdminName") != null)
                {
                    string password;
                    //string groupId;
                    if (systemUser.AdminPassword != null)
                    {
                        password = encDec.EncodePassword(systemUser.AdminPassword);
                    }
                    else
                    {
                        password = null;
                    }

                    string resp = _db.UpdateSysUserInfo(systemUser, password);
                    return Json("success");
                }
                return Json("UserUnAuthorized");

            }
            catch (Exception)
            {
                throw;
            }
        }

        //-----------------------------Update Customer Info------------------------------

        public IActionResult UpdateCustomerInfo(Customer customer)
        {
            try
            {
                if (HttpContext.Session.GetInt32("Sysadmin") == 1 && HttpContext.Session.GetString("AdminName") != null)
                {
                    string password;
                    if (customer.password != null)
                    {
                        password = encDec.EncodePassword(customer.password);
                    }
                    else
                    {
                        password = null;
                    }
                    string resp = _db.UpdateCustomerInfo(customer, password);
                    return Json("success");
                }
                return Json("UserUnAuthorized");

            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public IActionResult AddSysUser(SystemUser systemUser)
        {
            try
            {

                //List<Circle> circles = new List<Circle>();
                if (ModelState.IsValid)
                {
                    if (HttpContext.Session.GetInt32("Sysadmin") == 1 && HttpContext.Session.GetString("AdminName") != null)
                    {
                        systemUser.UserName.Trim();
                        string res = _db.InsertSysUserInfo(systemUser);
                        ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
                        ViewBag.GroupId = HttpContext.Session.GetInt32("GroupId");
                        ViewBag.SysAdmin = HttpContext.Session.GetInt32("Sysadmin");
                        ViewBag.message = res;
                        ModelState.Clear();

                        SystemUser systemUser1 = new SystemUser();
                        systemUser1.groups = _db.GetGroupList();
                        return View(systemUser1);
                    }
                    return RedirectToAction("Admin", "Account");
                }
                ViewBag.AdminName = HttpContext.Session.GetString("AdminName");

                //systemUser.circles = _db.GetCircles();
                SystemUser systemUser2 = new SystemUser();
                systemUser2.groups = _db.GetGroupList();
                return View(systemUser2);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public UserRights GetRigths()
        {
            UserRights userRights = new UserRights();
            userRights.CircleId = Convert.ToInt32((HttpContext.Session.GetInt32("CircleId")));
            userRights.RegionId = Convert.ToInt32((HttpContext.Session.GetInt32("RegionId")));
            userRights.FieldUnitId = Convert.ToInt32((HttpContext.Session.GetInt32("FieldUnitId")));
            userRights.GroupId = Convert.ToInt32((HttpContext.Session.GetInt32("GroupId")));
            return userRights;
        }
        ////--------------------------------SMS Log------------------------------------------------
        //public void SmsLog(string phone, int smsType,string text)
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
    }
}


