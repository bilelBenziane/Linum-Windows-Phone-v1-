param($installPath, $toolsPath, $package, $project)

$configFileName = "ApplicationInsights.config"
# 2 represent 'Copy if newer' option for 'Copy To Output Directory'
$copyToOutputDirValue = 2
$copyToOutputDirProperty = "CopyToOutputDirectory" 
$file = $project.ProjectItems.Item($configFileName)

# set 'Copy To Output Directory' to 'Copy if newer'
IF ($file)
{
    $copyToOutput = $file.Properties.Item($copyToOutputDirProperty)
    $copyToOutput.Value = $copyToOutputDirValue
}
