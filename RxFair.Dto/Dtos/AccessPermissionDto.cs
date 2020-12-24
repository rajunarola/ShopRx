using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class SubmenuRoleViewModel
    {
        public int MainMenuId { get; set; }
        public List<RolesViewModel> RoleList { get; set; }
        public List<SubMenuViewModel> SubMenuList { get; set; }
    }

    public class RolesViewModel
    {
        public long Id { get; set; }
        public string RoleName { get; set; }
        public bool IsAccess { get; set; }
        public List<FunctionalityAccessViewModel> Functionality { get; set; }
    }

    public class SubMenuViewModel
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public bool IsField { get; set; }
        public bool IsAccess { get; set; }
        public List<long> RoleIds { get; set; }
        public List<RoleModuleAccessDto> RoleFunctionalityAccess { get; set; }
        public List<int> RolesModuleId { get; set; }
        public List<int?> FunctionalityId { get; set; }
        public List<RolesViewModel> RolesList { get; set; }
        public List<int> AccessFunctionalityRolesId { get; set; }
        public List<FunctionalityAccessViewModel> Functionality { get; set; }
    }

    public class RoleModuleAccessDto
    {
        public long RoleId { get; set; }
        public FunctionalityAccessViewModel Functionality { get; set; }
    }

    public class FunctionalityAccessViewModel
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public int RolesModuleId { get; set; }
        public long RoleId { get; set; }
        public int FunctionalityId { get; set; }
        public bool IsAccess { get; set; }
    }

    public class MenuViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public List<long> RoleIds { get; set; }
        public List<RolesViewModel> RolesList { get; set; }
        public List<RoleModuleAccessDto> RoleFunctionalityAccess { get; set; }
        public List<FunctionalityAccessViewModel> Functionality { get; set; }
        public bool IsField { get; set; }
        public List<SubMenuViewModel> Childs { get; set; }
    }

    public class AllMenu
    {
        public List<RolesViewModel> RoleList { get; set; }
        public List<MenuViewModel> MenuList { get; set; }
    }

    public class AllowedMenuViewModel
    {
        public string MenuDisplayText { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsField { get; set; }
    }

    public class SystemModuleDto
    {
        public int Id { get; set; }

        public string ModuleName { get; set; }

        public int? ParentsId { get; set; }

        public string ParentMenu { get; set; }

        public string MenuDisplayText { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public bool IsField { get; set; }

        public bool IsAccess { get; set; }

        public int UserGroupId { get; set; }

        public string UserGroup { get; set; }

        public int TotalRecords { get; set; }
    }

    public class FunctionalityDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }

    public class RolesModuleAccessDto
    {
        public int Id { get; set; }

        public long RolesId { get; set; }

        public int SystemModuleId { get; set; }

        public int ModuleId { get; set; }
        public int FunctionalityId { get; set; }

        public string SystemModule { get; set; }
    }

    public class AccessModuleFunctionalityDto
    {
        public int Id { get; set; }

        public int ModuleId { get; set; }

        public int FunctionalityId { get; set; }

        public string FunctionalityName { get; set; }

        public bool IsActive { get; set; }
    }

    public class RolesModuleAccessPermissionDto
    {
        public int Id { get; set; }
        public int RolesModuleId { get; set; }
        public int ModuleId { get; set; }
        public int FunctionalityId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAccess { get; set; }
    }
}
