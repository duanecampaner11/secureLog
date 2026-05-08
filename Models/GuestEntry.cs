using System.ComponentModel.DataAnnotations;

namespace SecureLog.Models;

public class GuestEntry
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Visitor Name")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Purpose of Visit")]
    public string Purpose { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Time In")]
    public DateTime TimeIn { get; set; }
    
    [Display(Name = "Time Out")]
    public DateTime? TimeOut { get; set; }
    
    public string LoggedByUserId { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; }
}