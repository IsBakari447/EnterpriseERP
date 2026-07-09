using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseERP.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nom complet")]
        [StringLength(100)]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        // ==========================
        // RÔLES
        // ==========================
        // SuperAdmin
        // Admin
        // Manager
        // Employee
        // ==========================

        [Required]
        public string Role { get; set; } = "Employee";

        // Seul le SuperAdmin peut créer d'autres Admins
        public bool IsSuperAdmin { get; set; } = false;

        // Compte actif / désactivé
        public bool IsActive { get; set; } = true;

        // Validation par le SuperAdmin
        public bool IsApproved { get; set; } = true;

        // Informations de suivi
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLogin { get; set; }

        // Créateur du compte
        public int? CreatedById { get; set; }

        public User? CreatedBy { get; set; }

        // Dernière modification
        public DateTime? UpdatedAt { get; set; }

        // Photo de profil
        public string? PhotoPath { get; set; }

        // Téléphone
        [Phone]
        public string? Phone { get; set; }

        // Adresse
        public string? Address { get; set; }

        // Département
        public string? Department { get; set; }

        // Poste
        public string? Position { get; set; }

        // Langue préférée
        public string PreferredLanguage { get; set; } = "fr";

        // Thème
        public string Theme { get; set; } = "Dark";

        // Dernière adresse IP
        public string? LastIPAddress { get; set; }

        // Dernière connexion
        public DateTime? LastConnection { get; set; }

        // Nombre de connexions
        public int LoginCount { get; set; } = 0;
    }
}