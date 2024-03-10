namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table(DbConst.table_bingo_client)]
[Index(nameof(client_id), IsUnique = true)]
[Index(nameof(bingo_code))]
public class BingoClientModel : AutoGenerateUpdateTime {
	/// PK
	[Key]
	[Column("id")]
	public long id { get; set; }

	/// This is unique client id that be stored in client cookie.
	/// When a client come, we generate/assign new GUID to that client.
	[Required]
	[Column("client_id", TypeName = "varchar(255)"), MaxLength(255)]
	public string client_id { get; set; }

	/// Name of user for display.
	[Column("name", TypeName = "nvarchar(255)"), MaxLength(255)]
	public string? name { get; set; }

	/// Bingo number which was assigned to when create new user.
	[Required]
	[Column("bingo_code", TypeName = "varchar(255)"), MaxLength(255)]
	public string bingo_code { get; set; }

	/// When the bingo_code of this user was hit (at that time, user became to winner).
	[Column("bingo_hit_at")]
	public DateTime? bingo_hit_at { get; set; }

	/// Owner who issues the bingo winner for this user.
	[Column("winner_issued_by_user")]
	public long winner_issued_by_user { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}

public class BingoClientModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<BingoClientModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}

public class BingoClientModelConst {
}
