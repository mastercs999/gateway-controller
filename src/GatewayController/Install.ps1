param($installPath, $toolsPath, $package, $project)

$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("IBController.ini").Properties.Item("CopyToOutputDirectory").Value = 1
$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("IBController.jar").Properties.Item("CopyToOutputDirectory").Value = 1
$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("IBControllerGatewayStart.bat").Properties.Item("CopyToOutputDirectory").Value = 1
$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("IBControllerStop.bat").Properties.Item("CopyToOutputDirectory").Value = 1
$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("version").Properties.Item("CopyToOutputDirectory").Value = 1

$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("Scripts").ProjectItems.Item("DisplayBannerAndLaunch.bat").Properties.Item("CopyToOutputDirectory").Value = 1
$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("Scripts").ProjectItems.Item("getDayOfWeek.bat").Properties.Item("CopyToOutputDirectory").Value = 1
$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("Scripts").ProjectItems.Item("IBController.bat").Properties.Item("CopyToOutputDirectory").Value = 1
$configItem = $project.ProjectItems.Item("IBController").ProjectItems.Item("Scripts").ProjectItems.Item("SendIBControllerCommands.vbs").Properties.Item("CopyToOutputDirectory").Value = 1
