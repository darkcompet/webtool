namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table(DbConst.table_bingo)]
[Index(nameof(code), IsUnique = true)]
public class BingoModel {
	/// PK
	[Key]
	[Column("id")]
	public long id { get; set; }

	/// Bingo number
	[Required]
	[Column("code", TypeName = "varchar(255)"), MaxLength(255)]
	public string code { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("consumed_at")]
	public DateTime? consumed_at { get; set; }
}

public class BingoModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<BingoModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}

public class BingoModelConst {
}
