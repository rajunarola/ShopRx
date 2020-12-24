using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{
    public class ContactUsRepository : GenericRepository<ContactDetails>, IContactUsService
    {
        private readonly RxFairDbContext _context;
        public ContactUsRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }

    public class EmailSubscriptionRepository : GenericRepository<EmailSubscription>, IEmailSubscriptionService
    {
        private readonly RxFairDbContext _context;
        public EmailSubscriptionRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
