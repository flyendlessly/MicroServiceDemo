using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels
{
    public class UserViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "用户账户名不能为空")]
        [MinLength(2)]
        [MaxLength(100)]
        [DisplayName("账户名")]
        public String Account { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(2)]
        [MaxLength(16)]
        [DisplayName("密码")]
        public String PassWord { get; set; }

        [DisplayName("角色")]
        public String Role { get; set; }
    }
}
