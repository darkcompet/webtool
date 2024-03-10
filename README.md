# ASP.NET Core Razor webapp

This is asp.net core Razor webapp.


## Quick Start

- Setup and Run


	```bash
	# Clone the project
	cd api-server
	git checkout develop

	# Generate files for current environment.
	# Lets modify setting in result files to match with current env.
	cp Properties/launchSettings.sample Properties/launchSettings.json

	# Modify app settings to match with current env.
	cp appsettings.sample appsettings.json
	nano appsettings.json

	# Init submodules for this project
	./git-pull-current.sh

	# [Optional] For Apple silicon chip arm64, we need create link of dotnet x64
	sudo ln -s /usr/local/share/dotnet/x64/dotnet /usr/local/bin/

	# Start server manually.
	# For IIS server, just restart the site.
	dotnet run

	# View api doc at
	http://localhost:8100/swagger
	```


## Tips

- For command mode

	```bash
	# Open a terminal, run:
	dotnet publish --configuration Release
	dotnet bin/Release/net7.0/publish/Adabet.dll --urls http://0.0.0.0:5000

	# At another terminal, run:
	~/.dotnet/tools/httprepl http://localhost:5000
	ls
	cd api/xxx
	post
	exit
	```

- Remove a submodule

	```bash
	# For eg,. we remove some module under tool/compet folder
	cd tool/compet
	git submodule deinit -f [the_submodule]
	git rm -f [the_submodule]
	rm -rf .git/modules/tool/compet/[the_submodule]
	```
