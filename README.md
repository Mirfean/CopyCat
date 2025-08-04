# CopyCat

Easy and simple file synchronization tool by Patryk Gusarzewski.

HOW TO USE

To use CopyCat, follow these steps:
	1.Open the terminal or command prompt.
	2.Navigate to the directory where CopyCat is located.
	3.Run the command: "CopyCat.exe <source_directory> <destination_directory> <synchronization_interval> <log_file_path>>"

Example:
C:\CopyCat>CopyCat.exe "C:\Private\Folder" "D:\Backup" "300" "D:\Logs"

`<source_directory>` - path to the directory you want to synchronize.
`<destination_directory>` - path to the directory where you want to synchronize files. If you already have that directory, CopyCat will ask if you want to clear it.
`<synchronization_interval>` - interval in seconds at which you want to synchronize files. You can use few formats(look below)
`<log_file_path>` - path with directory where you want to save your logs.

Synchronization interval formats (examples):
- `15` - 15 seconds *default*
- `21s` - 21 second
- `4m` - 4 minutes
- `37h` - 37 hours
- `4d` - 4 days

DO NOT USE MORE THAN 1 FORMAT AT ONCE!

"17d 7h" - will not work.
"123m" - works like a charm.