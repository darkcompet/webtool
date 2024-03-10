namespace App;

/// This mapping with `appsettings.json` file to avoid
/// hardcoded setting-reference at multiple places.

/// NOTE: If you want to rename field names, must rename section name in `appsettings.json` too.

/// dkopt: For better we should annotate fields to each name if appsettings.json
/// to avoid changes in refactoring process.
public class AppSetting {
	/// Name of sections in `appSetting.json` file.
	public const string SECTION_APP = "App";
	public const string SECTION_QUARTZ = "quartz";

	public bool debug { get; set; }

	/// Current environment
	public string environment { get; set; }
	public const string ENV_DEVELOPMENT = "development";
	public const string ENV_STAGING = "staging";
	public const string ENV_PRODUCTION = "production";

	/// Database connection strings
	public Database database { get; set; }

	/// Authentication
	public JwtSetting jwt { get; set; }
	public AuthCookie authCookie { get; set; }

	public SES ses { get; set; }

	public S3 s3 { get; set; }

	/// Crontab or Command
	public TaskMode taskMode { get; set; }

	public RootUserTemplate rootUserTemplate { get; set; }

	public Web web { get; set; }

	public class JwtSetting {
		public string key { get; set; }
		public string issuer { get; set; }
		public string audience { get; set; }
		public string subject { get; set; }
		/// JWT access token timeout
		public long expiresInSeconds { get; set; }
		/// JWT refresh access token timeout
		public long refreshExpiresInSeconds { get; set; }
	}

	public class AuthCookie {
		public string issuer { get; set; }
		public string audience { get; set; }
	}

	public class Database {
		public string appdb { get; set; }
		public string redis { get; set; }
		public string casinoDb { get; set; }
	}

	/// To create SMTP credentials, see:
	/// https://ap-southeast-1.console.aws.amazon.com/ses/home?region=ap-southeast-1#/account
	/// https://stackoverflow.com/questions/57517582/authentication-required-smtpexception-trying-to-send-mail-from-ec2-instance
	public class SES {
		public string fromEmail { get; set; }
		public string fromName { get; set; }
		public string accessKey { get; set; }
		public string secretKey { get; set; }
		// public string smtpUsername { get; set; }
		// public string smtpPassword { get; set; }
		public string region { get; set; }
	}

	public class S3 {
		public string baseUrl { get; set; }
		public string bucketName { get; set; }
		public string accessKeyId { get; set; }
		public string secretAccessKey { get; set; }
		public string region { get; set; }
	}

	public class RootUserTemplate {
		public string email { get; set; }
		public string password { get; set; }
	}

	public class TaskMode {
		public bool enableCronJob { get; set; }
		public bool enableCommand { get; set; }
	}

	public class Web {
		public string baseUrl { get; set; }
	}
}
