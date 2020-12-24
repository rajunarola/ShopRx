using RxFair.Data.DbModel;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{
    public interface IContactUsService : IGenericService<ContactDetails>
    {

    }

    public interface IEmailSubscriptionService : IGenericService<EmailSubscription>
    {

    }
}
