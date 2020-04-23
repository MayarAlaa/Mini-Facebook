using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name Required")]
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string Description { get; set; }
    }
}
