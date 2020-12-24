using AutoMapper;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Utility.Extension;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RxFair.AutoMapperProfileConfiguration
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<Blog, BlogDto>()
                .ForMember(des => des.BlogTag, opt => opt.MapFrom(src => src.BlogTags.Select(x => x.TagName).ToList()))
                .ReverseMap();
            CreateMap<BlogImage, BlogImageDto>().ReverseMap();
            CreateMap<AdvertiseEmailTemplate, AdvertiseTemplateDto>().ReverseMap();
            CreateMap<ContactDetails, ContactDetailView>().ReverseMap();
            CreateMap<ContactRequest, ContactRequestView>().ReverseMap();
            CreateMap<Testimonials, TestimonialDto>().ReverseMap();
            CreateMap<RewardProduct, RewardProductDto>().ReverseMap();
            CreateMap<Distributor, DistributorDto>().ReverseMap();
            CreateMap<DistributorSubscription, DistributorSubscriptionDto>()
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(des => des.SubscriptionTypeId, opt => opt.MapFrom(src => src.SubscriptionTypeId))
                .ForMember(des => des.DistributorId, opt => opt.MapFrom(src => src.DistributorId))
                .ForMember(des => des.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ReverseMap();
            CreateMap<DistributorOrderSetting, DistributerOrderSettingDto>()
                .ForMember(des => des.MondayCutOffTime, opt => opt.MapFrom(src => ChangeTimeSpanToString(src.MondayCutOffTime)))
                .ForMember(des => des.TuesdayCutOffTime, opt => opt.MapFrom(src => ChangeTimeSpanToString(src.TuesdayCutOffTime)))
                .ForMember(des => des.WednesdayCutOffTime, opt => opt.MapFrom(src => ChangeTimeSpanToString(src.WednesdayCutOffTime)))
                .ForMember(des => des.ThursdayCutOffTime, opt => opt.MapFrom(src => ChangeTimeSpanToString(src.ThursdayCutOffTime)))
                .ForMember(des => des.FridayCutOffTime, opt => opt.MapFrom(src => ChangeTimeSpanToString(src.FridayCutOffTime)))
                .ForMember(des => des.SaturdayCutOffTime, opt => opt.MapFrom(src => ChangeTimeSpanToString(src.SaturdayCutOffTime)))
                .ForMember(des => des.SundayCutOffTime, opt => opt.MapFrom(src => ChangeTimeSpanToString(src.SundayCutOffTime)))
                .ReverseMap();
            CreateMap<DocumentMaster, DocumentView>()
                .ReverseMap();
            CreateMap<Distributor, DistributorProfileDto>()
                .ForMember(des => des.StateName, opt => opt.MapFrom(src => src.State.Name ?? ""))
                .ForMember(des => des.ContactStateName, opt => opt.MapFrom(src => src.ContactState.Name ?? ""))
                .ReverseMap()
                .ForPath(src => src.State.Name, opt => opt.Ignore())
                .ForPath(src => src.ContactState.Name, opt => opt.Ignore());
            CreateMap<NewDistributorRequest, NewDistributorRequestDto>()
                .ForMember(des => des.StateName, opt => opt.MapFrom(src => src.State.Name ?? ""))
                .ReverseMap();
            CreateMap<RewardMoneyMaster, RewardMoneyDto>().ReverseMap();
            CreateMap<RewardEarn, RewardEarnDto>().ReverseMap();
            CreateMap<Pharmacy, NewPharmacyDto>()
                .ForMember(des => des.FileLicense, opt => opt.MapFrom(src => src.LicenseFile))
                .ForMember(des => des.FileDea, opt => opt.MapFrom(src => src.DeaFile))
                .Ignore(des => des.LicenseFile)
                .Ignore(des => des.DeaFile)
                .ReverseMap();
            CreateMap<PharmacyBillingAddress, PharmacyBillOrShipAddressDto>()
                .ForMember(des => des.StateName, opt => opt.MapFrom(src => src.State.Name ?? ""))
                .ReverseMap()
                .ForPath(src => src.State.Name, opt => opt.Ignore());
            CreateMap<PharmacyShippingAddress, PharmacyBillOrShipAddressDto>()
                .ForMember(des => des.StateName, opt => opt.MapFrom(src => src.State.Name ?? ""))
                .ReverseMap()
                .ForPath(src => src.State.Name, opt => opt.Ignore());
            CreateMap<SystemModule, SystemModuleDto>().ReverseMap();
            CreateMap<Functionality, FunctionalityDto>().ReverseMap();
            CreateMap<AccessModuleFunctionality, AccessModuleFunctionalityDto>().ReverseMap();
            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<DistributorMedicine, DistributorMedicineDto>().ReverseMap();
            CreateMap<UploadedMedicine, AddEditMedicine>()
                .ForMember(up => up.IsNdc, opt => opt.MapFrom(src => src.Ndc != src.Upc))
                .ReverseMap();
            CreateMap<MedicinePriceMaster, MedicinePriceMaster>()
           .ReverseMap()
           .ForPath(src => src.Id, opt => opt.Ignore())
           .ForPath(src => src.InStock, opt => opt.Ignore())
           .ForPath(src => src.IsShortDated, opt => opt.Ignore())
           .ForPath(src => src.IsContracted, opt => opt.Ignore());
        }

        private static string ChangeTimeSpanToString(TimeSpan? time)
        {
            return time.ToDefaultTime();
        }

    }

    public static class MappingExpression
    {
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(this IMappingExpression<TSource, TDestination> map, Expression<Func<TDestination, object>> selector)
        {
            map.ForMember(selector, config => config.Ignore());
            return map;
        }
    }


}
