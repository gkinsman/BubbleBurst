#
# RunMany.ps1
# Warning: this melts the CPU
#

$files = "Game1.txt","Game2.txt","Game3.txt"
$depthPenalties = 7,8,9,10,11,12,13

foreach($file in $files) {
	foreach($depthPenalty in $depthPenalties) {
		
		$args = "-gameFile=$file -depthPenalty=$depthPenalty"

		Write-Host $args
		start-process BubbleBurst.Runner.exe -ArgumentList $args
	}
}
