
UNITY_EXE=$1
PROJECT_PATH=$2
PLATFORM=$3

case "$PLATFORM" in
	"StandaloneWindows64") BUILD_FUNCTION_NAME="BuildAddressable.SwitchPlatform_windows"
	;;
	"Android") BUILD_FUNCTION_NAME="BuildAddressable.SwitchPlatform_android"
	;;
	"iOS") BUILD_FUNCTION_NAME="BuildAddressable.SwitchPlatform_ios"
	;;
esac

"$UNITY_EXE" -quit -batchmode -nographics -projectPath $PROJECT_PATH -executeMethod $BUILD_FUNCTION_NAME