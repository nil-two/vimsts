.PHONY: all windows mac linux

GAME_NAME = vimsts

all: windows mac linux

windows:
	Unity -batchmode -nographics -quit -projectPath . -buildWindows64Player ./Builds/$(GAME_NAME)_windows/$(GAME_NAME).exe
	cp README.md ./Builds/$(GAME_NAME)_windows/README.txt
	cd Builds && zip -r  $(GAME_NAME)_windows.zip ./$(GAME_NAME)_windows
	rm -rf ./Builds/$(GAME_NAME)_windows

mac:
	Unity -batchmode -nographics -quit -projectPath . -buildOSXUniversalPlayer ./Builds/$(GAME_NAME)_mac/$(GAME_NAME)
	cp README.md ./Builds/$(GAME_NAME)_mac/README.txt
	cd Builds && zip -r  $(GAME_NAME)_mac.zip ./$(GAME_NAME)_mac
	rm -rf ./Builds/$(GAME_NAME)_mac

linux:
	Unity -batchmode -nographics -quit -projectPath . -buildLinux64Player ./Builds/$(GAME_NAME)_linux/$(GAME_NAME)
	cp README.md ./Builds/$(GAME_NAME)_linux/README.txt
	cd Builds && zip -r  $(GAME_NAME)_linux.zip ./$(GAME_NAME)_linux
	rm -rf ./Builds/$(GAME_NAME)_linux
