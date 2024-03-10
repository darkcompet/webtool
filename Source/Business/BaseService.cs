namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public partial class BaseService {
	protected readonly AppDbContext dbContext;
	protected readonly AppSetting appSetting;

	public BaseService(AppDbContext dbContext, IOptionsSnapshot<AppSetting> snapshot) {
		this.dbContext = dbContext;
		this.appSetting = snapshot.Value;
	}
}
