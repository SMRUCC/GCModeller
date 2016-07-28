---
title: FileSystem
---

# FileSystem
_namespace: [Microsoft.VisualBasic.ComputingServices.FileSystem](N-Microsoft.VisualBasic.ComputingServices.FileSystem.html)_

Provides properties and methods for working with drives, files, and directories on remote server.
 (分布式文件系统)



### Methods

#### CopyDirectory
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.CopyDirectory(System.String,System.String,System.Boolean)
```
Copies the contents of a directory to another directory.

|Parameter Name|Remarks|
|--------------|-------|
|sourceDirectoryName|The directory to be copied.|
|destinationDirectoryName|The location to which the directory contents should be copied.|
|overwrite|True to overwrite existing files; otherwise False. Default is False.|


#### CopyFile
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.CopyFile(System.String,System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|sourceFileName|-|
|destinationFileName|-|
|overwrite|-|


#### CreateDirectory
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.CreateDirectory(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|directory|-|


#### DeleteDirectory
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.DeleteDirectory(System.String,Microsoft.VisualBasic.FileIO.DeleteDirectoryOption)
```


|Parameter Name|Remarks|
|--------------|-------|
|directory|-|
|onDirectoryNotEmpty|-|


#### DeleteFile
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.DeleteFile(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|file|-|


#### DirectoryExists
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.DirectoryExists(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|directory|-|


#### FileExists
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.FileExists(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|file|-|


#### FindInFiles
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.FindInFiles(System.String,System.String,System.Boolean,Microsoft.VisualBasic.FileIO.SearchOption,System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|directory|-|
|containsText|-|
|ignoreCase|-|
|searchType|-|
|fileWildcards|-|


#### GetDirectories
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.GetDirectories(System.String,Microsoft.VisualBasic.FileIO.SearchOption,System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|directory|-|
|searchType|-|
|wildcards|-|


#### GetDirectoryInfo
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.GetDirectoryInfo(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|directory|-|


#### GetDriveInfo
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.GetDriveInfo(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|drive|-|


#### GetFileInfo
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.GetFileInfo(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|file|-|


#### GetFiles
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.GetFiles(System.String,Microsoft.VisualBasic.FileIO.SearchOption,System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|directory|-|
|searchType|-|
|wildcards|-|


#### GetParentPath
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.GetParentPath(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### GetTempFileName
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.GetTempFileName
```


#### MoveDirectory
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.MoveDirectory(System.String,System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|sourceDirectoryName|-|
|destinationDirectoryName|-|
|overwrite|-|


#### MoveFile
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.MoveFile(System.String,System.String,System.Boolean)
```
Moves a file to a new location.

|Parameter Name|Remarks|
|--------------|-------|
|sourceFileName|Path of the file to be moved.|
|destinationFileName|Path of the directory into which the file should be moved.|
|overwrite|True to overwrite existing files; otherwise False. Default is False.|


#### OpenFileHandle
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.OpenFileHandle(System.String,System.IO.FileMode,System.IO.FileAccess,Microsoft.VisualBasic.Net.IPEndPoint)
```
在远程服务器上面打开一个文件句柄

|Parameter Name|Remarks|
|--------------|-------|
|file|-|


#### OpenTextFieldParser
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.OpenTextFieldParser(System.String,System.Int32[])
```
The OpenTextFieldParser method allows you to create a Microsoft.VisualBasic.FileIO.TextFieldParser
 object, which provides a way to easily and efficiently parse structured text
 files, such as logs. The TextFieldParser object can be used to read both delimited
 and fixed-width files.

|Parameter Name|Remarks|
|--------------|-------|
|file|The file to be opened with the TextFieldParser.|
|fieldWidths|Widths of the fields.|

_returns: Microsoft.VisualBasic.FileIO.TextFieldParser to read the specified file._

#### OpenTextFileReader
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.OpenTextFileReader(System.String,System.Text.Encoding)
```
Opens a System.IO.StreamReader object to read from a file.

|Parameter Name|Remarks|
|--------------|-------|
|file|File to be read.|
|encoding|The encoding to use for the file contents. Default is ASCII.|

_returns: System.IO.StreamReader object to read from the file_

#### OpenTextFileWriter
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.OpenTextFileWriter(System.String,System.Boolean,System.Text.Encoding)
```
Opens a System.IO.StreamWriter to write to the specified file.

|Parameter Name|Remarks|
|--------------|-------|
|file|File to be written to.|
|append|True to append to the contents in the file; False to overwrite the contents of
 the file. Default is False.|
|encoding|Encoding to be used in writing to the file. Default is ASCII.|

_returns: System.IO.StreamWriter object to write to the specified file._

#### ReadAllBytes
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.ReadAllBytes(System.String)
```
Returns the contents of a file as a byte array.

|Parameter Name|Remarks|
|--------------|-------|
|file|File to be read.|

_returns: Byte array containing the contents of the file._

#### ReadAllText
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.ReadAllText(System.String,System.Text.Encoding)
```
Returns the contents of a text file as a String.

|Parameter Name|Remarks|
|--------------|-------|
|file|Name and path of the file to read.|
|encoding|Character encoding to use in reading the file. Default is UTF-8.|

_returns: String containing the contents of the file._

#### RenameDirectory
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.RenameDirectory(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|directory|-|
|newName|-|


#### RenameFile
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.RenameFile(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|file|-|
|newName|-|


#### WriteAllBytes
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.WriteAllBytes(System.String,System.Byte[],System.Boolean)
```
Writes data to a binary file.

|Parameter Name|Remarks|
|--------------|-------|
|file|Path and name of the file to be written to.|
|data|Data to be written to the file.|
|append|True to append to the file contents; False to overwrite the file contents. Default
 is False.|


#### WriteAllText
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem.WriteAllText(System.String,System.String,System.Boolean,System.Text.Encoding)
```
Writes text to a file.

|Parameter Name|Remarks|
|--------------|-------|
|file|File to be written to.|
|text|Text to be written to file.|
|append|True to append to the contents of the file; False to overwrite the contents of
 the file.|
|encoding|What encoding to use when writing to file.|



### Properties

#### CurrentDirectory
Gets or sets the current directory.
#### Drives
Returns a read-only collection of all available drive names.
#### Portal
远端服务器的开放的句柄端口
