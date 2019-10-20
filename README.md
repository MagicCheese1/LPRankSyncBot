
# LPRankSyncBot
LPRankSyncBot is a bot for Discord and Minecraft-Server. It synchronizes the Minecraft LuckyPerms ranks with Discord Roles.

## used packages
* [Discord.net](https://www.nuget.org/packages/Discord.Net/) 2.1.1
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) 12.0.2


## Prerequisites For development and building

* [.Net Core SDK](https://dotnet.microsoft.com/download) 3.0

## Cloning, building, running and deploying
### Cloning
#### cloning the repository: 

`git clone https://github.com/Dodison/LPRankSyncBot.git`

#### updating the clone with the official repo:

`git pull`

### building/deploying and running framework-dependent executables
build DLL: `dotnet Build` (output in bin/Debug/netcoreapp3.0/)

build and run DLL: `dotnet run`(output in bin/Debug/netcoreapp3.0/)

run already built DLL: `dotnet bin/Debug/netcoreapp3.0/LPRankSyncBot.dll`

deploying/publishing DLL: `dotnet publish -c Release`

#### deploying/publishing (platform-dependent) executables: 

For Windows x86: `dotnet publish -c Release -r win-x86 --self-contained false`

For Windows x64: `dotnet publish -c Release -r win-x64 --self-contained false`

For Linux: `dotnet publish -c Release -r linux-x64 --self-contained false`

For Linux ARM: `dotnet publish -c Release -r linux-arm --self-contained false`

### building/deploying self-contained (platform-dependent) executables 
For Windows x86: `dotnet publish -c Release -r win-x86 --self-contained true`

For Windows x64: `dotnet publish -c Release -r win-x64 --self-contained true`

For Linux: `dotnet publish -c Release -r linux-x64 --self-contained true`

For Linux ARM: `dotnet publish -c Release -r linux-arm --self-contained true`

## License
This repo is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) - see the [LICENSE.txt](LICENSE.txt) file for details
