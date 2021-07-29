RELEASE=jetblack-messagebus-3.0.0

.PHONY: all build clean

all:
	@echo "targets: publish clean"

publish: publish-win10-x86 publish-win10-x64 publish-linux-x64 publish-osx-x64

publish-win10-x86:
	dotnet publish core/src/JetBlack.MessageBus.Distributor -r win10-x86 -p:PublishSingleFile=true -o build/distributor-5.0.0-win10-x86
	cd build && zip -r distributor-5.0.0-win10-x86.zip distributor-5.0.0-win10-x86
	rm -r build/distributor-5.0.0-win10-x86

publish-win10-x64:
	dotnet publish core/src/JetBlack.MessageBus.Distributor -r win10-x64 -p:PublishSingleFile=true -o build/distributor-5.0.0-win10-x64
	cd build && zip -r distributor-5.0.0-win10-x64.zip distributor-5.0.0-win10-x64
	rm -r build/distributor-5.0.0-win10-x64

publish-linux-x64:
	dotnet publish core/src/JetBlack.MessageBus.Distributor -r linux-x64 -p:PublishSingleFile=true -o build/distributor-5.0.0-linux-x64
	cd build && tar cvzf distributor-5.0.0-linux-x64.tar.gz distributor-5.0.0-linux-x64
	rm -r build/distributor-5.0.0-linux-x64

publish-osx-x64:
	dotnet publish core/src/JetBlack.MessageBus.Distributor -r osx-x64 -p:PublishSingleFile=true -o build/distributor-5.0.0-osx-x64
	cd build && tar cvzf distributor-5.0.0-osx-x64.tar.gz distributor-5.0.0-osx-x64
	rm -r build/distributor-5.0.0-osx-x64

push-common:
	dotnet pack core/src/JetBlack.MessageBus.Common
	dotnet nuget push core/src/JetBlack.MessageBus.Common/bin/Debug/JetBlack.MessageBus.Common.5.0.0.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-messages:
	dotnet pack core/src/JetBlack.MessageBus.Messages
	dotnet nuget push core/src/JetBlack.MessageBus.Messages/bin/Debug/JetBlack.MessageBus.Messages.5.0.0.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-adapters:
	dotnet pack core/src/JetBlack.MessageBus.Adapters
	dotnet nuget push core/src/JetBlack.MessageBus.Adapters/bin/Debug/JetBlack.MessageBus.Adapters.5.0.0.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-jwtauthentication:
	dotnet pack extensions/authentication/src/JetBlack.MessageBus.Extension.JwtAuthentication
	dotnet nuget push extensions/authentication/src/JetBlack.MessageBus.Extension.JwtAuthentication/bin/Debug/JetBlack.MessageBus.Extension.JwtAuthentication.5.0.0.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-ldapauthentication:
	dotnet pack extensions/authentication/src/JetBlack.MessageBus.Extension.LdapAuthentication
	dotnet nuget push extensions/authentication/src/JetBlack.MessageBus.Extension.LdapAuthentication/bin/Debug/JetBlack.MessageBus.Extension.LdapAuthentication.5.0.0.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-pwdauthentication:
	dotnet pack extensions/authentication/src/JetBlack.MessageBus.Extension.PasswordFileAuthentication
	dotnet nuget push extensions/authentication/src/JetBlack.MessageBus.Extension.PasswordFileAuthentication/bin/Debug/JetBlack.MessageBus.Extension.PasswordFileAuthentication.5.0.0.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json


clean:
	rm -r build
	find . -type d -name bin -exec rm -r {} \;	
	find . -type d -name obj -exec rm -r {} \;	
