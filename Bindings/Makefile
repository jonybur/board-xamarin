XBUILD=/Applications/Xcode-beta.app/Contents/Developer/usr/bin/xcodebuild
PROJECT_ROOT=./MGImageUtilities
PROJECT=$(PROJECT_ROOT)/MGImageUtilities.xcodeproj
TARGET=MGImageUtilities

all: libMGImageUtilities.a

libMGImageUtilities-i386.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphonesimulator -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphonesimulator/lib$(TARGET).a $@

libMGImageUtilities-armv7.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch armv7 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libMGImageUtilities-arm64.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch arm64 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libMGImageUtilities.a: libMGImageUtilities-i386.a libMGImageUtilities-armv7.a libMGImageUtilities-arm64.a
	xcrun -sdk iphoneos lipo -create -output $@ $^

clean:
	-rm -f *.a *.dll