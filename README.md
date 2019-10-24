
# LPRankSyncBot
LPRankSyncBot is a bot for Discord and Sponge Minecraft-Server. It synchronizes the Minecraft LuckPerms ranks with Discord Roles.
The program can only run from your servers directory

## Used packages
* [Discord.net](https://www.nuget.org/packages/Discord.Net/) 2.1.1
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) 12.0.2
* [System.Data.SQlite](https://www.nuget.org/packages/System.Data.SQLite/) 1.0.111


## Prerequisites For development and building

* [.Net Core SDK](https://dotnet.microsoft.com/download) 3.0

## Cloning, building, running and deploying
### Cloning
#### cloning the repository: 

1. `git clone https://github.com/Dodison/LPRankSyncBot.git`
2. `cd LPRankSyncBot`
3. `Dotnet restore` 

#### Updating the clone with the official repo:

1. `git pull`
2. `Dotnet restore`

### Building/deploying and running framework-dependent executables
build DLL: `dotnet Build` (output in bin/Debug/netcoreapp3.0/)

build and run DLL: `dotnet run`(output in bin/Debug/netcoreapp3.0/)

run already built DLL: `dotnet bin/Debug/netcoreapp3.0/LPRankSyncBot.dll`

deploying/publishing DLL: `dotnet publish -c Release`

#### Deploying/publishing (platform-dependent) executables: 

For Windows x86: `dotnet publish -c Release -r win-x86`

For Windows x64: `dotnet publish -c Release -r win-x64`

For Linux: `dotnet publish -c Release -r linux-x64`

For Linux ARM: `dotnet publish -c Release -r linux-arm`

### Building/deploying self-contained (platform-dependent) executables 
For Windows x86: `dotnet publish -c Release -r win-x86 --self-contained true`

For Windows x64: `dotnet publish -c Release -r win-x64 --self-contained true`

For Linux: `dotnet publish -c Release -r linux-x64 --self-contained true`

For Linux ARM: `dotnet publish -c Release -r linux-arm --self-contained true`

### Building/deploying single-file (platform-dependent) executables
For Windows x86: `dotnet publish -c Release -r win-x86 /p:PublishSingleFile=true`

For Windows x64: `dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true`

For Linux: `dotnet publish -c Release -r linux-x64 /p:PublishSingleFile=true`

For Linux ARM: `dotnet publish -c Release -r linux-arm /p:PublishSingleFile=true`
 

## License
This repo is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) - see the [LICENSE.txt](LICENSE.txt) file for details
