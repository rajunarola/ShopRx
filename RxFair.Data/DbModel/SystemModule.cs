using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbContext;

namespace RxFair.Data.DbModel
{
    [Table("SystemModule")]
    public class SystemModule
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ModuleName { get; set; }

        public int? ParentsId { get; set; }
        [ForeignKey("ParentsId")]
        public virtual SystemModule Parent { get; set; }

        public string MenuDisplayText { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public bool IsField { get; set; }

        public int UserGroupId { get; set; }

        public virtual ICollection<SystemModule> Children { get; set; }

        public virtual ICollection<RolesModuleAccess> RolesModuleAccess { get; set; }

        public virtual ICollection<AccessModuleFunctionality> AccessModuleFunctionalities { get; set; }
    }

    [Table("Functionality")]
    public class Functionality
    {
        public Functionality()
        {
            IsActive = true;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<AccessModuleFunctionality> AccessModuleFunctionalities { get; set; }

        public virtual ICollection<RolesModuleAccess> RolesModuleAccesses { get; set; }
    }

    [Table("AccessModuleFunctionality")]
    public class AccessModuleFunctionality
    {
        public AccessModuleFunctionality()
        {
            IsActive = true;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual SystemModule SystemModule { get; set; }

        public int FunctionalityId { get; set; }
        [ForeignKey("FunctionalityId")]
        public virtual Functionality Functionality { get; set; }

        public bool IsActive { get; set; }
    }

    [Table("RolesModuleAccess")]
    public class RolesModuleAccess
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Roles { get; set; }

        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual SystemModule SystemModule { get; set; }

        public int? FunctionalityId { get; set; }
        [ForeignKey("FunctionalityId")]
        public virtual Functionality Functionality { get; set; }
    }

}
