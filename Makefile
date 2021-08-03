DISTRIBUTOR_VERSION=5.1.1
COMMON_VERSION=5.0.0
MESSAGES_VERSION=5.0.0
ADAPTERS_VERSION=5.0.0
JWT_AUTHENTICATION_VERSION=5.2.0
LDAP_AUTHENTICATION_VERSION=5.2.0
PWD_AUTHENTICATION_VERSION=5.2.0

CORE_SRC=core/src
EXTENSIONS_AUTH_SRC=extensions/authentication/src
UTILS_SRC=utilities/src
DIST_WIN10_X86=distributor-${DISTRIBUTOR_VERSION}-win10-x86
DIST_WIN10_X64=distributor-${DISTRIBUTOR_VERSION}-win10-x64
DIST_LINUX_X64=distributor-${DISTRIBUTOR_VERSION}-linux-x64
DIST_OSX_X64=distributor-${DISTRIBUTOR_VERSION}-osx-x64

.PHONY: all dist dotnet-build clean

all:
	@echo "targets: dist clean"

dist: publish copy-extensions

dotnet-build:
	dotnet build

.PHONY: publish publish-win10-x86 publish-win10-x64 publish-linux-x64 publish-osx-x64

publish: publish-win10-x86 publish-win10-x64 publish-linux-x64 publish-osx-x64

publish-win10-x86:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r win10-x86 -p:PublishSingleFile=true -o build/${DIST_WIN10_X86}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_WIN10_X86}
	cp scripts/distributor.bat build/${DIST_WIN10_X86}
	cd build && zip -r ${DIST_WIN10_X86}.zip ${DIST_WIN10_X86}
	rm -r build/${DIST_WIN10_X86}

publish-win10-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r win10-x64 -p:PublishSingleFile=true -o build/${DIST_WIN10_X64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_WIN10_X64}
	cp scripts/distributor.bat build/${DIST_WIN10_X64}
	cd build && zip -r ${DIST_WIN10_X64}.zip ${DIST_WIN10_X64}
	rm -r build/${DIST_WIN10_X64}

publish-linux-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r linux-x64 -p:PublishSingleFile=true -o build/${DIST_LINUX_X64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_LINUX_X64}
	cp scripts/distributor.sh build/${DIST_LINUX_X64}
	cd build && tar cvzf ${DIST_LINUX_X64}.tar.gz ${DIST_LINUX_X64}
	rm -r build/${DIST_LINUX_X64}

publish-osx-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r osx-x64 -p:PublishSingleFile=true -o build/${DIST_OSX_X64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_OSX_X64}
	cp scripts/distributor.sh build/${DIST_OSX_X64}
	cd build && tar cvzf ${DIST_OSX_X64}.tar.gz ${DIST_OSX_X64}
	rm -r build/${DIST_OSX_X64}


.PHONY: copy-extensions copy-extension-jwtauthentication copy-extension-ldapauthentication copy-extension-pwdauthentication

copy-extensions: dotnet-build copy-extension-jwtauthentication copy-extension-ldapauthentication copy-extension-pwdauthentication

copy-extension-jwtauthentication: build
	cp -r ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.JwtAuthentication/bin/Debug/netstandard2.1 build/JetBlack.MessageBus.Extension.JwtAuthentication-${JWT_AUTHENTICATION_VERSION}
	cd build && zip -r JetBlack.MessageBus.Extension.JwtAuthentication-${JWT_AUTHENTICATION_VERSION}.zip JetBlack.MessageBus.Extension.JwtAuthentication-${JWT_AUTHENTICATION_VERSION}
	cd build && tar czvf JetBlack.MessageBus.Extension.JwtAuthentication-${JWT_AUTHENTICATION_VERSION}.tar.gz JetBlack.MessageBus.Extension.JwtAuthentication-${JWT_AUTHENTICATION_VERSION}
	rm -r build/JetBlack.MessageBus.Extension.JwtAuthentication-${JWT_AUTHENTICATION_VERSION}

copy-extension-ldapauthentication: build
	cp -r ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.LdapAuthentication/bin/Debug/netstandard2.1 build/JetBlack.MessageBus.Extension.LdapAuthentication-${LDAP_AUTHENTICATION_VERSION}
	cd build && zip -r JetBlack.MessageBus.Extension.LdapAuthentication-${LDAP_AUTHENTICATION_VERSION}.zip JetBlack.MessageBus.Extension.LdapAuthentication-${LDAP_AUTHENTICATION_VERSION}
	cd build && tar czvf JetBlack.MessageBus.Extension.LdapAuthentication-${LDAP_AUTHENTICATION_VERSION}.tar.gz JetBlack.MessageBus.Extension.LdapAuthentication-${LDAP_AUTHENTICATION_VERSION}
	rm -r build/JetBlack.MessageBus.Extension.LdapAuthentication-${LDAP_AUTHENTICATION_VERSION}

copy-extension-pwdauthentication: build
	cp -r ${EXTENSIONS_AUTH_SRC}/JetBlack.MessageBus.Extension.PasswordFileAuthentication/bin/Debug/netstandard2.1 build/JetBlack.MessageBus.Extension.PasswordFileAuthentication-${PWD_AUTHENTICATION_VERSION}
	cd build && zip -r JetBlack.MessageBus.Extension.PasswordFileAuthentication-${PWD_AUTHENTICATION_VERSION}.zip JetBlack.MessageBus.Extension.PasswordFileAuthentication-${PWD_AUTHENTICATION_VERSION}
	cd build && tar czvf JetBlack.MessageBus.Extension.PasswordFileAuthentication-${PWD_AUTHENTICATION_VERSION}.tar.gz JetBlack.MessageBus.Extension.PasswordFileAuthentication-${PWD_AUTHENTICATION_VERSION}
	rm -r build/JetBlack.MessageBus.Extension.PasswordFileAuthentication-${PWD_AUTHENTICATION_VERSION}

build:
	mkdir build

push-common:
	dotnet pack ${CORE_SRC}/JetBlack.MessageBus.Common
	dotnet nuget push ${CORE_SRC}/JetBlack.MessageBus.Common/bin/Debug/JetBlack.MessageBus.Common.${COMMON_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-messages:
	dotnet pack ${CORE_SRC}/JetBlack.MessageBus.Messages
	dotnet nuget push ${CORE_SRC}/JetBlack.MessageBus.Messages/bin/Debug/JetBlack.MessageBus.Messages.${MESSAGES_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

push-adapters:
	dotnet pack ${CORE_SRC}/JetBlack.MessageBus.Adapters
	dotnet nuget push ${CORE_SRC}/JetBlack.MessageBus.Adapters/bin/Debug/JetBlack.MessageBus.Adapters.${ADAPTERS_VERSION}.nupkg --api-key ${NUGET_API_KEY} --source https://api.nuget.org/v3/index.json

clean:
	rm -r build
	find . -type d -name bin -exec rm -rf {} \;	
	find . -type d -name obj -exec rm -rf {} \;	
