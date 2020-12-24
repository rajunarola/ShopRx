using System.ComponentModel.DataAnnotations;

namespace RxFair.Dto.Dtos
{
    public class ContactDetailsDto
    {
        public ContactDetailView ContactDetails { get; set; }
        public ContactRequestView ContactRequest { get; set; }
    }

    public class ContactDetailView : BaseModel
    {
        [Required(ErrorMessage = "Please enter email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter address.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter city.")]
        public string City { get; set; }

        public long StateId { get; set; }

        public string State { get; set; }

        [Required(ErrorMessage = "Please enter ZipCode.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Please enter Telephone no.")]
        public string Telephone { get; set; }

        public string Fax { get; set; }

    }

    public class ContactRequestView : BaseModel
    {
        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        public string Message { get; set; }
    }
}