namespace RxFair.Dto.Dtos
{
    public class UserBasicInfo : BaseModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string UserGroup { get; set; }

        public bool IsRoleChange { get; set; }
    }
}
