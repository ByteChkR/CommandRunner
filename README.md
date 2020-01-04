# Command Runner
A small project that is made to ease the use of Commandline Programs that require a lot of commands and to remove the need to write a cli parsing system for every project.

## Features
The System Features Multiple Command Styles:
```
	Default: "--command"
	and Short: "-c"
```
The Arguments can be loaded from file by passing the path prepended with the FilePathPrefix: 
```
-c @Path/To/File.txt
```
Multiple Executions of the Same Command is possible(Will be executed in 3 executions):
```
-c @PathToFile.txt -c Other Arguments --command Different Arguments
```