using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repozytorium.Models
{
    public class Kategoria
    {
        public Kategoria()
        {
            this.Ogloszenie_Kategoria = new HashSet<Ogloszenie_Kategoria>();
        }

        [Key]
        [Display(Name = "ID kategorii:")]
        public int Id { get; set; }

        [Display(Name = "Nazwa kategorii: Co?")]
        [Required]
        public string Nazwa { get; set; }

        [Display(Name = "Id rodzica:")]
        [Required]
        public int ParentId { get; set; }

        #region SEO

        [Display(Name = "Tytuł w Google:")]
        [MaxLength(72)]
        public string MetaTytul { get; set; }

        [Display(Name = "Opis strony w Google:")]
        [MaxLength(160)]
        public string MetaOpis { get; set; }

        [Display(Name = "Słowa kluczowe Google:")]
        [MaxLength(160)]
        public string MetaSlowa { get; set; }

        [Display(Name = "Treść strony:")]
        [MaxLength(500)]
        public string Tresc { get; set; }

        #endregion

        public ICollection<Ogloszenie_Kategoria> Ogloszenie_Kategoria { get; set; }
    }

}