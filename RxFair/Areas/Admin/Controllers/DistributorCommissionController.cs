using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using static RxFair.Dto.Enum.GlobalEnums;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin), Area("Admin")]
    public class DistributorCommissionController : BaseController<DistributorCommissionController>
    {

        #region Fields
        private readonly ICommissionHistoryService _commissionHistory;
        private readonly IDistributorOrderChargeService _orderCharge;
        private readonly IInvoiceService _invoice;
        private readonly IInvoicePaymentService _invoicePayment;
        private readonly EmailService _emailService;

        #endregion

        #region Ctor
        public DistributorCommissionController(IOptions<EmailSettingsGmail> emailSettingsGmail, IInvoiceService invoice, IInvoicePaymentService invoicePayment, ICommissionHistoryService commissionHistory, IDistributorOrderChargeService orderCharge)
        {
            _invoice = invoice;
            _emailService = new EmailService(emailSettingsGmail);
            _invoicePayment = invoicePayment;
            _commissionHistory = commissionHistory;
            _orderCharge = orderCharge;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDistributorCommissionList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                var allList = await _commissionHistory.GetDistributorCommissionList(parameters.ToArray());

                var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetDistributorCommissionList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        [HttpGet]
        public IActionResult CommissionInvoicePayment(long id,long invoiceId)
        {
            ViewBag.distributorId = id;
            ViewBag.InvoiceId = invoiceId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCommissionInvoicePaymentList(long id, long invoiceId,JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = id });
                parameters.Parameters.Insert(1, new SqlParameter("@InvoiceId", SqlDbType.BigInt) { Value = invoiceId });
                var allList = await _commissionHistory.GetCommissionInvoicePaymentList(parameters.Parameters.ToArray());

                var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetCommissionInvoicePaymentList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        [HttpGet]
        public IActionResult AddInvoicePayment(long id)
        {
            ViewBag.InvoiceIdList = _commissionHistory.GetAll().Where(x => x.CommissionStatus != (short)PaymentStatus.Completed).Select(x => new SelectListItem() { Text = x.Id.ToString(), Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            return View(@"Components/_AddInvoicePayment");
        }

        public async Task<IActionResult> GetPendingCommissioninfo(long id)
        {
            try
            {
                var parameters = new List<SqlParameter> { new SqlParameter("@CommissionId", SqlDbType.BigInt) { Value = id }, };

                var PendingPaymentinfo = await _commissionHistory.GetPendingPaymentlist(parameters.ToArray());
                return JsonResponse.GenerateJsonResult(1, "success", PendingPaymentinfo);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GET-GetPendingCommissioninfo");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(GetCommissionDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var commissionHistory = _commissionHistory.GetById(model.InvoiceId);
                    short invoiceStatus ;
                    if (commissionHistory.CommissionAmount == model.PaidAmount)
                        invoiceStatus = (short)PaymentStatus.Completed;
                    else if(commissionHistory.CommissionAmount != model.PaidAmount)
                        invoiceStatus = (short)PaymentStatus.Partial;
                    else
                        invoiceStatus = (short)PaymentStatus.Pending;

                    Invoice invoice = new Invoice
                    {
                        CommissionId = model.InvoiceId,
                        Amount = commissionHistory.CommissionAmount,
                        InvoiceStatus = invoiceStatus
                    };
                    if (commissionHistory.Invoices.Count == 0)
                    {
                        await _invoice.InsertAsync(invoice, Accessor, User.GetUserId());
                    }
                    
                    InvoicePayment invoicePayment = new InvoicePayment
                    {
                        InvoiceId = commissionHistory.Invoices.Where(x => x.CommissionId == commissionHistory.Id).Select(x => x.Id).FirstOrDefault(),
                        PaymentAmount = model.PaidAmount
                    };
                    var previoslypaidAmount = _invoicePayment.GetAll(x => x.InvoiceId == invoicePayment.InvoiceId).Select(x => x.PaymentAmount).Sum();

                    invoicePayment.PaymentStatus = (short)PaymentStatus.Completed;
                    invoicePayment.IsActive = true;
                    invoicePayment.PaidBy = "Manually";
                    var result = await _invoicePayment.InsertAsync(invoicePayment, Accessor, User.GetUserId());

                    if (commissionHistory.CommissionAmount == (previoslypaidAmount + result.PaymentAmount))
                    {
                        // update Invoice Status Invoice Table
                        var Invoice = _invoice.GetSingle(x => x.CommissionId == model.InvoiceId);
                        Invoice.InvoiceStatus = (short)PaymentStatus.Completed;
                        await _invoice.UpdateAsync(Invoice, Accessor, User.GetUserId());

                        //Update Commission Status Commission History Table
                        commissionHistory.CommissionStatus = (short)PaymentStatus.Completed;
                        await _commissionHistory.UpdateAsync(commissionHistory, Accessor, User.GetUserId());
                    }
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.PaymentUpdated);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/CreatePayment");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult DownloadCommissionInvoice(long id)
        {
            try
            {
                var commmissionHistory = _commissionHistory.GetById(id);
                var commissionInvoiceDto = new InvoicecommissionDto
                {
                    InvoiceId = commmissionHistory.Id,
                    InvoiceDate = commmissionHistory.CreatedDate.ToShortDateString(),
                    InvoiceAmount = commmissionHistory.CommissionAmount,
                    PaymentStatus = Enum.GetName(typeof(PaymentStatus), commmissionHistory.CommissionStatus),
                    Distributor = commmissionHistory.Distributor.CompanyName
                };

                return RedirectToAction("DownloadInvoiceAsPDF", commissionInvoiceDto);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "DownloadCommissionInvoice");
                return View("Error", "Home");
            }

        }

        [HttpGet]
        public async Task<IActionResult> SendCommissionInvoiceEmail(long id)
        {
            try
            {

                var commmissionHistory = _commissionHistory.GetById(id);
                InvoicecommissionDto commissionInvoiceDto = new InvoicecommissionDto
                {
                    InvoiceId = commmissionHistory.Id,
                    InvoiceDate = commmissionHistory.CreatedDate.ToShortDateString(),
                    InvoiceAmount = commmissionHistory.CommissionAmount,
                    PaymentStatus = Enum.GetName(typeof(PaymentStatus), commmissionHistory.CommissionStatus),
                    Distributor = commmissionHistory.Distributor.CompanyName
                };
                string fileName = $@"{commissionInvoiceDto.InvoiceId}.pdf";
                var content = new ViewAsPdf("DownloadCommissionInvoice", commissionInvoiceDto) { FileName = fileName };
                var mailAttachment = await content.BuildFile(this.ControllerContext);

                await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                {
                    Subject = "Commission Invoice",
                    ToAddress = commmissionHistory.Distributor.Email,
                    BodyText = "",
                    Attachment = mailAttachment,
                    AttachmentName = fileName
                });

                return JsonResponse.GenerateJsonResult(1, GlobalConstant.EmailSent);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "SendCommissionInvoiceEmail");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }

        }

        #endregion

        #region Common
        public ActionResult DownloadInvoiceAsPDF(InvoicecommissionDto model)
        {
            return new ViewAsPdf("DownloadCommissionInvoice", model) { FileName = model.InvoiceId + ".pdf" };
        }
        #endregion
    }
}