using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Chola.Filters;
using Chola.Models;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.IO;
using System.Data.Objects.SqlClient;

namespace Chola.Controllers
{
    public class MiscellaneousController : Controller
    {
        #region CommonFunctions
       

        #endregion

        #region Miscellaneous

        public ActionResult ListEntries(string Success)
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = Success;
                    }
                    catch
                    {

                    }
                    var count = context.OthersType.AsEnumerable().Count();
                    if (count > 0)
                    {
                        ViewBag.ServiceList = context.OthersType.ToList().Select(c => new SelectListItem
                         {
                             Value = c.OthersTypeCode.ToString(),
                             Text = c.OtherTypeName
                         });
                    }
                    else
                    {
                        ViewBag.ServiceList = new SelectListItem
                        {
                            Text = "",
                            Value = ""
                        };
                    }

                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        private DataSet ReturnQueryEntry(string OthersTypeCode)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[5];
                sqlParams[0] = new SqlParameter("@OthersCode", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
                sqlParams[1] = new SqlParameter("@OthersTypeCode", SqlDbType.Int);
                if (Shared.ToString(OthersTypeCode).Length > 0)
                    sqlParams[1].Value = Shared.ToInt(OthersTypeCode);
                else
                    sqlParams[1].Value = DBNull.Value;
               
                sqlParams[2] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[2].Value = WebSecurity.CurrentUserId;
                sqlParams[3] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[3].Value = "ListEntries";
                sqlParams[4] = new SqlParameter("@ParmType", SqlDbType.Int);
                sqlParams[4].Value = 2;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Miscellaneous", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<EntryListModel> GetListEntry(string OthersTypeCode)
        {
            DataSet ds = ReturnQueryEntry(OthersTypeCode);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new EntryListModel
                {
                    OthersCode = Shared.ToString(dataRow.Field<Int32>("OthersCode")),
                    OthersTypeName = dataRow.Field<string>("OtherTypeName"),
                    Charge = Shared.ToString(dataRow.Field<decimal>("Charge")),
                    Remarks = dataRow.Field<string>("Remarks"),
                    TrxnDate = dataRow.Field<string>("CreatedDate")
                });
                return trxnList.ToList();
            }
            else

