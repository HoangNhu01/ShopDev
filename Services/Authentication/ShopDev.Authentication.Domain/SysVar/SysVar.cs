using ShopDev.Constants.Database;
using Microsoft.EntityFrameworkCore;

namespace ShopDev.Authentication.Domain.SysVar
{
    [Table(nameof(SysVar), Schema = DbSchemas.Default)]
    [Index(nameof(GrName), nameof(VarName), Name = $"IX_{nameof(SysVar)}")]
    public class SysVar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public required string GrName { get; set; }

        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public required string VarName { get; set; }

        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public required string VarValue { get; set; }

        [MaxLength(128)]
        [Unicode(true)]
        public string? VarDesc { get; set; }
    }
}
