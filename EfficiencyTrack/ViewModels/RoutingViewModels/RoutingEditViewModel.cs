using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.Routing
{
    public class RoutingEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Code cannot be longer than 20 characters.")]
        [Display(Name = "Код")]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Зона")]
        public string Zone { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 180, ErrorMessage = "Minutes per piece must be between 0.01 and 180 minutes.")]
        [Display(Name = "Време за една бройка в минутри")]
        public decimal MinutesPerPiece { get; set; }

        [Required]
        [Display(Name = "Отдел")]
        public Guid DepartmentId { get; set; }

        [Display(Name = "Отдели")]
        public List<SelectListItem> AvailableDepartments { get; set; } = new();

        [Display(Name = "Зони")]
        public List<SelectListItem> AvailableZones { get; set; } = new();
    }
}