                return new List<EntryListModel>();
        }

        public JsonResult QueryEntryList(string OthersTypeCode)
        {
            try
            {
                List<EntryListModel> query = GetListEntry(OthersTypeCode);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.OthersCode, x.OthersTypeName, x.Charge, x.Remarks, x.TrxnDate })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region CreateEditEntry
        public ActionResult CreateEntry(string Success)
        {
            using (Entities context = new Entities())
            {
                CreateEntryModel model = new CreateEntryModel();

                ViewBag.ServiceList = context.OthersType.ToList().Select(c => new SelectListItem
                {
                    Value = c.OthersTypeCode.ToString(),
                    Text = c.OtherTypeName
                });             

                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEntry(CreateEntryModel model, FormCollection frm)
        {
            using (Entities context = new Entities())
            {
                int _affectedRows = 0;
                try
                {
                    // Has permission to view the page?
                    if (!Request.IsAuthenticated)
                    {
                        return RedirectToAction("LogOn", "Account");
                    }
                    else
                    {
                        //if (Shared.ToString(model.OthersTypeCode).Length == 0) ModelState.AddModelError(string.Empty, "Service Type is Required.");

                        if (ModelState.IsValid)
                        {
                            try
                            {
                                _affectedRows = context.sp_frm_add_upd_Miscellaneous(
                                    (int?)null,
                               Shared.ToInt(model.OthersTypeCode),
                               Shared.ToDecimal(model.Charge),
                                model.Remarks,
                                WebSecurity.CurrentUserId,
                                "CreateEntry"
                                );

                                return RedirectToAction("ListEntries", "Miscellaneous", new { Success = "Successfully Added" });

                            }
                            catch (Exception e)
                            {
                                ModelState.AddModelError(string.Empty, e.Message);
                            }

                        }
                        else
                        {
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);

                    return View(model);
                }

                finally
                {
                    ViewBag.ServiceList = context.OthersType.ToList().Select(c => new SelectListItem
                    {
                        Value = c.OthersTypeCode.ToString(),
                        Text = c.OtherTypeName
                    });
                }
                return View(model);
            }
        }

        public ActionResult EditEntry(string EntryId, string Success)
        {
            using (Entities context = new Entities())
            {
                CreateEntryModel model = new CreateEntryModel();

                try
                {
                    ViewBag.Message = Success;
                    int? _Entryid = Shared.ToInt(EntryId);

                    model = (from c in context.sp_frm_get_Miscellaneous(_Entryid, null, WebSecurity.CurrentUserId, "ListEntries", 1, null, null)
                             select new CreateEntryModel
                             {
                                 OthersCode = c.OthersCode,
                                 OthersTypeCode = Shared.ToString(c.OthersTypeCode),
                                 Charge = Shared.ToString(c.Charge),
                                 Remarks = c.Remarks
                             }).FirstOrDefault(i => i.OthersCode == _Entryid);

                }
                catch (Exception ex)
                {
                    return View(model);
                }
                finally
                {
                    ViewBag.ServiceList = context.OthersType.ToList().Select(c => new SelectListItem
                    {
                        Value = c.OthersTypeCode.ToString(),
                        Text = c.OtherTypeName
                    }); 

                }
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditEntry(CreateEntryModel model, FormCollection frm)
        {
            using (Entities context = new Entities())
            {
                int _affectedRows = 0;
                try
                {
                    // Has permission to view the page?
                    if (!Request.IsAuthenticated)
                    {
                        return RedirectToAction("LogOn", "Account");
                    }
                    else
                    {                        

                        if (ModelState.IsValid)
                        {
                            try
                            {
                                _affectedRows = context.sp_frm_add_upd_Miscellaneous(
                                   model.OthersCode,
                              Shared.ToInt(model.OthersTypeCode),
                              Shared.ToDecimal(model.Charge),
                               model.Remarks,
                               WebSecurity.CurrentUserId,
                               "CreateEntry"
                               );

                                return RedirectToAction("ListEntries", "Miscellaneous", new { Success = "Successfully Updated" });
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);

                                return View(model);
                            }

                        }
                        else
                        {
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);

                    return View(model);
                }

                finally
                {
                    ViewBag.ServiceList = context.OthersType.ToList().Select(c => new SelectListItem
                    {
                        Value = c.OthersTypeCode.ToString(),
                        Text = c.OtherTypeName
                    });
                }                
            }
        }

        //[HttpPost]
        //public ActionResult DeleteEntry(string EntryID)
        //{
        //    using (Entities context = new Entities())
        //    {
        //        try
        //        {
        //            MembershipEntry mu = Membership.GetEntry(EntryName);
        //            mu.ChangePassword(mu.ResetPassword(), Password);

        //            return Json(new { success = true, response = "Password Successfully Updated!" });
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
        //        }
        //    }
        //}
        #endregion

        #region Invoice
        public ActionResult Invoice()
        {
            IEnumerable<InvoiceModel> model = null;

            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.ServiceList = context.OthersType.ToList().Select(c => new SelectListItem
                        {
                            Value = c.OthersTypeCode.ToString(),
                            Text = c.OtherTypeName
                        }); 
                    }
                    catch
                    {

                    }
                    finally
                    {
                        
                    }
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        public class DetailsModel
        {
            public string Services { get; set; }
            public string Costs { get; set; }
        } 

        public ActionResult InvoicePrint(string CustomerName, string Service, string Cost, string Total)
        {           

            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.CustomerName = CustomerName;
                        ViewBag.Total = Total;

                        List<DetailsModel> model = new List<DetailsModel>();

                        var numbersAndWords = Service.Split(',').Zip(Cost.Split(','), (n, w) => new { Services = n, Costs = w });

                        foreach (var nw in numbersAndWords)
                        {
                            model.Add(new DetailsModel { Services = nw.Services, Costs = nw.Costs });
                        }                       

                        ViewBag.Details = model.ToList();

                    }
                    catch
                    {

                    }
                    finally
                    {

                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }
        #endregion

    }
}
