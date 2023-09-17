using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Chola.Models;
using WebMatrix.WebData;

namespace Chola.Controllers
{
    public class PayRollsController : Controller
    {
        #region CommonFunctions


        #endregion

        #region ListVouchers
        public ActionResult ListVouchers(string Success)
        {
            ViewBag.Message = Success;
            return View();
            
        }

        public JsonResult QueryVouchersList(string MonthYear)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    List<VouchersListModel> query = new List<VouchersListModel>();
                   
                    query = context.PaySlips.Where(s => s.MonthYear.Contains(MonthYear)).AsEnumerable().Select(dataRow => new VouchersListModel
                    {
                        PaySlipCode = dataRow.PaySlipCode,
                        PayTo = dataRow.PayTo,
                        MonthYear = dataRow.MonthYear,
                        NetPay = dataRow.GrossPay - dataRow.TotalDeductions + dataRow.TotalAdditions,
                        PayDay = dataRow.PayDay,
                        ApprovedBy = dataRow.ApprovedBy
                    }).ToList();

                    return Json(new
                    {
                        aaData = query.Select(x => new[] { Shared.ToString(x.PaySlipCode), x.PayTo, x.MonthYear, String.Format("{0:n}", x.NetPay), Shared.ToString(x.PayDay), x.ApprovedBy, Shared.ToString(x.PaySlipCode) })
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Result = "ERROR", Message = ex.Message });
                }
            }
        }       


        public ActionResult Invoice(string VoucherId)
        {
            using (Entities context = new Entities())
            {
                int? _VoucherCode = Shared.ToInt(VoucherId);
                var Voucher = (from c in context.PaySlips where c.PaySlipCode == _VoucherCode select c).FirstOrDefault();
                var _netpay = Shared.ToDecimal(Voucher.GrossPay) - Shared.ToDecimal(Voucher.TotalDeductions) + Shared.ToDecimal(Voucher.TotalAdditions);
                ViewBag.VoucherCode = Shared.ToString(Voucher.PaySlipCode);
                ViewBag.NetPay = Shared.ToString(_netpay);
                ViewBag.PayTo = Shared.ToString(Voucher.PayTo);
                ViewBag.FINNo = Shared.ToString(Voucher.FINNo);
                ViewBag.MonthYear = Shared.ToString(Voucher.MonthYear);
                ViewBag.BasicPay = Shared.ToString(Voucher.BasicPay);
                ViewBag.OverTime = Shared.ToString(Voucher.OverTime);
                ViewBag.Commission = Shared.ToString(Voucher.Commission);
                ViewBag.Allowance = Shared.ToString(Voucher.Allowance);
                ViewBag.GrossPay = Shared.ToString(Voucher.GrossPay);
                ViewBag.CPF = Shared.ToString(Voucher.CPF);
                ViewBag.MBF = Shared.ToString(Voucher.MBF);
                ViewBag.Advance = Shared.ToString(Voucher.Advance);
                ViewBag.IncomeTax = Shared.ToString(Voucher.IncomeTax);
                ViewBag.TotalDeductions = Shared.ToString(Voucher.TotalDeductions);
                ViewBag.Reembursement = Shared.ToString(Voucher.Reembursement);
                ViewBag.TotalAdditions = Shared.ToString(Voucher.TotalAdditions);
                ViewBag.ApprovedBy = Shared.ToString(Voucher.ApprovedBy);
                ViewBag.PayDay = Shared.ToString(Voucher.PayDay.Value.ToString("dd MMM yyyy"));

                return View();
            }
        }

        #endregion

        #region CreateEditVoucher
        public ActionResult CreateVoucher(string Success)
        {
            using (Entities context = new Entities())
            {
                CreateVoucherModel model = new CreateVoucherModel();
               

                int maxId = 0;
                var _maxId = "";
                var count = context.PaySlips.AsEnumerable().Count();
                if (count > 0)
                {
                    try
                    {
                        maxId = context.PaySlips.Max(p => p.PaySlipCode);
                        _maxId = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("D2") + (Shared.ToInt(count) + 1).ToString("D4") + Shared.ToString(WebSecurity.CurrentUserId);
                    }

                    catch (Exception ex)
                    {
                        maxId = 0;
                    }
                }
                else
                {
                    _maxId = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("D2") + "0001" + Shared.ToString(WebSecurity.CurrentUserId);
                }
                maxId = maxId + 1;
                model.PaySlipCode = Shared.ToInt(_maxId);
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVoucher(CreateVoucherModel model, FormCollection frm)
        {
            using (Entities context = new Entities())
            {
                
                try
                {
                    // Has permission to view the page?
                    if (!Request.IsAuthenticated)
                    {
                        return RedirectToAction("LogOn", "Account");
                    }
                    else
                    {
                        if (Shared.ToString(model.PayTo) == "") ModelState.AddModelError("PayTo", "Name is Required.");
                        if (Shared.ToString(model.MonthYear) == "") ModelState.AddModelError("MonthYear", "Month Year is Required.");
                        if (!model.PayDay.HasValue) ModelState.AddModelError("PayDay", "Pay Date is Required.");

                        if (ModelState.IsValid)
                        {
                            try
                            {
                                PaySlip tbl = new PaySlip();
                                tbl.PaySlipCode = Shared.ToInt(model.PaySlipCode);
                                tbl.MonthYear = model.MonthYear;
                                tbl.PayTo = model.PayTo;
                                tbl.FINNo = model.FINNo;
                                tbl.BasicPay = Shared.ToDecimal(model.BasicPay);
                                tbl.OverTime = Shared.ToDecimal(model.OverTime);
                                tbl.Commission = Shared.ToDecimal(model.Commission);
                                tbl.Allowance = Shared.ToDecimal(model.Allowance);
                                tbl.GrossPay = Shared.ToDecimal(model.BasicPay) + Shared.ToDecimal(model.OverTime) + Shared.ToDecimal(model.Commission) + Shared.ToDecimal(model.Allowance);
                                tbl.CPF = Shared.ToDecimal(model.CPF);
                                tbl.MBF = Shared.ToDecimal(model.MBF);
                                tbl.Advance = Shared.ToDecimal(model.Advance);
                                tbl.IncomeTax = Shared.ToDecimal(model.IncomeTax);
                                tbl.TotalDeductions = Shared.ToDecimal(model.CPF) + Shared.ToDecimal(model.MBF) + Shared.ToDecimal(model.Advance) + Shared.ToDecimal(model.IncomeTax);
                                tbl.Reembursement = Shared.ToDecimal(model.Reembursement);
                                tbl.TotalAdditions = Shared.ToDecimal(model.Reembursement);
                                tbl.ApprovedBy = Shared.ToString(model.ApprovedBy);
                                tbl.PayDay = model.PayDay;
                                tbl.CreatedBy = WebSecurity.CurrentUserId;
                                tbl.CreatedDate = DateTime.Now;

                                context.PaySlips.Add(tbl);
                                context.SaveChanges();

                                return RedirectToAction("ListVouchers", "PayRolls", new { Success = "Successfully Added" });
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
                   

                }
                return View(model);
            }
        }

        public ActionResult EditVoucher(string VoucherId, string Success)
        {
            using (Entities context = new Entities())
            {
                EditVoucherModel model = new EditVoucherModel();

                try
                {
                    ViewBag.Message = Success;
                    int? _Voucherid = Shared.ToInt(VoucherId);

                    var entity = context.PaySlips.FirstOrDefault(item => item.PaySlipCode == _Voucherid);

                    // Validate entity is not null
                    if (entity != null)
                    {
                        model.PaySlipCode = entity.PaySlipCode;
                        model.MonthYear = entity.MonthYear;
                        model.PayTo = entity.PayTo;
                        model.FINNo = entity.FINNo;
                        model.BasicPay = Shared.ToDecimal(entity.BasicPay);
                        model.OverTime = Shared.ToDecimal(entity.OverTime);
                        model.Commission = Shared.ToDecimal(entity.Commission);
                        model.Allowance = Shared.ToDecimal(entity.Allowance);
                        model.GrossPay = Shared.ToDecimal(entity.GrossPay);
                        model.CPF = Shared.ToDecimal(entity.CPF);
                        model.MBF = Shared.ToDecimal(entity.MBF);
                        model.Advance = Shared.ToDecimal(entity.Advance);
                        model.IncomeTax = Shared.ToDecimal(entity.IncomeTax);
                        model.TotalDeductions = Shared.ToDecimal(entity.TotalDeductions);
                        model.Reembursement = Shared.ToDecimal(entity.Reembursement);
                        model.TotalAdditions = Shared.ToDecimal(entity.Reembursement);
                        model.ApprovedBy = Shared.ToString(entity.ApprovedBy);
                        model.PayDay = entity.PayDay;                       
                    }

                }
                catch (Exception ex)
                {
                    return View(model);
                }
                finally
                {                   

                }
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditVoucher(EditVoucherModel model, FormCollection frm)
        {
            using (Entities context = new Entities())
            {
                
                try
                {
                    // Has permission to view the page?
                    if (!Request.IsAuthenticated)
                    {
                        return RedirectToAction("LogOn", "Account");
                    }
                    else
                    {
                        if (Shared.ToString(model.PayTo) == "") ModelState.AddModelError("PayTo", "Name is Required.");
                        if (Shared.ToString(model.MonthYear) == "") ModelState.AddModelError("MonthYear", "Month Year is Required.");
                        if (!model.PayDay.HasValue) ModelState.AddModelError("PayDay", "Pay Date is Required.");

                        if (ModelState.IsValid)
                        {
                            try
                            {
                                var entity = context.PaySlips.FirstOrDefault(item => item.PaySlipCode == model.PaySlipCode);

                                // Validate entity is not null
                                if (entity != null)
                                {
                                    
                                    entity.MonthYear = model.MonthYear;
                                    entity.PayTo = model.PayTo;
                                    entity.FINNo = model.FINNo;
                                    entity.BasicPay = Shared.ToDecimal(model.BasicPay);
                                    entity.OverTime = Shared.ToDecimal(model.OverTime);
                                    entity.Commission = Shared.ToDecimal(model.Commission);
                                    entity.Allowance = Shared.ToDecimal(model.Allowance);
                                    entity.GrossPay = Shared.ToDecimal(model.BasicPay) + Shared.ToDecimal(model.OverTime) + Shared.ToDecimal(model.Commission) + Shared.ToDecimal(model.Allowance);
                                    entity.CPF = Shared.ToDecimal(model.CPF);
                                    entity.MBF = Shared.ToDecimal(model.MBF);
                                    entity.Advance = Shared.ToDecimal(model.Advance);
                                    entity.IncomeTax = Shared.ToDecimal(model.IncomeTax);
                                    entity.TotalDeductions = Shared.ToDecimal(model.CPF) + Shared.ToDecimal(model.MBF) + Shared.ToDecimal(model.Advance) + Shared.ToDecimal(model.IncomeTax);
                                    entity.Reembursement = Shared.ToDecimal(model.Reembursement);
                                    entity.TotalAdditions = Shared.ToDecimal(model.Reembursement);
                                    entity.ApprovedBy = Shared.ToString(model.ApprovedBy);
                                    entity.PayDay = model.PayDay;
                                    context.SaveChanges();
                                }

                                return RedirectToAction("ListVouchers", "PayRolls", new { Success = "Successfully Updated" });
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
                  

                }
            }
        }

       
        #endregion

    }
}
