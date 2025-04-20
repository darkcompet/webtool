# ASP.NET Core webapp server

This is asp.net core web app.

## How this project was made

- Make project at local

	```bash
	# Check available templates to choose best suitable template to proceed.
	dotnet new --list

	# Create webapp project
	dotnet new mvc -o WebTool
	cd WebTool

	# Init git
	# This is good time to create files as: .editorconfig, .gitignore,...
	git init

	# Make dotnet tools config
	# Set dotnet version (at this time, we use 7.0.0)
	# Lets change version to the version (at both global.json, and TargetFramework in .csproj)
	dotnet new tool-manifest
	dotnet new globaljson
	dotnet --list-sdks
	nano ./global.json
	nano ./WebTool.csproj
	# Install dotnet-ef tool and set version (at this time, we use 7.0.0)
	dotnet tool install --local dotnet-ef
	nano .config/dotnet-tools.json

	# [Setup] Trust the HTTPS development certificate
	dotnet dev-certs https --trust

	# Cleanup and Add submodules
	cp Properties/launchSettings.json Properties/launchSettings.sample
	mv appsettings.Development.json appsettings.sample
	cp appsettings.sample appsettings.json

	# NOTE: we should add submodule at main branch first, so can add more
	# at another branches (develop,...).
	# This is important behavior since when first pull/checkout at another place
	# will normally checkout main branch first, so submodule is initialized well.
	mkdir -p Tool/Compet; cd Tool/Compet;
	git submodule add https://github.com/darkcompet/cs.git
	git submodule add https://github.com/darkcompet/cs-http.git
	git submodule add https://github.com/darkcompet/cs-net.git
	git submodule add https://github.com/darkcompet/cs-net-json.git
	git submodule add https://github.com/darkcompet/cs-net-http.git
	git submodule add https://github.com/darkcompet/cs-net-efcore.git
	git submodule add https://github.com/darkcompet/cs-asp.git
	cd ../../

	# [Modify setting files]
	nano app/appsettings.json
	nano app/Properties/launchSettings.json

	# [Add JWT authentication packages]
	# Ref: https://www.c-sharpcorner.com/article/asp-net-core-web-api-5-0-authentication-using-jwtjson-base-token/
	dotnet add package Microsoft.AspNetCore.Authentication
	dotnet add package System.IdentityModel.Tokens.Jwt
	dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

	# [Add EntityFrameworkCore packages]
	dotnet add package Microsoft.EntityFrameworkCore
	dotnet add package Microsoft.EntityFrameworkCore.SqlServer
	dotnet add package Microsoft.EntityFrameworkCore.Tools
	dotnet add package MySql.EntityFrameworkCore

	# [File Logging]
	dotnet add package Serilog.AspNetCore

	# Redis for cache and Lock for redis
	dotnet add package StackExchange.Redis
	dotnet add package RedLock.net

	# Cron/Quartz job for asp.net core
	dotnet add package Quartz.AspNetCore
	dotnet add package Quartz.Extensions.Hosting
	dotnet add package Quartz.Extensions.DependencyInjection

	# Parse excel
	dotnet add package EPPlus
	```
