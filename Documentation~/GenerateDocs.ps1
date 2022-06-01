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

### Rename generated files
# Get-ChildItem -Path ./html -Exclude ('dir_*', 'namespace_*') | Foreach-Object {
# 	$oldFilename = $_.BaseName + $_.Extension
# 	$match = [regex]::new('(\w+_1_1_|md__.*_|class_*_)([a-zA-Z0-9_-]+)').Matches($oldFilename)
# 
# 	# Write-Output "Matching '$oldFilename' - " $match.groups
# 	if(!$match.groups) { return }
# 	
# 	$newFilename = $match.groups[2].Value + $_.Extension
# 	# Write-Output "Replacing $oldFilename with $newFilename"
# 
# 	Move-Item -Force -Path "html/$oldFilename" -Destination "html/$newFilename"
# }
# 
# ### Rename generated hyperlinks
# Get-ChildItem -Path ./html -Recurse -Include ('*.html', '*.js') -Exclude 'jquery.js' |
# Foreach-Object {
# 	(Get-Content $_.FullName) -Replace [regex]::new('(\w+_1_1_)|(md__.*_)|(class_*_)'), '' | Set-Content $_.FullName
# }