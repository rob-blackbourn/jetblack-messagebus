DISTRIBUTOR_VERSION=6.0.0-alpha.1
COMMON_VERSION=6.0.0-alpha.1
MESSAGES_VERSION=6.0.0-alpha.1
ADAPTERS_VERSION=6.0.0-alpha.1
JWT_AUTHENTICATION_VERSION=6.0.0-alpha.1
LDAP_AUTHENTICATION_VERSION=6.0.0-alpha.1
PWD_AUTHENTICATION_VERSION=6.0.0-alpha.1
MKPASSWD_VERSION=6.0.0-alpha.1

CORE_SRC=core/src
EXTENSIONS_AUTH_SRC=extensions/authentication/src
UTILS_SRC=utilities/src

DIST_WIN10_X86=distributor-${DISTRIBUTOR_VERSION}-win10-x86
DIST_WIN10_X64=distributor-${DISTRIBUTOR_VERSION}-win10-x64
DIST_WIN10_ARM64=distributor-${DISTRIBUTOR_VERSION}-win10-arm64
DIST_LINUX_X64=distributor-${DISTRIBUTOR_VERSION}-linux-x64
DIST_LINUX_ARM64=distributor-${DISTRIBUTOR_VERSION}-linux-arm64
DIST_OSX_X64=distributor-${DISTRIBUTOR_VERSION}-osx-x64
DIST_OSX_ARM64=distributor-${DISTRIBUTOR_VERSION}-osx-arm64

MKPASSWD_WIN10_X86=MakePassword-${MKPASSWD_VERSION}-win10-x86
MKPASSWD_WIN10_X64=MakePassword-${MKPASSWD_VERSION}-win10-x64
MKPASSWD_WIN10_ARM64=MakePassword-${MKPASSWD_VERSION}-win10-arm64
MKPASSWD_LINUX_X64=MakePassword-${MKPASSWD_VERSION}-linux-x64
MKPASSWD_LINUX_ARM64=MakePassword-${MKPASSWD_VERSION}-linux-arm64
MKPASSWD_OSX_X64=MakePassword-${MKPASSWD_VERSION}-osx-x64
MKPASSWD_OSX_ARM64=MakePassword-${MKPASSWD_VERSION}-osx-arm64

.PHONY: all dist dotnet-build publish clean

all:
	@echo "targets: dist clean"

dist: publish copy-extensions

dotnet-build:
	dotnet build

.PHONY: publish-dist publish-mkpasswd

publish: publish-dist publish-mkpasswd

.PHONY: publish-dist-win10 publish-dist-linux publish-dist-osx

publish-dist: publish-dist-win10 publish-dist-linux publish-dist-osx

.PHONY: publish-dist-win10-x86 publish-dist-win10-x64 publish-dist-win10-arm64

publish-dist-win10: publish-dist-win10-x86 publish-dist-win10-x64 publish-dist-win10-arm64

publish-dist-win10-x86:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r win10-x86 -p:PublishSingleFile=true --self-contained false -o build/${DIST_WIN10_X86}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_WIN10_X86}
	cp scripts/distributor.bat build/${DIST_WIN10_X86}
	cd build && zip -r ${DIST_WIN10_X86}.zip ${DIST_WIN10_X86}
	rm -r build/${DIST_WIN10_X86}

publish-dist-win10-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r win10-x64 -p:PublishSingleFile=true --self-contained false -o build/${DIST_WIN10_X64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_WIN10_X64}
	cp scripts/distributor.bat build/${DIST_WIN10_X64}
	cd build && zip -r ${DIST_WIN10_X64}.zip ${DIST_WIN10_X64}
	rm -r build/${DIST_WIN10_X64}

publish-dist-win10-arm64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r win10-arm64 -p:PublishSingleFile=true --self-contained false -o build/${DIST_WIN10_ARM64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_WIN10_ARM64}
	cp scripts/distributor.bat build/${DIST_WIN10_ARM64}
	cd build && zip -r ${DIST_WIN10_ARM64}.zip ${DIST_WIN10_ARM64}
	rm -r build/${DIST_WIN10_ARM64}

.PHONY: publish-dist-linux-x64 publish-dist-linux-arm64

public-dist-linux: publish-dist-linux-x64 publish-dist-linux-arm64

publish-dist-linux-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r linux-x64 -p:PublishSingleFile=true --self-contained false -o build/${DIST_LINUX_X64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_LINUX_X64}
	cp scripts/distributor.sh build/${DIST_LINUX_X64}
	cd build && tar cvzf ${DIST_LINUX_X64}.tar.gz ${DIST_LINUX_X64}
	rm -r build/${DIST_LINUX_X64}

publish-dist-linux-arm64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r linux-arm64 -p:PublishSingleFile=true --self-contained false -o build/${DIST_LINUX_ARM64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_LINUX_ARM64}
	cp scripts/distributor.sh build/${DIST_LINUX_ARM64}
	cd build && tar cvzf ${DIST_LINUX_ARM64}.tar.gz ${DIST_LINUX_ARM64}
	rm -r build/${DIST_LINUX_ARM64}

.PHONY: publish-dist-osx-x64 publish-dist-osx-arm64

publish-dist-osx: publish-dist-osx-x64 publish-dist-osx-arm64

publish-dist-osx-x64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r osx-x64 -p:PublishSingleFile=true --self-contained false -o build/${DIST_OSX_X64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_OSX_X64}
	cp scripts/distributor.sh build/${DIST_OSX_X64}
	cd build && tar cvzf ${DIST_OSX_X64}.tar.gz ${DIST_OSX_X64}
	rm -r build/${DIST_OSX_X64}

