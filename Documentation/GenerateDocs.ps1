### Get JSON content
$JSON 	 = Get-Content -Path ../package.json

### Retrieve version from JSON
# Get line with "version" key
$match = [regex]::new('"version":\s?"((\d\.?)+)"').Matches($JSON)

# Retrieve version rom match
$ProjectVersion = $match.groups[1].Value

Write-Output "Generating documents for v$ProjectVersion"

### Generate documentation
# Pipe output with project version to doxygen command
cmd.exe /c "(type Doxyfile & echo. & echo PROJECT_NUMBER=$ProjectVersion) | doxygen -"
