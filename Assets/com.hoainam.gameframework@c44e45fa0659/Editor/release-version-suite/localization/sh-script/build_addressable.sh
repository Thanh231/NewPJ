
UNITY_EXE=$1
PROJECT_PATH=$2

"$UNITY_EXE" -quit -batchmode -nographics -projectPath $PROJECT_PATH -executeMethod "BuildAddressable.Build"