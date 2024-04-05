using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.DAL.Enums
{
    public enum Role
    {
        [Display(Name = "Глядач")]
        Viewer = 0,

        [Display(Name = "Художник")]
        Painter = 1,

        [Display(Name = "Адміністратор")]
        Admin = 2,
    }
}
