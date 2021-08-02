DISTRIBUTOR_VERSION=5.1.0
COMMON_VERSION=5.0.0
MESSAGES_VERSION=5.0.0
ADAPTERS_VERSION=5.0.0
JWT_AUTHENTICATION_VERSION=5.0.0
LDAP_AUTHENTICATION_VERSION=5.0.0
PWD_AUTHENTICATION_VERSION=5.1.0

CORE_SRC=core/src
EXTENSIONS_AUTH_SRC=extensions/authentication/src

.PHONY: all build clean

all:
	@echo "targets: publish clean"

publish: publish-win10-x86 publish-win10-x64 publish-linux-x64 publish-osx-x64

publish-win10-x86:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r win10-x86 -p:PublishSingleFile=true -o build/distributor-${DISTRIBUTOR_VERSION}-win10-x86
	cd build && zip -r distributor-${DISTRIBUTOR_VERSION}-win10-x86.zip distributor-${DISTRIBUTOR_VERSION}-win10-x86
	rm -r build/distributor-${DISTRIBUTOR_VERSION}-win10-x86

publish-win10-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r win10-x64 -p:PublishSingleFile=true -o build/distributor-${DISTRIBUTOR_VERSION}-win10-x64
	cd build && zip -r distributor-${DISTRIBUTOR_VERSION}-win10-x64.zip distributor-${DISTRIBUTOR_VERSION}-win10-x64
	rm -r build/distributor-${DISTRIBUTOR_VERSION}-win10-x64

publish-linux-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r linux-x64 -p:PublishSingleFile=true -o build/distributor-${DISTRIBUTOR_VERSION}-linux-x64
	cd build && tar cvzf distributor-${DISTRIBUTOR_VERSION}-linux-x64.tar.gz distributor-${DISTRIBUTOR_VERSION}-linux-x64
	rm -r build/distributor-${DISTRIBUTOR_VERSION}-linux-x64

publish-osx-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r osx-x64 -p:PublishSingleFile=true -o build/distributor-${DISTRIBUTOR_VERSION}-osx-x64
	cd build && tar cvzf distributor-${DISTRIBUTOR_VERSION}-osx-x64.tar.gz distributor-${DISTRIBUTOR_VERSION}-osx-x64
	rm -r build/distributor-${DISTRIBUTOR_VERSION}-osx-x64

push-common:
	dotnet pack ${CORE_SRC}/JetBlack.MessageBus.Common
	dotnet nuget push ${CORE_SRC}/JetBlack.MessageBus.Common/bin/Debug/JetBlack.MessageBus.Common.${COMMON_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-messages:
	dotnet pack ${CORE_SRC}/JetBlack.MessageBus.Messages
	dotnet nuget push ${CORE_SRC}/JetBlack.MessageBus.Messages/bin/Debug/JetBlack.MessageBus.Messages.${MESSAGES_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-adapters:
	dotnet pack ${CORE_SRC}/JetBlack.MessageBus.Adapters
	dotnet nuget push ${CORE_SRC}/JetBlack.MessageBus.Adapters/bin/Debug/JetBlack.MessageBus.Adapters.${ADAPTERS_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-jwtauthentication:
	dotnet pack ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.JwtAuthentication
	dotnet nuget push ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.JwtAuthentication/bin/Debug/JetBlack.MessageBus.Extension.JwtAuthentication.${JWT_AUTHENTICATION_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-ldapauthentication:
	dotnet pack ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.LdapAuthentication
	dotnet nuget push ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.LdapAuthentication/bin/Debug/JetBlack.MessageBus.Extension.LdapAuthentication.${LDAP_AUTHENTICATION_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-pwdauthentication:
	dotnet pack ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.PasswordFileAuthentication
	dotnet nuget push ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.PasswordFileAuthentication/bin/Debug/JetBlack.MessageBus.Extension.PasswordFileAuthentication.${PWD_AUTHENTICATION_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json


clean:
	rm -r build
	find . -type d -name bin -exec rm -r {} \;	
	find . -type d -name obj -exec rm -r {} \;	