publish-dist-osx-arm64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r osx-arm64 -p:PublishSingleFile=true --self-contained false -o build/${DIST_OSX_ARM64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_OSX_ARM64}
	cp scripts/distributor.sh build/${DIST_OSX_ARM64}
	cd build && tar cvzf ${DIST_OSX_ARM64}.tar.gz ${DIST_OSX_ARM64}
	rm -r build/${DIST_OSX_ARM64}

publish-dist-osx-arm64:
	dotnet publish ${CORE_SRC}/JetBlack.MessageBus.Distributor -r osx-arm64 -p:PublishSingleFile=true --self-contained false -o build/${DIST_OSX_ARM64}
	cp core/src/JetBlack.MessageBus.Distributor/appsettings.json build/${DIST_OSX_ARM64}
	cp scripts/distributor.sh build/${DIST_OSX_ARM64}
	cd build && tar cvzf ${DIST_OSX_ARM64}.tar.gz ${DIST_OSX_ARM64}
	rm -r build/${DIST_OSX_ARM64}

.PHONY: publish-mkpasswd-win10 publish-mkpasswd-linux publish-mkpasswd-osx

publish-mkpasswd: publish-mkpasswd-win10 publish-mkpasswd-linux publish-mkpasswd-osx

publish-mkpasswd-win10: publish-mkpasswd-win10-x86 publish-mkpasswd-win10-x64 publish-mkpasswd-win10-arm64

.PHONY: publish-mkpasswd-win10-x86 publish-mkpasswd-win10-x64 publish-mkpasswd-win10-arm64

publish-mkpasswd-win10-x86:
	dotnet publish ${UTILS_SRC}/MakePassword -r win10-x86 -p:PublishSingleFile=true --self-contained false -o build/${MKPASSWD_WIN10_X86}
	cp scripts/mkpasswd.bat build/${MKPASSWD_WIN10_X86}
	cd build && zip -r ${MKPASSWD_WIN10_X86}.zip ${MKPASSWD_WIN10_X86}
	rm -r build/${MKPASSWD_WIN10_X86}

publish-mkpasswd-win10-x64:
	dotnet publish ${UTILS_SRC}/MakePassword -r win10-x64 -p:PublishSingleFile=true --self-contained false -o build/${MKPASSWD_WIN10_X64}
	cp scripts/mkpasswd.bat build/${MKPASSWD_WIN10_X64}
	cd build && zip -r ${MKPASSWD_WIN10_X64}.zip ${MKPASSWD_WIN10_X64}
	rm -r build/${MKPASSWD_WIN10_X64}

publish-mkpasswd-win10-arm64:
	dotnet publish ${UTILS_SRC}/MakePassword -r win10-arm64 -p:PublishSingleFile=true --self-contained false -o build/${MKPASSWD_WIN10_ARM64}
	cp scripts/mkpasswd.bat build/${MKPASSWD_WIN10_ARM64}
	cd build && zip -r ${MKPASSWD_WIN10_ARM64}.zip ${MKPASSWD_WIN10_ARM64}
	rm -r build/${MKPASSWD_WIN10_ARM64}

publish-mkpasswd-linux: publish-mkpasswd-linux-x64 publish-mkpasswd-linux-arm64

.PHONY: publish-mkpasswd-linux-x64 publish-mkpasswd-linux-arm64

publish-mkpasswd-linux-x64:
	dotnet publish ${UTILS_SRC}/MakePassword -r linux-x64 -p:PublishSingleFile=true --self-contained false -o build/${MKPASSWD_LINUX_X64}
	cp scripts/mkpasswd.sh build/${MKPASSWD_LINUX_X64}
	cd build && tar cvzf ${MKPASSWD_LINUX_X64}.tar.gz ${MKPASSWD_LINUX_X64}
	rm -r build/${MKPASSWD_LINUX_X64}

publish-mkpasswd-linux-arm64:
	dotnet publish ${UTILS_SRC}/MakePassword -r linux-arm64 -p:PublishSingleFile=true --self-contained false -o build/${MKPASSWD_LINUX_ARM64}
	cp scripts/mkpasswd.sh build/${MKPASSWD_LINUX_ARM64}
	cd build && tar cvzf ${MKPASSWD_LINUX_ARM64}.tar.gz ${MKPASSWD_LINUX_ARM64}
	rm -r build/${MKPASSWD_LINUX_ARM64}

publish-mkpasswd-osx: publish-mkpasswd-osx-x64 publish-mkpasswd-osx-arm64

.PHONY: publish-mkpasswd-osx-x64 publish-mkpasswd-osx-arm64

publish-mkpasswd-osx-x64:
	dotnet publish ${UTILS_SRC}/MakePassword -r osx-x64 -p:PublishSingleFile=true --self-contained false -o build/${MKPASSWD_OSX_X64}
	cp scripts/mkpasswd.sh build/${MKPASSWD_OSX_X64}
	cd build && tar cvzf ${MKPASSWD_OSX_X64}.tar.gz ${MKPASSWD_OSX_X64}
	rm -r build/${MKPASSWD_OSX_X64}

publish-mkpasswd-osx-arm64:
	dotnet publish ${UTILS_SRC}/MakePassword -r osx-arm64 -p:PublishSingleFile=true --self-contained false -o build/${MKPASSWD_OSX_ARM64}
	cp scripts/mkpasswd.sh build/${MKPASSWD_OSX_ARM64}
	cd build && tar cvzf ${MKPASSWD_OSX_ARM64}.tar.gz ${MKPASSWD_OSX_ARM64}
	rm -r build/${MKPASSWD_OSX_ARM64}

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
	-rm -r build
	find . -type d -name bin -exec rm -rf {} \;	
	find . -type d -name obj -exec rm -rf {} \;	
