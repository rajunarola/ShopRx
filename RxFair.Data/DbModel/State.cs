using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class State : EntityWithAudit
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [InverseProperty("State")]
        public virtual ICollection<Distributor> Distributors { get; set; }

        [InverseProperty("ContactState")]
        public virtual ICollection<Distributor> ContactState { get; set; }

        public virtual ICollection<PharmacyBillingAddress> PharmacyBillingAddresses { get; set; }
        public virtual ICollection<PharmacyShippingAddress> PharmacyShippingAddresses { get; set; }
    }
}
