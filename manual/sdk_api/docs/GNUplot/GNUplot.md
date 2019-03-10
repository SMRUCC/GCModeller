# GNUplot
_namespace: [GNUplot](./index.md)_

Gnuplot is a portable command-line driven graphing utility for Linux, OS/2, MS Windows, OSX, VMS, and many other platforms. 
 The source code is copyrighted but freely distributed (i.e., you don't have to pay for it). It was originally created to 
 allow scientists and students to visualize mathematical functions and data interactively, but has grown to support many 
 non-interactive uses such as web scripting. It is also used as a plotting engine by third-party applications like Octave. 
 Gnuplot has been supported and under active development since 1986.



### Methods

#### Close
```csharp
GNUplot.GNUplot.Close
```
Close GNUplot main window

#### makeContourFile
```csharp
GNUplot.GNUplot.makeContourFile(System.String,System.String)
```
these makecontourFile functions should probably be merged into one function and use a StoredPlot parameter

|Parameter Name|Remarks|
|--------------|-------|
|fileOrFunction|-|
|outputFile|-|


#### Start
```csharp
GNUplot.GNUplot.Start(System.String)
```
If you have change the default installed location of the gnuplot, then this 
 function is required for manually starting the gnuplot services.
 (假若从默认的位置启动程序没有成功的话，会需要使用这个函数从自定义位置启动程序)

|Parameter Name|Remarks|
|--------------|-------|
|gnuplot|
 The file path of the program file: ``gnuplot.exe``
 |


_returns: The gnuplot services start successfully or not?_


### Properties

#### __gnuplot
gnuplot interop services instance.
