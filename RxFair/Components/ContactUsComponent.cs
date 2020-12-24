using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface;

namespace RxFair.Components
{
    public class ContactUsViewComponent : ViewComponent
    {
        private readonly IContactUsService _contactUs;
        private readonly IMapper _mapper;

        public ContactUsViewComponent(IContactUsService contactUs, IMapper mapper)
        {
            _contactUs = contactUs;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _mapper.Map<ContactDetailView>(_contactUs.GetAll(x => x.IsActive).FirstOrDefault());

            return await Task.FromResult((IViewComponentResult)View("~/Views/Home/Components/_ContactUs.cshtml", model));
        }
    }
}
