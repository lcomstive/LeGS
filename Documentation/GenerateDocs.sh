### Retrieve version from JSON
JSON=$(cat ../package.json)
REGEX="\"version\":\\s?\"(([0-9]\\.?)+)\""
if ! [[ $JSON =~ $REGEX ]] ; then
	echo "No version found in ../package.json"
	exit -1
fi

# Retrieve version from match
VERSION=${BASH_REMATCH[1]}
echo "Generating documents for v$VERSION"

### Generate documentation
# Pipe output with project version to doxygen command
( cat Doxyfile ; echo ; echo "PROJECT_NUMBER=$VERSION" ) | doxygen -
