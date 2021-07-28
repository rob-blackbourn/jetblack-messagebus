RELEASE=jetblack-messagebus-3.0.0

.PHONY: all build clean

all:
	@echo "targets: build clean."

build:
	dotnet build
	mkdir -p build/${RELEASE}/core/JetBlack.MessageBus.Distributor
	cp -r core/src/JetBlack.MessageBus.Distributor/bin/Debug/* build/${RELEASE}/core/JetBlack.MessageBus.Distributor
	mkdir -p build/${RELEASE}/core/JetBlack.MessageBus.Adapters
	cp -r core/src/JetBlack.MessageBus.Adapters/bin/Debug/* build/${RELEASE}/core/JetBlack.MessageBus.Adapters
	mkdir -p build/${RELEASE}/extensions/JetBlack.MessageBus.Extension.LdapAuthentication
	cp -r extensions/authentication/src/JetBlack.MessageBus.Extension.LdapAuthentication/bin/Debug/* build/${RELEASE}/extensions/JetBlack.MessageBus.Extension.LdapAuthentication
	mkdir -p build/${RELEASE}/extensions/JetBlack.MessageBus.Extension.PasswordFileAuthentication
	cp -r extensions/authentication/src/JetBlack.MessageBus.Extension.PasswordFileAuthentication/bin/Debug/* build/${RELEASE}/extensions/JetBlack.MessageBus.Extension.PasswordFileAuthentication
	mkdir -p build/${RELEASE}/extensions/JetBlack.MessageBus.Extension.JwtAuthentication
	cp -r extensions/authentication/src/JetBlack.MessageBus.Extension.JwtAuthentication/bin/Debug/* build/${RELEASE}/extensions/JetBlack.MessageBus.Extension.JwtAuthentication
	cp scripts/* build/${RELEASE}
	cd build && tar cvzf ${RELEASE}.tar.gz ${RELEASE}
	cd build && zip -r ${RELEASE}.zip ${RELEASE}

clean:
	rm -r build
	find . -type d -name bin -exec rm -r {} \;	
	find . -type d -name obj -exec rm -r {} \;	
