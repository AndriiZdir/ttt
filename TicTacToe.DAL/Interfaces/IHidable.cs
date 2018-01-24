using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.DAL.Enums;

namespace TicTacToe.DAL.Interfaces
{
    public interface IHidable : IEntity
    {
        VisibilityStatus VisibilityStatus { get; set; }
    }

    public enum VisibilityStatus
    {
        [Display(Name = "Visible")]
        Visible,
        [Display(Name = "Hidden")]
        Hidden,
        [Display(Name = "NotPublished")]
        NotPublished
    }
}
