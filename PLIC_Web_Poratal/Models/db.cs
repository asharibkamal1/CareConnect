using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace PLIC_Web_Poratal.Models
{
    public class db
    {
        string _regionId;
        SqlConnection con;
        public db()
        {
            var configuration = GetConfiguration();
            con = new SqlConnection(configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            con = new SqlConnection(configuration.GetSection("ConnectionStrings").GetSection("CARGOConnection").Value);
        }
        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        //-------------------Get All Policies----------------------------------------
        public IEnumerable<Policy> GetAllPolicies(string CNIC, string Phone)
        {
            db _db = new db();
            Policy policy = new Policy();
            var policieslist = new List<Policy>();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                string policyQuery = "select PolicyNo from Insurant where NIC=@CNIC and Phone=@Password";
                //string sqlquery = "select count(*) from Insurant";

                SqlCommand command = new SqlCommand(policyQuery, conn);
                command.Parameters.AddWithValue("@CNIC", CNIC);      //"36502-8724972-1"
                command.Parameters.AddWithValue("@Password", Phone);  //"03026928084"
                conn.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    policy.policy = dr.GetValue(0).ToString();
                    policieslist.Add(policy);
                }
            }
            return policieslist;
        }

        //------------------------------Get Voucher Detail------------------------------------
        public IEnumerable<PrintPremiumVoucher> GetVoucherDetail(string VoucherNo, string iid)
        {
            db _db = new db();
            var vouchersdetail = new List<PrintPremiumVoucher>();
            //var voucherdetail = new PrintPremiumVoucher();
            try
            {
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_SELECT_INSURANT_PaymentVoucherDetail_FOR_WEB", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("VoucherNo", VoucherNo);
                    cmd.Parameters.AddWithValue("IID", iid);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var voucherdetail = new PrintPremiumVoucher();
                        voucherdetail.PolicyNo = dr["PolicyNo"].ToString();
                        voucherdetail.Name = dr["Name"].ToString();
                        voucherdetail.CNIC = dr["NIC"].ToString();
                        voucherdetail.PremiumMode = dr["PremiumMode"].ToString();
                        voucherdetail.BookNo = Convert.ToInt32(dr["BookNo"]);
                        voucherdetail.VoucherNo = dr["VoucherNo"].ToString();
                        voucherdetail.Amount = Convert.ToDouble(dr["Amount"]);
                        //var temp = Convert.ToDateTime(dr["DueDate"]);
                        voucherdetail.DueDate = Convert.ToDateTime(dr["DueDate"]);
                        voucherdetail.PolicyYear = dr["P_Year"].ToString();
                        voucherdetail.ABL_AccountNo = dr["ABL_AccountNo"].ToString();
                        voucherdetail.UBL_AccountNo = dr["UBL_AccountNo"].ToString();
                        voucherdetail.Is_ABLAccount = dr["Is_ABLAccount"].ToString();
                        voucherdetail.Is_UBLAccount = dr["Is_UBLAccount"].ToString();
                        voucherdetail.P_YearType = dr["p_YearType"].ToString();
                        voucherdetail.RegionName = dr["RegionName"].ToString();
                        voucherdetail.Rebate = (dr["Rebate"] is DBNull) ? null : Convert.ToDouble(dr["Rebate"]);
                        voucherdetail.AmountWithinDueDate = (dr["AmountWithinDue"] is DBNull) ? null : Convert.ToDouble(dr["AmountWithinDue"]);
                        vouchersdetail.Add(voucherdetail);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return vouchersdetail;
        }

        //--------------------------Get Voucher Detail First Premium-----------------------------------
        public IEnumerable<PrintfirstPremiumVoucher> GetVoucherDetailfirstpremium(string VoucherNo, string iid)
        {
            db _db = new db();
            var vouchersdetail = new List<PrintfirstPremiumVoucher>();
            //var voucherdetail = new PrintPremiumVoucher();

            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_SELECT_INSURANT_PaymentVoucherDetail_FOR_WEB", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VoucherNo", VoucherNo);
                cmd.Parameters.AddWithValue("IID", iid);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var voucherdetail = new PrintfirstPremiumVoucher();
                    voucherdetail.PolicyNo = dr["PolicyNo"].ToString();
                    voucherdetail.Name = dr["Name"].ToString();
                    voucherdetail.CNIC = dr["NIC"].ToString();
                    voucherdetail.PremiumMode = dr["PremiumMode"].ToString();
                    voucherdetail.BookNo = Convert.ToInt32(dr["BookNo"]);
                    voucherdetail.VoucherNo = dr["VoucherNo"].ToString();
                    voucherdetail.Amount = Convert.ToDouble(dr["Amount"]);
                    //var temp = Convert.ToDateTime(dr["DueDate"]);
                    voucherdetail.DueDate = Convert.ToDateTime(dr["DueDate"]);
                    voucherdetail.PolicyYear = dr["P_Year"].ToString();
                    voucherdetail.ABL_AccountNo = dr["ABL_AccountNo"].ToString();
                    voucherdetail.UBL_AccountNo = dr["UBL_AccountNo"].ToString();
                    voucherdetail.Is_ABLAccount = dr["Is_ABLAccount"].ToString();
                    voucherdetail.Is_UBLAccount = dr["Is_UBLAccount"].ToString();
                    voucherdetail.P_YearType = dr["p_YearType"].ToString();
                    voucherdetail.RegionName = dr["RegionName"].ToString();
                    voucherdetail.Rebate = (dr["Rebate"] is DBNull) ? null : Convert.ToDouble(dr["Rebate"]);
                    voucherdetail.AmountWithinDueDate = (dr["AmountWithinDue"] is DBNull) ? null : Convert.ToDouble(dr["AmountWithinDue"]);
                    vouchersdetail.Add(voucherdetail);
                }
                conn.Close();
            }
            return vouchersdetail;
        }

        //--------------------Get Information-----------------------------------
        public ViewModel GetInformation(int iid)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                ViewModel viewObj = new ViewModel();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Personal_Info", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("InsrnseID", iid);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    viewObj.info = new InsurantInfo();
                    DataTable dt = ds.Tables[0];
                    viewObj.info = Helper.GetItem<InsurantInfo>(dt.Rows[0]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    viewObj.occupation = new Occupation();
                    DataTable dt = ds.Tables[1];
                    viewObj.occupation = Helper.GetItem<Occupation>(dt.Rows[0]);
                }
                if (ds.Tables[2].Rows.Count > 0)
                {
                    viewObj.familyHistory = new List<FamilyHistory>();
                    DataTable dt = ds.Tables[2];
                    viewObj.familyHistory = Helper.ConvertDataTable<FamilyHistory>(dt);
                }

                if (ds.Tables[3].Rows.Count > 0)
                {
                    viewObj.nominees = new List<Nominee>();
                    DataTable dt = ds.Tables[3];
                    viewObj.nominees = Helper.ConvertDataTable<Nominee>(dt);
                }
                return viewObj;
            }
            catch (Exception)
            {

                throw;
            }
        }


        //--------------------GetTotalComplaints-----------------------------------
        public ViewModel GetTotalComplaints(UserRights userRights)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                ViewModel viewObj = new ViewModel();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Total_Complaints", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("regionId", userRights.RegionId);
                    cmd.Parameters.AddWithValue("circleId", userRights.CircleId);
                    cmd.Parameters.AddWithValue("fielduniId", userRights.FieldUnitId);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    viewObj.preLoginComplaint = new PreLoginComplaint();
                    DataTable dt = ds.Tables[0];
                    viewObj.preLoginComplaint = Helper.GetItem<PreLoginComplaint>(dt.Rows[0]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    viewObj.postLoginComplaint = new PostLoginComplaint();
                    DataTable dt = ds.Tables[1];
                    viewObj.postLoginComplaint = Helper.GetItem<PostLoginComplaint>(dt.Rows[0]);
                }
                //if (ds.Tables[2].Rows.Count > 0)
                //{
                //    viewObj.systemUsers = new List<SystemUser>();
                //    DataTable dt = ds.Tables[2];
                //    viewObj.systemUsers = Helper.ConvertDataTable<SystemUser>(dt);
                //}
                if (userRights.CircleId != 0 && userRights.RegionId == 0)
                {
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        viewObj.regions = new List<Region>();
                        DataTable dt = ds.Tables[2];
                        viewObj.regions = Helper.ConvertDataTable<Region>(dt);
                        viewObj.option = 1;
                    }

                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        viewObj.postLoginComplaint1 = new PostLoginComplaint();
                        DataTable dt = ds.Tables[3];
                        viewObj.postLoginComplaint1 = Helper.GetItem<PostLoginComplaint>(dt.Rows[0]);
                    }

                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        viewObj.complaintsSummaries = new List<ComplaintsSummary>();
                        DataTable dt = ds.Tables[4];
                        viewObj.complaintsSummaries = Helper.ConvertDataTable<ComplaintsSummary>(dt);
                    }

                    if (ds.Tables[5].Rows.Count > 0)
                    {
                        viewObj.circle = new Circle();
                        DataTable dt = ds.Tables[5];
                        viewObj.circle = Helper.GetItem<Circle>(dt.Rows[0]);
                    }

                    if (ds.Tables[6].Rows.Count > 0)
                    {
                        viewObj.preLoginComplaint1 = new PreLoginComplaint();
                        DataTable dt = ds.Tables[6];
                        viewObj.preLoginComplaint1 = Helper.GetItem<PreLoginComplaint>(dt.Rows[0]);
                    }

                    if (ds.Tables[7].Rows.Count > 0)
                    {
                        viewObj.postLoginComplaint2 = new PostLoginComplaint();
                        DataTable dt = ds.Tables[7];
                        viewObj.postLoginComplaint2 = Helper.GetItem<PostLoginComplaint>(dt.Rows[0]);
                    }

                }
                else if (userRights.CircleId == 0)
                {
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        viewObj.circles = new List<Circle>();
                        DataTable dt = ds.Tables[2];
                        viewObj.circles = Helper.ConvertDataTable<Circle>(dt);
                        viewObj.option = 2;
                    }

                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        viewObj.postLoginComplaint1 = new PostLoginComplaint();
                        DataTable dt = ds.Tables[3];
                        viewObj.postLoginComplaint1 = Helper.GetItem<PostLoginComplaint>(dt.Rows[0]);
                    }

                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        viewObj.complaintsSummaries = new List<ComplaintsSummary>();
                        DataTable dt = ds.Tables[4];
                        viewObj.complaintsSummaries = Helper.ConvertDataTable<ComplaintsSummary>(dt);
                    }


                }
                else
                {
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        viewObj.postLoginComplaint1 = new PostLoginComplaint();
                        DataTable dt = ds.Tables[2];
                        viewObj.postLoginComplaint1 = Helper.GetItem<PostLoginComplaint>(dt.Rows[0]);
                    }

                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        viewObj.complaintsSummaries = new List<ComplaintsSummary>();
                        DataTable dt = ds.Tables[3];
                        viewObj.complaintsSummaries = Helper.ConvertDataTable<ComplaintsSummary>(dt);
                    }

                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        viewObj.circle = new Circle();
                        DataTable dt = ds.Tables[4];
                        viewObj.circle = Helper.GetItem<Circle>(dt.Rows[0]);
                    }

                    if (ds.Tables[5].Rows.Count > 0)
                    {
                        viewObj.region = new Region();
                        DataTable dt = ds.Tables[5];
                        viewObj.region = Helper.GetItem<Region>(dt.Rows[0]);
                    }
                }

                return viewObj;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //--------------------GetTotalRsolvedComplaints-----------------------------------
        public ViewModel GetTotalRsolvedComplaints(ViewDataModel viewDataModel)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                ViewModel viewObj = new ViewModel();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Total_Resolved_Complaints", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("circleId", viewDataModel.circleId);
                    cmd.Parameters.AddWithValue("regionId", viewDataModel.regionId);
                    cmd.Parameters.AddWithValue("userId", viewDataModel.userId);
                    cmd.Parameters.AddWithValue("fromDate", viewDataModel.fromDate);
                    cmd.Parameters.AddWithValue("toDate", viewDataModel.toDate);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    viewObj.preLoginComplaint = new PreLoginComplaint();
                    DataTable dt = ds.Tables[0];
                    viewObj.preLoginComplaint = Helper.GetItem<PreLoginComplaint>(dt.Rows[0]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    viewObj.postLoginComplaint = new PostLoginComplaint();
                    DataTable dt = ds.Tables[1];
                    viewObj.postLoginComplaint = Helper.GetItem<PostLoginComplaint>(dt.Rows[0]);
                }
                return viewObj;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------Get FieldUnit--------------------------------
        public PostLoginComplaint GetFielUnit()
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                PostLoginComplaint postLoginComplaint = new PostLoginComplaint();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select FieldUnitID as fieldUnitId,FieldUnitName as fieldUnitName from FieldUnit", conn);

                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    postLoginComplaint.fieldUnits = new List<FieldUnits>();
                    DataTable dt = ds.Tables[0];
                    postLoginComplaint.fieldUnits = Helper.ConvertDataTable<FieldUnits>(dt);
                }
                return postLoginComplaint;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //--------------------GetCompalintsSummaryReporte-----------------------------------
        public ViewModel GetCompalintsSummaryReporte(ViewDataModel viewDataModel)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                ViewModel viewObj = new ViewModel();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_ComplaintsSummary", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("circleId", viewDataModel.circleId);
                    cmd.Parameters.AddWithValue("regionId", viewDataModel.regionId);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    viewObj.postLoginComplaint = new PostLoginComplaint();
                    DataTable dt = ds.Tables[0];
                    viewObj.postLoginComplaint = Helper.GetItem<PostLoginComplaint>(dt.Rows[0]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    viewObj.complaintsSummaries = new List<ComplaintsSummary>();
                    DataTable dt = ds.Tables[1];
                    viewObj.complaintsSummaries = Helper.ConvertDataTable<ComplaintsSummary>(dt);
                }
                return viewObj;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //--------------------GetTotalRsolvedComplaints-----------------------------------
        public ViewModel GetUserPerformanceSummary(ViewDataModel viewDataModel)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                ViewModel viewObj = new ViewModel();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_UserPerformanceSummary", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("circleId", viewDataModel.circleId);
                    cmd.Parameters.AddWithValue("regionId", viewDataModel.regionId);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    viewObj.userPerformanceSummaries = new List<UserPerformanceSummary>();
                    DataTable dt = ds.Tables[0];
                    viewObj.userPerformanceSummaries = Helper.ConvertDataTable<UserPerformanceSummary>(dt);
                }
                return viewObj;
            }
            catch (Exception)
            {

                throw;
            }
        }


        //-------------------------Get All Vouchers----------------------------------------
        public IEnumerable<PremiumVoucher> GetAllVouchers(string policy)
        {
            //List<string> PolicyNo

            db _db = new db();
            var voucherslist = new List<PremiumVoucher>();

            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_SELECT_INSURANT_PaymentVoucher_For_Web", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VoucherType", "Renewal Premium");
                cmd.Parameters.AddWithValue("IID", DBNull.Value);
                cmd.Parameters.AddWithValue("PolicyNumber", policy);
                cmd.Parameters.AddWithValue("RegionID", DBNull.Value);
                cmd.Parameters.AddWithValue("FieldUnitName", DBNull.Value);
                cmd.Parameters.AddWithValue("Origin", DBNull.Value);
                conn.Open();
                //cmd.CommandTimeout = 10000000;

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var PremiumVouchers = new PremiumVoucher();
                    PremiumVouchers.SerialNo = Convert.ToInt64(dr["SerialNo"]);
                    PremiumVouchers.PolicyNumber = dr["PolicyNo"].ToString();
                    PremiumVouchers.LIR = dr["LIR"].ToString();
                    PremiumVouchers.Name = dr["Name"].ToString();
                    PremiumVouchers.FHName = dr["FhName"].ToString();
                    PremiumVouchers.PolicyTypeCode = dr["PolicyTypeCode"].ToString();
                    PremiumVouchers.PolicyTerm = dr["PolicyTerm"].ToString();
                    PremiumVouchers.Age = Convert.ToInt32(dr["Age"].ToString());
                    PremiumVouchers.DueDate = Convert.ToDateTime(dr["DueDate"]);
                    PremiumVouchers.RiskDate = Convert.ToDateTime(dr["RiskDate"]);
                    PremiumVouchers.SumAssured = Convert.ToInt64(dr["SumAssured"].ToString());
                    PremiumVouchers.PremiumMode = dr["PremiumMode"].ToString();
                    PremiumVouchers.IsDefence = Convert.ToBoolean(dr["IsDefence"]);
                    PremiumVouchers.FieldUnitName = dr["FieldUnitName"].ToString();
                    //PremiumVouchers.OriginalID = Convert.ToInt32(dr["OriginalID"].ToString());
                    PremiumVouchers.vouchertype = dr["vouchertype"].ToString();
                    PremiumVouchers.SeqNo = Convert.ToInt32(dr["SeqNo"].ToString());
                    PremiumVouchers.BookNo = Convert.ToInt64(dr["BookNo"].ToString());
                    PremiumVouchers.VoucherNo = dr["VoucherNo"].ToString();
                    PremiumVouchers.Amount = Convert.ToInt64(dr["Amount"]);
                    PremiumVouchers.IsPaid = Convert.ToBoolean(dr["IsPaid"].ToString());
                    //PremiumVouchers.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    PremiumVouchers._IsPaid = Convert.ToBoolean(dr["IsPaid"].ToString());
                    PremiumVouchers.ReceiptNo = dr["ReceiptNo"].ToString();


                    voucherslist.Add(PremiumVouchers);
                }

                conn.Close();
            }

            return voucherslist;
        }

        //---------------------------Get All First Premium Vouchers------------------------------
        public IEnumerable<PremiumVoucher> GetAllFirstPremiumVouchers(string policy)
        {
            //List<string> PolicyNo
            db _db = new db();
            var voucherslist = new List<PremiumVoucher>();

            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_SELECT_INSURANT_PaymentVoucher_FOR_WEB_First_Premium", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VoucherType", "first premium");
                cmd.Parameters.AddWithValue("IID", DBNull.Value);
                cmd.Parameters.AddWithValue("PolicyNumber", policy);
                cmd.Parameters.AddWithValue("RegionID", DBNull.Value);
                cmd.Parameters.AddWithValue("FieldUnitName", DBNull.Value);
                cmd.Parameters.AddWithValue("Origin", DBNull.Value);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var PremiumVouchers = new PremiumVoucher();
                    PremiumVouchers.SerialNo = Convert.ToInt64(dr["SerialNo"]);
                    PremiumVouchers.PolicyNumber = dr["PolicyNo"].ToString();
                    PremiumVouchers.LIR = dr["LIR"].ToString();
                    PremiumVouchers.Name = dr["Name"].ToString();
                    PremiumVouchers.FHName = dr["FhName"].ToString();
                    PremiumVouchers.PolicyTypeCode = dr["PolicyTypeCode"].ToString();
                    PremiumVouchers.PolicyTerm = dr["PolicyTerm"].ToString();
                    PremiumVouchers.Age = Convert.ToInt32(dr["Age"].ToString());
                    PremiumVouchers.DueDate = Convert.ToDateTime(dr["DueDate"]);
                    PremiumVouchers.RiskDate = Convert.ToDateTime(dr["RiskDate"]);
                    PremiumVouchers.SumAssured = Convert.ToInt64(dr["SumAssured"].ToString());
                    PremiumVouchers.PremiumMode = dr["PremiumMode"].ToString();
                    PremiumVouchers.IsDefence = Convert.ToBoolean(dr["IsDefence"]);
                    PremiumVouchers.FieldUnitName = dr["FieldUnitName"].ToString();
                    //if (!DBNull.Value.Equals(row[fieldName]))
                    //{
                    //    //not null
                    //}
                    //else
                    //{
                    //    //null
                    //}
                    //PremiumVouchers.OriginalID = Convert.ToInt32(dr["OriginalID"].ToString());
                    PremiumVouchers.OriginalID = '0';

                    PremiumVouchers.vouchertype = dr["vouchertype"].ToString();
                    PremiumVouchers.SeqNo = Convert.ToInt32(dr["SeqNo"].ToString());
                    PremiumVouchers.BookNo = Convert.ToInt64(dr["BookNo"].ToString());
                    PremiumVouchers.VoucherNo = dr["VoucherNo"].ToString();
                    PremiumVouchers.Amount = Convert.ToInt64(dr["Amount"]);
                    PremiumVouchers.IsPaid = Convert.ToBoolean(dr["IsPaid"].ToString());
                    //PremiumVouchers.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    PremiumVouchers._IsPaid = Convert.ToBoolean(dr["IsPaid"].ToString());
                    PremiumVouchers.ReceiptNo = dr["ReceiptNo"].ToString();


                    voucherslist.Add(PremiumVouchers);
                }

                conn.Close();
            }

            return voucherslist;
        }


        //-----------------------------Get All Late Payment Vouchers-----------------------------
        public IEnumerable<PremiumVoucher> GetAllLatePaymentVouchers(string policy)
        {
            //List<string> PolicyNo
            db _db = new db();
            var voucherslist = new List<PremiumVoucher>();

            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_SELECT_INSURANT_PaymentVoucher_FOR_WEB_Late_Payment", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VoucherType", "Late Payment");
                cmd.Parameters.AddWithValue("IID", DBNull.Value);
                cmd.Parameters.AddWithValue("PolicyNumber", policy);
                cmd.Parameters.AddWithValue("RegionID", DBNull.Value);
                cmd.Parameters.AddWithValue("FieldUnitName", DBNull.Value);
                cmd.Parameters.AddWithValue("Origin", DBNull.Value);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var PremiumVouchers = new PremiumVoucher();
                    PremiumVouchers.SerialNo = Convert.ToInt64(dr["SerialNo"]);
                    PremiumVouchers.PolicyNumber = dr["PolicyNo"].ToString();
                    PremiumVouchers.LIR = dr["LIR"].ToString();
                    PremiumVouchers.Name = dr["Name"].ToString();
                    PremiumVouchers.FHName = dr["FhName"].ToString();
                    PremiumVouchers.PolicyTypeCode = dr["PolicyTypeCode"].ToString();
                    PremiumVouchers.PolicyTerm = dr["PolicyTerm"].ToString();
                    PremiumVouchers.Age = Convert.ToInt32(dr["Age"].ToString());
                    PremiumVouchers.DueDate = Convert.ToDateTime(dr["DueDate"]);
                    PremiumVouchers.RiskDate = Convert.ToDateTime(dr["RiskDate"]);
                    PremiumVouchers.SumAssured = Convert.ToInt64(dr["SumAssured"].ToString());
                    PremiumVouchers.PremiumMode = dr["PremiumMode"].ToString();
                    PremiumVouchers.IsDefence = Convert.ToBoolean(dr["IsDefence"]);
                    PremiumVouchers.FieldUnitName = dr["FieldUnitName"].ToString();
                    //if (!DBNull.Value.Equals(row[fieldName]))
                    //{
                    //    //not null
                    //}
                    //else
                    //{
                    //    //null
                    //}
                    //PremiumVouchers.OriginalID = Convert.ToInt32(dr["OriginalID"].ToString());
                    PremiumVouchers.OriginalID = '0';

                    PremiumVouchers.vouchertype = dr["vouchertype"].ToString();
                    PremiumVouchers.SeqNo = Convert.ToInt32(dr["SeqNo"].ToString());
                    PremiumVouchers.BookNo = Convert.ToInt64(dr["BookNo"].ToString());
                    PremiumVouchers.VoucherNo = dr["VoucherNo"].ToString();
                    PremiumVouchers.Amount = Convert.ToInt64(dr["Amount"]);
                    PremiumVouchers.IsPaid = Convert.ToBoolean(dr["IsPaid"].ToString());
                    //PremiumVouchers.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    PremiumVouchers._IsPaid = Convert.ToBoolean(dr["IsPaid"].ToString());
                    PremiumVouchers.ReceiptNo = dr["ReceiptNo"].ToString();


                    voucherslist.Add(PremiumVouchers);
                }

                conn.Close();
            }

            return voucherslist;
        }



        //----------------------------GEt All Short Payment Vouchers---------------------------------
        public IEnumerable<PremiumVoucher> GetAllShortPaymentVouchers(string policy)
        {
            //List<string> PolicyNo
            db _db = new db();
            var voucherslist = new List<PremiumVoucher>();

            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_SELECT_INSURANT_PaymentVoucher_FOR_WEB_Short_Payment", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VoucherType", "Short Payment");
                cmd.Parameters.AddWithValue("IID", DBNull.Value);
                cmd.Parameters.AddWithValue("PolicyNumber", policy);
                cmd.Parameters.AddWithValue("RegionID", DBNull.Value);
                cmd.Parameters.AddWithValue("FieldUnitName", DBNull.Value);
                cmd.Parameters.AddWithValue("Origin", DBNull.Value);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var PremiumVouchers = new PremiumVoucher();
                    PremiumVouchers.SerialNo = Convert.ToInt64(dr["SerialNo"]);
                    PremiumVouchers.PolicyNumber = dr["PolicyNo"].ToString();
                    PremiumVouchers.LIR = dr["LIR"].ToString();
                    PremiumVouchers.Name = dr["Name"].ToString();
                    PremiumVouchers.FHName = dr["FhName"].ToString();
                    PremiumVouchers.PolicyTypeCode = dr["PolicyTypeCode"].ToString();
                    PremiumVouchers.PolicyTerm = dr["PolicyTerm"].ToString();
                    PremiumVouchers.Age = Convert.ToInt32(dr["Age"].ToString());
                    PremiumVouchers.DueDate = Convert.ToDateTime(dr["DueDate"]);
                    PremiumVouchers.RiskDate = Convert.ToDateTime(dr["RiskDate"]);
                    PremiumVouchers.SumAssured = Convert.ToInt64(dr["SumAssured"].ToString());
                    PremiumVouchers.PremiumMode = dr["PremiumMode"].ToString();
                    PremiumVouchers.IsDefence = Convert.ToBoolean(dr["IsDefence"]);
                    PremiumVouchers.FieldUnitName = dr["FieldUnitName"].ToString();
                    //if (!DBNull.Value.Equals(row[fieldName]))
                    //{
                    //    //not null
                    //}
                    //else
                    //{
                    //    //null
                    //}
                    //PremiumVouchers.OriginalID = Convert.ToInt32(dr["OriginalID"].ToString());
                    PremiumVouchers.OriginalID = '0';

                    PremiumVouchers.vouchertype = dr["vouchertype"].ToString();
                    PremiumVouchers.SeqNo = Convert.ToInt32(dr["SeqNo"].ToString());
                    PremiumVouchers.BookNo = Convert.ToInt64(dr["BookNo"].ToString());
                    PremiumVouchers.VoucherNo = dr["VoucherNo"].ToString();
                    PremiumVouchers.Amount = Convert.ToInt64(dr["Amount"]);
                    PremiumVouchers.IsPaid = Convert.ToBoolean(dr["IsPaid"].ToString());
                    //PremiumVouchers.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    PremiumVouchers._IsPaid = Convert.ToBoolean(dr["IsPaid"].ToString());
                    PremiumVouchers.ReceiptNo = dr["ReceiptNo"].ToString();

                    voucherslist.Add(PremiumVouchers);
                }

                conn.Close();
            }

            return voucherslist;
        }

        //---------------------------Get Loan Voucher------------------------------------
        public List<LoanVoucher> GetLoanVouchers(int iid)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<LoanVoucher> loanVoucherList = new List<LoanVoucher>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_SELECT_LOAN_VOUCHER_For_WebPortal", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("LoanAC", DBNull.Value);
                    cmd.Parameters.AddWithValue("IID", iid);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[1];
                    loanVoucherList = Helper.ConvertDataTable<LoanVoucher>(dt);
                }
                return loanVoucherList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //----------------------Get Laon Voucher Detail---------------------------------------
        public PrintLoanVoucher GetLoanVoucherDetail(string voucherNo, string iid)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                PrintLoanVoucher voucherDetail = new PrintLoanVoucher();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_SELECT_LoanVoucherDetail_For_WebPortal", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("VoucherNo", voucherNo);
                    cmd.Parameters.AddWithValue("IID", iid);
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
                    voucherDetail = Helper.GetItem<PrintLoanVoucher>(dt.Rows[0]);
                }
                return voucherDetail;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------------------Get Data--------------------------------------------
        public PostLoginComplaint GetPostComplaintsData(int iid)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                PostLoginComplaint postLoginComplaint = new PostLoginComplaint();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select Insurant.PolicyNo as policyNo,CAST(Insurant.FUID as varchar) as fieldUnitId,FieldUnit.FieldUnitName as fieldUnit from Insurant inner join  FieldUnit on Insurant.FUID = FieldUnit.FieldUnitID where Insurant.IID = '" + iid + "'", conn);
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
                    postLoginComplaint = Helper.GetItem<PostLoginComplaint>(dt.Rows[0]);
                }
                return postLoginComplaint;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //------------------------Get Pre Complaints---------------------------------------
        public List<PreComplaint> GetPreComplaints(UserRights userRights)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<PreComplaint> preComplaints = new List<PreComplaint>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_PreComplaints", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CircleId", userRights.CircleId);
                    cmd.Parameters.AddWithValue("RegionId", userRights.RegionId);
                    cmd.Parameters.AddWithValue("FieldUnitId", userRights.FieldUnitId);

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
                    preComplaints = Helper.ConvertDataTable<PreComplaint>(dt);
                }
                return preComplaints;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------- Get Post Complaints -----------------------------------
        public List<PostComplaint> GetPostComplaints(UserRights userRights)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<PostComplaint> postComplaint = new List<PostComplaint>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_PostComplaint", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CircleId", userRights.CircleId);
                    cmd.Parameters.AddWithValue("RegionId", userRights.RegionId);
                    cmd.Parameters.AddWithValue("FieldUnitId", userRights.FieldUnitId);
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
                    postComplaint = Helper.ConvertDataTable<PostComplaint>(dt);
                }
                return postComplaint;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------- Get Forwarded Complaints -----------------------------------
        public List<PostComplaint> GetForwardedComplaints(UserRights userRights)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<PostComplaint> postComplaint = new List<PostComplaint>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Forwarded_Complaints", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CircleId", userRights.CircleId);
                    cmd.Parameters.AddWithValue("RegionId", userRights.RegionId);
                    cmd.Parameters.AddWithValue("FieldUnitId", userRights.FieldUnitId);
                    cmd.Parameters.AddWithValue("GroupId", userRights.GroupId);
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
                    postComplaint = Helper.ConvertDataTable<PostComplaint>(dt);
                }
                return postComplaint;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //---------------------------Get Resolved Pre Complaints----------------------------------------------
        public List<PreComplaint> GetResolvedPreComplaints(UserRights userRights)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<PreComplaint> preComplaints = new List<PreComplaint>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Resolved_PreComplaint", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CircleId", userRights.CircleId);
                    cmd.Parameters.AddWithValue("RegionId", userRights.RegionId);
                    cmd.Parameters.AddWithValue("FieldUnitId", userRights.FieldUnitId);
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
                    preComplaints = Helper.ConvertDataTable<PreComplaint>(dt);
                }
                return preComplaints;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------Get Resolved Post Complaints-----------------------------------
        public List<PostComplaint> GetResolvedPostComplaints(UserRights userRights)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<PostComplaint> postComplaint = new List<PostComplaint>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Resolved_PostComplaint", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CircleId", userRights.CircleId);
                    cmd.Parameters.AddWithValue("RegionId", userRights.RegionId);
                    cmd.Parameters.AddWithValue("FieldUnitId", userRights.FieldUnitId);
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
                    postComplaint = Helper.ConvertDataTable<PostComplaint>(dt);
                }
                return postComplaint;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------Get Resolved Post Complaints-----------------------------------
        public List<PostComplaint> GetReopendComplaints(UserRights userRights)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<PostComplaint> postComplaint = new List<PostComplaint>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Reopend_Complaint", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CircleId", userRights.CircleId);
                    cmd.Parameters.AddWithValue("RegionId", userRights.RegionId);
                    cmd.Parameters.AddWithValue("FieldUnitId", userRights.FieldUnitId);
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
                    postComplaint = Helper.ConvertDataTable<PostComplaint>(dt);
                }
                return postComplaint;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //------------------------Resolved Pre Complaints---------------------------------------
        public string ResolvedPreComplaint(PreComplaint preComplaint)
        {
            try
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    string Sp = "";
                    Sp = "SP_PreComplaint_Resolved";
                    SqlCommand cmd = new SqlCommand(Sp, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("id", preComplaint.srNo);
                    cmd.Parameters.AddWithValue("@CNIC", preComplaint.cnic);
                    cmd.Parameters.AddWithValue("newPhoneNo", preComplaint.newPhoneNo);
                    cmd.Parameters.AddWithValue("resolveDate", preComplaint.resolveDate);
                    cmd.Parameters.AddWithValue("adminId", preComplaint.adminId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "OK";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        //--------------------------Get My  RegisteredComplaints -----------------------------------
        public List<PostComplaint> GetMyRegisteredComplaints(int adminId)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<PostComplaint> postComplaint = new List<PostComplaint>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_MyRegisteredComplaints", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("AdminId", adminId);
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
                    postComplaint = Helper.ConvertDataTable<PostComplaint>(dt);
                }
                return postComplaint;
            }
            catch (Exception)
            {

                throw;
            }
        }


        //--------------------Get Information-----------------------------------
        public ComplaintDetailViewModel GetComplaintDetail(int complaintId)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                ComplaintDetailViewModel complaintDetailViewObj = new ComplaintDetailViewModel();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Complaint_Detail", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("ComplaintId", complaintId);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    complaintDetailViewObj.postComplaint = new PostComplaint();
                    DataTable dt = ds.Tables[0];
                    complaintDetailViewObj.postComplaint = Helper.GetItem<PostComplaint>(dt.Rows[0]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    complaintDetailViewObj.complaintDocuments = new List<ComplaintDocument>();
                    DataTable dt = ds.Tables[1];
                    complaintDetailViewObj.complaintDocuments = Helper.ConvertDataTable<ComplaintDocument>(dt);
                }

                return complaintDetailViewObj;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //-------------------------------ResolvedPostComplaint-----------------------------------

        public string ResolvedPostComplaint(PostComplaint postComplaint)
        {
            try
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    string Sp = "";
                    Sp = "SP_PostComplaint_Resolved";
                    SqlCommand cmd = new SqlCommand(Sp, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("id", postComplaint.srNo);
                    cmd.Parameters.AddWithValue("policyNo", postComplaint.policyNo);
                    cmd.Parameters.AddWithValue("adminComments", postComplaint.adminComments);
                    cmd.Parameters.AddWithValue("adminId", postComplaint.adminId);
                    cmd.Parameters.AddWithValue("resloveDate", postComplaint.resolveDate);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "OK";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------------Add Forworded Complaint-----------------------------------

        public string AddForwordedComplaint(PostComplaint postComplaint)
        {
            try
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    string Sp = "";
                    Sp = "SP_Insert_Forwarded_Complaint";
                    SqlCommand cmd = new SqlCommand(Sp, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("complaintId", postComplaint.complaintId);
                    cmd.Parameters.AddWithValue("groupId", postComplaint.groupId);
                    cmd.Parameters.AddWithValue("groupNewId", postComplaint.groupNewId);
                    cmd.Parameters.AddWithValue("adminComments", postComplaint.adminComments);
                    cmd.Parameters.AddWithValue("adminId", postComplaint.adminId);
                    cmd.Parameters.Add("@retValue", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    int retval = (int)cmd.Parameters["@retValue"].Value;
                    conn.Close();
                    if (retval == 2)
                    {
                        return "failed";
                    }
                    return "success";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------------ResolvedPostComplaint-----------------------------------

        public string ReopenComplaint(CustomerPostComplaints customerPostComplaints)
        {
            try
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    string Sp = "";
                    Sp = "SP_Insert_Reopen_Complaint";
                    SqlCommand cmd = new SqlCommand(Sp, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("id", customerPostComplaints.complaintId);
                    cmd.Parameters.AddWithValue("complaintid", customerPostComplaints.complaint_Id);
                    cmd.Parameters.AddWithValue("reopenComments", customerPostComplaints.reopenComments);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "Success";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------------Generate Vouchar --------------------------------------------------------
        public string GenerateVouchar(string policy, int iid)
        {
            //List<string> PolicyNo
            db _db = new db();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                string Sp = "";
                Sp = "SP_INSERT_PAYMENTVOUCHER_For_WebPortal";
                SqlCommand cmd = new SqlCommand(Sp, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VoucherType", "Premium");
                cmd.Parameters.AddWithValue("IID", iid);
                cmd.Parameters.AddWithValue("UserID", iid);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return "OK";
            }
        }

        //----------------------------------Generate Late Payment Vouchar-------------------------------------------
        public string GenerateLPVouchar(string policy, int iid)
        {
            //List<string> PolicyNo
            db _db = new db();
            DateTime riskDate = new DateTime();
            DateTime periodFrom = new DateTime();
            DateTime periodTo = new DateTime();
            int interest = 0;
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                string policyQuery = "select RiskDate from Insurant where iid='" + iid + "'";
                SqlCommand command = new SqlCommand(policyQuery, conn);

                conn.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    riskDate = DateTime.Parse(dr.GetValue(0).ToString());
                }
                //dr.Close();
                conn.Close();
                //---------------------------------------------------------------------------------------------------------------
                SqlCommand cmdd = new SqlCommand("SP_CALCULATE_LATE_PAYMENTSLPDataChk", conn);
                cmdd.CommandType = CommandType.StoredProcedure;
                cmdd.Parameters.AddWithValue("PolicyNo", policy);
                cmdd.Parameters.AddWithValue("Date", riskDate);
                cmdd.Parameters.AddWithValue("Date2", DateTime.Now);
                cmdd.Parameters.AddWithValue("InterestRate", 12);
                conn.Open();
                SqlDataReader dr2 = cmdd.ExecuteReader();

                if (dr2.Read())
                {
                    conn.Close();
                    SqlCommand cmd1 = new SqlCommand("SP_CALCULATE_LATE_PAYMENTSLP", conn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("IID", iid);
                    cmd1.Parameters.AddWithValue("Date", riskDate);
                    cmd1.Parameters.AddWithValue("Date2", DateTime.Now);
                    conn.Open();

                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    while (dr1.Read())
                    {
                        periodFrom = DateTime.Parse(dr1.GetValue(0).ToString());
                        periodTo = DateTime.Parse(dr1.GetValue(1).ToString());
                        interest = Convert.ToInt32(dr1.GetValue(2));
                    }
                    conn.Close();
                    //----------------------------------------------------------------------------------------------------------------

                    SqlCommand cmd = new SqlCommand("SP_INSERT_PAYMENTVOUCHERlp_For_Web_Portal", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("VoucherType", "Premium");
                    cmd.Parameters.AddWithValue("IID", iid);
                    cmd.Parameters.AddWithValue("Date", riskDate);
                    cmd.Parameters.AddWithValue("Date2", DateTime.Now);
                    cmd.Parameters.AddWithValue("UserID", iid);
                    cmd.Parameters.AddWithValue("PeriodFrom", periodFrom);
                    cmd.Parameters.AddWithValue("PeriodTo", periodTo);
                    cmd.Parameters.AddWithValue("Interest", interest);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "OK";
                }
                else
                {
                    conn.Close();
                    return "Failed";
                }
            }
        }

        //-------------------------Generate Short Payment Voucher----------------------------------
        public string GenerateSPVouchar(string policy, int iid)
        {
            db _db = new db();
            DateTime riskDate = new DateTime();
            DateTime periodFrom = new DateTime();
            DateTime periodTo = new DateTime();
            int shortpayment = 0;
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                string policyQuery = "select RiskDate from Insurant where iid='" + iid + "'";
                SqlCommand command = new SqlCommand(policyQuery, conn);

                conn.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    riskDate = DateTime.Parse(dr.GetValue(0).ToString());
                }
                //dr.Close();
                conn.Close();
                //------------------------------------------------------------------------------
                SqlCommand cmd2 = new SqlCommand("SP_CALCULATE_PR_SHORT_PAYMENTVrGenDataCheck", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("IID", iid);
                cmd2.Parameters.AddWithValue("Date", riskDate);
                cmd2.Parameters.AddWithValue("Date2", DateTime.Now);
                cmd2.Parameters.AddWithValue("InterestRate", 12);
                conn.Open();
                SqlDataReader dr2 = cmd2.ExecuteReader();

                if (dr2.Read())
                {
                    conn.Close();
                    //----------------------------------------------------------------------------
                    SqlCommand cmd1 = new SqlCommand("SP_CALCULATE_PR_SHORT_PAYMENTVrGen", conn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("IID", iid);
                    cmd1.Parameters.AddWithValue("Date", riskDate);
                    cmd1.Parameters.AddWithValue("Date2", DateTime.Now);
                    cmd1.Parameters.AddWithValue("InterestRate", 12);
                    conn.Open();

                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    while (dr1.Read())
                    {
                        periodFrom = DateTime.Parse(dr1.GetValue(0).ToString());
                        periodTo = DateTime.Parse(dr1.GetValue(1).ToString());
                        shortpayment = Convert.ToInt32(dr1.GetValue(2)) + Convert.ToInt32(dr1.GetValue(3)) - Convert.ToInt32(dr1.GetValue(5));
                    }
                    conn.Close();
                    //----------------------------------------------------------------------------------------------------------------

                    SqlCommand cmd = new SqlCommand("SP_INSERT_PAYMENTVOUCHERsp_For_Web_Portal", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("VoucherType", "Premium");
                    cmd.Parameters.AddWithValue("IID", iid);
                    cmd.Parameters.AddWithValue("Date", riskDate);
                    cmd.Parameters.AddWithValue("Date2", DateTime.Now);
                    cmd.Parameters.AddWithValue("UserID", iid);
                    cmd.Parameters.AddWithValue("PeriodFrom", periodFrom);
                    cmd.Parameters.AddWithValue("PeriodTo", periodTo);
                    cmd.Parameters.AddWithValue("Interest", shortpayment);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "OK";
                }
                else
                {
                    conn.Close();
                    return "Failed";
                }
            }
        }

        //-----------------------------Generate Loan Voucher--------------------------------------
        public string GenerateLoanVouchar(int iid, string origion)
        {
            //List<string> PolicyNo
            db _db = new db();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                string Sp = "";
                Sp = "SP_GENERATE_LOAN_VOUCHER_For_WebPortal";
                SqlCommand cmd = new SqlCommand(Sp, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VoucherType", "Loan");
                cmd.Parameters.AddWithValue("IID", iid);
                // cmd.Parameters.AddWithValue("LoanAC", DBNull.Value);
                cmd.Parameters.AddWithValue("UserID", iid);
                cmd.Parameters.AddWithValue("FieldUnit", 0);
                cmd.Parameters.AddWithValue("Origin", origion);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                //using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                //{
                //    sda.Fill(ds);
                //}
                conn.Close();

                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    DataTable dt = ds.Tables[0];
                //    //ErrorMessage message = Helper.GetItem<ErrorMessage>(dt.Rows[3]);
                //   List<ErrorMessage> message = Helper.ConvertDataTable<ErrorMessage>(dt);
                //}
                return "OK";
            }
        }

        //-------------------------Insert PreLogin Complaint----------------------------------
        public string insertPreLoginComplaint(PreLoginComplaint preLoginComplaint)
        {
            //List<string> PolicyNo
            db _db = new db();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                string Sp = "";
                Sp = "SP_Insert_PortalPreLoginComplaint";
                SqlCommand cmd = new SqlCommand(Sp, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("cnic", preLoginComplaint.cnic);
                cmd.Parameters.AddWithValue("mobileNumber", preLoginComplaint.mobileNumber);
                cmd.Parameters.AddWithValue("region", preLoginComplaint.region);
                cmd.Parameters.Add("@retValue", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                int retval = (int)cmd.Parameters["@retValue"].Value;
                conn.Close();
                string complaintId = Convert.ToString(retval);

                return complaintId;
            }
        }

        //-------------------------Insert PreLogin Complaint----------------------------------
        public string insertMobileNumberComplaint(PostLoginComplaint postLoginComplaint)
        {
            db _db = new db();
            DataSet ds = new DataSet();
            //List<string> PolicyNo
            
            //--------------------------------Checking Duplication in PortalPreLoginComplaint table------------------------------------------
            string sqlqry = "select top 1 CNIC,Password from PortalPreLoginComplaint where CNIC='" + postLoginComplaint.cnicNo + "' and Password = '" + postLoginComplaint.newMobileNumber + "'";
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
                return "ComplaintExist";
            }
            else
            {
                
                using (SqlConnection conn1 = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd1 = new SqlCommand("select RegionID from FieldUnit where FieldUnitID='" + postLoginComplaint.fieldUnitId + "'", conn1);
                    conn1.Open();
                    //"03026928084"
                    SqlDataReader sdr = cmd1.ExecuteReader();

                    while (sdr.Read())
                    {
                        _regionId = sdr.GetValue(0).ToString();
                    }
                    conn1.Close();
                }
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    string Sp = "";
                    Sp = "SP_Insert_PortalPreLoginComplaint";
                    SqlCommand cmd = new SqlCommand(Sp, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("cnic", postLoginComplaint.cnicNo);
                    cmd.Parameters.AddWithValue("mobileNumber", postLoginComplaint.newMobileNumber);
                    cmd.Parameters.AddWithValue("region", _regionId);
                    cmd.Parameters.Add("@retValue", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    int retval = (int)cmd.Parameters["@retValue"].Value;
                    conn.Close();
                    string complaintId = Convert.ToString(retval);

                    return complaintId;
                }
            }



        }

        //-------------------------Insert PostLogin Complaint--------------------------------------
        public string insertPostLoginComplaint(PostLoginComplaint postLoginComplaint)
        {
            //List<string> PolicyNo
            db _db = new db();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                string Sp = "";
                Sp = "SP_Insert_PortalPostLoginComplaint";
                SqlCommand cmd = new SqlCommand(Sp, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("cnicNo", postLoginComplaint.cnicNo);
                cmd.Parameters.AddWithValue("policyNo", postLoginComplaint.policyNo);
                cmd.Parameters.AddWithValue("fieldUnit", Convert.ToInt32(postLoginComplaint.fieldUnitId));
                cmd.Parameters.AddWithValue("complaintType", postLoginComplaint.complaintType);
                cmd.Parameters.AddWithValue("complaintOrigin", postLoginComplaint.complaintOrigin);
                cmd.Parameters.AddWithValue("comments", postLoginComplaint.comments);
                cmd.Parameters.AddWithValue("adminId", postLoginComplaint.adminId);
                cmd.Parameters.Add("@retValue", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                int retval = (int)cmd.Parameters["@retValue"].Value;
                conn.Close();
                string complaintId = Convert.ToString(retval);
                return complaintId;
            }
        }

        //-------------------------Insert Customer Login Data--------------------------------------
        public string InsertCustomerLoginData(string cnic, string mobile, string password)
        {
            //List<string> PolicyNo
            db _db = new db();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_Insert_CustomertLogin_Info", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("CNIC", cnic);
                cmd.Parameters.AddWithValue("Phone", mobile);
                cmd.Parameters.AddWithValue("Password", password);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return "OK";
            }
        }

        //--------------------------Regenerate Voucher----------------------------------------------
        public string RegenerateVouchar(string policy, int iid, string option, string voucharno)
        {
            //List<string> PolicyNo
            db _db = new db();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                // string Sp = "";
                if (option == "All_Voucher")
                {
                    DeleteVouchar(iid, option, voucharno);
                    GenerateVouchar(policy, iid);
                }
                else if (option == "Late_Payment_Voucher")
                {
                    DeleteVouchar(iid, option, voucharno);
                    GenerateLPVouchar(policy, iid);
                }
                else if (option == "Short_Payment_Voucher")
                {
                    DeleteVouchar(iid, option, voucharno);
                    GenerateSPVouchar(policy, iid);
                }
                //Sp = "SP_INSERT_PAYMENTVOUCHER_For_WebPortal";
                //SqlCommand cmd = new SqlCommand(Sp, conn);
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("VoucherType", "Premium");
                //cmd.Parameters.AddWithValue("IID", iid);
                //cmd.Parameters.AddWithValue("UserID", iid);
                //conn.Open();
                //cmd.ExecuteNonQuery();
                //conn.Close();
                return "OK";
            }
        }

        //---------------------------Delete Voucher----------------------------------------------
        public void DeleteVouchar(int iid, string option, string voucharno)
        {
            if (option == "All_Voucher")
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("delete from paymentvoucher where iid='" + iid + "' and period_to > (select max(periodto) from premiumreceived where iid='" + iid + "')", conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            else if (option == "Late_Payment_Voucher")
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("update PaymentVoucherLP set IsDeleted='" + 1 + "' where iid='" + iid + "'", conn); /* and VoucherNumber='"+ voucharno +"'*/
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            else if (option == "Short_Payment_Voucher")
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("update PaymentVoucherLP set IsDeleted='" + 1 + "' where iid='" + iid + "'", conn); /* and VoucherNumber='"+ voucharno +"'*/
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        //---------------------------Get Policy Numbers------------------------------------
        public List<Policy> GetPolicyNo(string cnicNo)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<Policy> policies = new List<Policy>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select PolicyNo as policy from Insurant where NIC='" + cnicNo + "'", conn);

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
                    policies = Helper.ConvertDataTable<Policy>(dt);
                }
                return policies;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //---------------------------Get Regions------------------------------------
        public List<Region> GetRegions(int circleId)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<Region> regions = new List<Region>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select RegionID as regionId, RegionName as regionName from Region where CircleID='" + circleId + "'", conn);

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
                    regions = Helper.ConvertDataTable<Region>(dt);
                }
                return regions;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //---------------------------Get Field Unit List------------------------------------
        public List<FieldUnits> GetFielUnitList(int regionId)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<FieldUnits> fieldUnits = new List<FieldUnits>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select FieldUnitID as fieldUnitId,FieldUnitName as fieldUnitName from FieldUnit where RegionID='" + regionId + "'", conn);

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
                    fieldUnits = Helper.ConvertDataTable<FieldUnits>(dt);
                }
                return fieldUnits;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //------------------------------Get AdminUsers---------------------------------
        public List<SystemUser> GetAdminUsers(int regionId)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<SystemUser> systemUsers = new List<SystemUser>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select UserID as UserId,UserName as UserName from WebPortalAdminLogin where RegionId='" + regionId + "'", conn);

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
                    systemUsers = Helper.ConvertDataTable<SystemUser>(dt);
                }
                return systemUsers;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //---------------------------Get FieldUnit------------------------------------
        public Details GetFiedUnitAndMobileNumber(string policyNo)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                Details details = new Details();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select i.FUID as fieldUnitId,fu.FieldUnitName as fieldUnitName,i.Phone as mobileNumber from Insurant i inner join FieldUnit fu on i.FUID=fu.FieldUnitID where i.PolicyNo='" + policyNo + "'", conn);
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
                    details = Helper.GetItem<Details>(dt.Rows[0]);
                }
                return details;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //---------------------------Get Regions------------------------------------
        public List<Region> GetRegions()
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<Region> regions = new List<Region>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select RegionID as regionId,RegionName as regionName from Region", conn);
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
                    regions = Helper.ConvertDataTable<Region>(dt);
                }
                return regions;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //---------------------------Get Circles------------------------------------
        public List<Circle> GetCircles()
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<Circle> circles = new List<Circle>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select CircleID as CircleId,CircleName as CirclelName from Circle", conn);
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
                    circles = Helper.ConvertDataTable<Circle>(dt);
                }
                return circles;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------Insert SysUser Info--------------------------------------
        public string InsertSysUserInfo(SystemUser systemUser)
        {
            //List<string> PolicyNo;
            db _db = new db();
            DataSet ds = new DataSet();
            //using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            //{
            //    SqlCommand cmd = new SqlCommand("select * from WebPortalAdminLogin where UserName='" + systemUser.UserName + "' or PhoneNo='" + systemUser.PhoneNo + "'", conn);
            //    conn.Open();
            //    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            //    {
            //        sda.Fill(ds);
            //    }
            //    conn.Close();
            //}
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    return "Failed";
            //}
            //else
            //{
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_Insert_SystemUser_Info", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserName", systemUser.UserName);
                cmd.Parameters.AddWithValue("FullName", systemUser.FullName);
                cmd.Parameters.AddWithValue("CNIC", systemUser.CNIC);
                cmd.Parameters.AddWithValue("PhoneNo", systemUser.PhoneNo);
                cmd.Parameters.AddWithValue("Designation", systemUser.Designation);
                cmd.Parameters.AddWithValue("Circle", systemUser.circleId);
                cmd.Parameters.AddWithValue("Region", systemUser.RegionId);
                cmd.Parameters.AddWithValue("FiedUnit", systemUser.FieldUnitId);
                cmd.Parameters.AddWithValue("Level", systemUser.Level);
                cmd.Parameters.AddWithValue("GroupId", systemUser.GroupId);
                cmd.Parameters.Add("@retValue", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                conn.Open();
                cmd.ExecuteNonQuery();
                int retval = (int)cmd.Parameters["@retValue"].Value;
                conn.Close();
                if (retval == 2)
                {
                    return "Failed";
                }
                return "OK";
            }
            //}

        }

        //-------------------------GetPostComplaintList-----------------------------------
        public List<CustomerPostComplaints> GetPostComplaintList(string cnicNo)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<CustomerPostComplaints> customerPostComplaints = new List<CustomerPostComplaints>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_Customer_PostComplaint", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CNIC", cnicNo);

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
                    customerPostComplaints = Helper.ConvertDataTable<CustomerPostComplaints>(dt);
                }
                return customerPostComplaints;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------Get SysUsers List-----------------------------------
        public List<SystemUser> GetSysUsers()
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<SystemUser> systemUsers = new List<SystemUser>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_CMSSysUsers", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

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
                    systemUsers = Helper.ConvertDataTable<SystemUser>(dt);
                }
                return systemUsers;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------Get customer List-----------------------------------
        public List<Customer> GetCustomer()
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<Customer> customers = new List<Customer>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select Id as id,CNIC as cnic,PhoneNo as phoneno,Password as password from WebPortalCustomerLogin", conn);
                    //cmd.CommandType = CommandType.StoredProcedure;

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
                    customers = Helper.ConvertDataTable<Customer>(dt);
                }
                return customers;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //------------------------UpdateSysUserInfo---------------------------------------
        public string UpdateSysUserInfo(SystemUser systemUser, string password)
        {
            try
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    string Sp = "";
                    if (systemUser.GroupId != null)
                    {
                        Sp = "update WebPortalAdminLogin set PhoneNo='" + systemUser.PhoneNo + "', Password='" + password + "', GroupID='" + systemUser.GroupId + "' where UserID='" + systemUser.UserId + "'";
                    }
                    else
                    {
                        Sp = "update WebPortalAdminLogin set PhoneNo='" + systemUser.PhoneNo + "', Password='" + password + "' where UserID='" + systemUser.UserId + "'";
                    }
                    SqlCommand cmd = new SqlCommand(Sp, conn);
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "OK";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //------------------------UpdateCustomerInfo---------------------------------------
        public string UpdateCustomerInfo(Customer customer, string password)
        {
            try
            {
                db _db = new db();
                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    string Sp = "";
                    Sp = "update WebPortalCustomerLogin set Password='" + password + "' where Id='" + customer.id + "'";
                    SqlCommand cmd = new SqlCommand(Sp, conn);
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "OK";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //-------------------------Get GroupList List-----------------------------------
        public List<Group> GetGroupList()
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                List<Group> groups = new List<Group>();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("select GroupID,GroupDescription from [dbo].[Group] where GroupID=2 or GroupID=4 or GroupID=7 or GroupID=8 or GroupID=9 or GroupID=3 or GroupID=10 order by GroupID asc", conn);
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
                    groups = Helper.ConvertDataTable<Group>(dt);
                }
                return groups;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //----------------------------SetCustomerComplaintStatus-----------------------------
        public string SetCustomerComplaintStatus(int complaintId)
        {
            //List<string> PolicyNo
            db _db = new db();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("update PortalPostLoginComplaint set IsClosed='" + 1 + "' where PostComplaintID='" + complaintId + "'", conn);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return "OK";
            }
        }

        //----------------------------Insert Upload Documnets-----------------------------
        public string InsertUploadDocumnets(string complaintId, string fileName)
        {
            //List<string> PolicyNo
            db _db = new db();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("insert into WebPortalDocument(ComplaintId,EntryDate,DocumentName) values('" + complaintId + "',GETDATE(),'" + fileName + "')", conn);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return "OK";
            }
        }

        //--------------------Get Complaint Forwardig Details -----------------------------------
        public ViewForwardedComplaintDataModel GetComplaintForwardigDetails(int complaintId)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                ViewForwardedComplaintDataModel viewObj = new ViewForwardedComplaintDataModel();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_ComplaintForwardingDetails", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("complaintId", complaintId);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    viewObj.postComplaint = new PostComplaint();
                    DataTable dt = ds.Tables[0];
                    viewObj.postComplaint = Helper.GetItem<PostComplaint>(dt.Rows[0]);
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    viewObj.forwardComplaintHistories = new List<ForwardComplaintHistory>();
                    DataTable dt = ds.Tables[1];
                    viewObj.forwardComplaintHistories = Helper.ConvertDataTable<ForwardComplaintHistory>(dt);
                }
                return viewObj;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ViewForwardedComplaintDataModel GetTrackingDetails(int TrackingId)
        {
            try
            {
                db _db = new db();
                DataSet ds = new DataSet();
                ViewForwardedComplaintDataModel viewObj = new ViewForwardedComplaintDataModel();

                using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("SP_Select_ComplaintForwardingDetails", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("complaintId", TrackingId);
                    conn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    conn.Close();
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    viewObj.postComplaint = new PostComplaint();
                    DataTable dt = ds.Tables[0];
                    viewObj.postComplaint = Helper.GetItem<PostComplaint>(dt.Rows[0]);
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    viewObj.forwardComplaintHistories = new List<ForwardComplaintHistory>();
                    DataTable dt = ds.Tables[1];
                    viewObj.forwardComplaintHistories = Helper.ConvertDataTable<ForwardComplaintHistory>(dt);
                }
                return viewObj;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //--------------------------------SMS Log------------------------------------------------
        public void SmsLog(string phone, int smsType, string text)
        {
            db _db = new db();
            using (SqlConnection conn = new SqlConnection(_db.GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("SP_Insert_smsLog_For_WebPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("phone", phone);
                cmd.Parameters.AddWithValue("smsType", smsType);
                cmd.Parameters.AddWithValue("text", text);

                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
