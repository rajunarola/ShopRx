using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;

namespace RxFair.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly IContactUsService _contactUs;
        private readonly IContactRequestService _contactRequest;
        private readonly EmailService _emailService;
        private readonly IEmailSubscriptionService _emailSubscription;
        private readonly IFaQsService _faqservice;
        private readonly ITestimonialsService _testimonials;
        private readonly ITermsAndConditionService _termsAndCondition;
        private readonly IBlogService _blog;

        public HomeController(IBlogService blog, IContactUsService contactUs, IContactRequestService contactRequest, IEmailSubscriptionService emailSubscription, IOptions<EmailSettingsGmail> emailSettingsGmail,
            IFaQsService faqservice, ITermsAndConditionService termsAndCondition, ITestimonialsService testimonials)
        {
            _contactUs = contactUs;
            _contactRequest = contactRequest;
            _emailSubscription = emailSubscription;
            _emailService = new EmailService(emailSettingsGmail);
            _faqservice = faqservice;
            _termsAndCondition = termsAndCondition;
            _testimonials = testimonials;
            _blog = blog;

        }

        public IActionResult Index()
        {
            var emailUnSubscribe = Accessor.HttpContext.Session.GetObjectFromJson<EmailUnSubscribe>("emailUnSubscribe");

            var model = new RxFairHomePageModel()
            {
                TestimonialDto = Mapper.Map<List<TestimonialDto>>(_testimonials.GetAll(x => x.IsActive)),
                EmailUnSubscribe = emailUnSubscribe
            };
            model.TestimonialDto.ForEach(x => { x.Image = $@"{FilePathList.Testimonial}\{x.Image}"; });
            return View(model);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            var model = new ContactDetailsDto
            {
                ContactDetails = Mapper.Map<ContactDetailView>(_contactUs.GetAll(x => x.IsActive).FirstOrDefault())
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactRequest(ContactDetailsDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var contactRequest = Mapper.Map<ContactRequestView, ContactRequest>(model.ContactRequest);
                        await _contactRequest.InsertAsync(contactRequest, Accessor);
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Your request submission is successfully done, we will respond to you soon.");
                    }
                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ContactRequest");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public IActionResult Faqs()
        {
            return View();
        }

        public async Task<IActionResult> SearchFaq(string qution)
        {
            var param = new List<SqlParameter>()
            {
                new SqlParameter("@Search", SqlDbType.VarChar) {Value = qution.NullToString()},
            };
            var model = await _faqservice.GetFaQsListAsyncBySearch(param.ToArray());
            return View(@"Components/_SearchFaq", model);
        }

        public IActionResult TermsCondition()
        {
            var list = _termsAndCondition.GetSingle(x => x.IsActive);

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> EmailSubscription(string email)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (string.IsNullOrEmpty(email))
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, "Please enter email address !");
                    }
                    if (_emailSubscription.GetSingle(x => x.Email.Equals(email)) != null)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, "Your email already subscribed.");
                    }
                    var newEmailSubscription = new EmailSubscription()
                    {
                        Email = email,
                        IsActive = true
                    };
                    await _emailSubscription.InsertAsync(newEmailSubscription, Accessor);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Your email subscribed successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/EmailSubscription");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult EmailUnSubscribe(string email, string returnUrl = null)
        {
            Accessor.HttpContext.Session.SetObjectAsJson("emailUnSubscribe", new EmailUnSubscribe { Email = email, ReturnUrl = returnUrl });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UnSubscribeEmail(string email, bool flag)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _emailSubscription.GetSingle(x => x.Email.Equals(email));
                    if (result == null) return JsonResponse.GenerateJsonResult(0);
                    result.IsActive = !flag;
                    _emailSubscription.Update(result);
                    Accessor.HttpContext.Session.SetObjectAsJson("emailUnSubscribe", new EmailUnSubscribe());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Your email unsubscribed successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/UnSubscribeEmail");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //ViewBag.HideLogin = true;
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorCode = Accessor.HttpContext.Response.StatusCode
            });
        }

        public IActionResult SuccessView()
        {
            return View();
        }

        public IActionResult FailureView()
        {
            return View();
        }

        public async Task<IActionResult> TestMail()
        {
            await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
            {
                BodyText = @"<h1>Test Mail</h1>",
                Subject = "Test Mail",
                ToAddress = "anc@narola.email",
                ToDisplayName = "Anc Narola"
            });
            return RedirectToAction("Index", "Home");
        }

    }
}
