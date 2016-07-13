---
title: RSystem
---

# RSystem
_namespace: [RDotNET.Extensions.VisualBasic](N-RDotNET.Extensions.VisualBasic.html)_

R Engine extensions.(似乎对于RDotNet而言，在一个应用程序的实例进程之中仅允许一个REngine的实例存在，所以在这里就统一的使用一个公共的REngine的实例对象)



### Methods

#### #cctor
```csharp
RDotNET.Extensions.VisualBasic.RSystem.#cctor
```
Initialize the default R Engine.

#### getwd
```csharp
RDotNET.Extensions.VisualBasic.RSystem.getwd
```
Display the current working directory

#### Library
```csharp
RDotNET.Extensions.VisualBasic.RSystem.Library(System.String)
```
Load a available R package which was installed in the R system.(加载一个可用的R包)

|Parameter Name|Remarks|
|--------------|-------|
|packageName|-|


#### packageVersion
```csharp
RDotNET.Extensions.VisualBasic.RSystem.packageVersion(System.String)
```
Parses and returns the ‘DESCRIPTION’ file of a package.

|Parameter Name|Remarks|
|--------------|-------|
|pkg|a character string with the package name.|


#### Seq
```csharp
RDotNET.Extensions.VisualBasic.RSystem.Seq(System.Int32,System.Int32,System.Double)
```
[Sequence Generation] Generate regular sequences. seq is a standard generic with a default method.

|Parameter Name|Remarks|
|--------------|-------|
|From|the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.|
|To|the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.|
|By|number: increment of the sequence|


#### Source
```csharp
RDotNET.Extensions.VisualBasic.RSystem.Source(System.String,System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.Int32,System.Boolean,System.String,System.Boolean,System.Int32,System.Boolean)
```
The R engine execute a R script. source causes R to accept its input from the named file or URL or connection.
 Input is read and parsed from that file until the end of the file is reached, then the parsed expressions are
 evaluated sequentially in the chosen environment.
 (R引擎执行文件系统之中的一个R脚本)

|Parameter Name|Remarks|
|--------------|-------|
|file|a connection Or a character String giving the pathname Of the file Or URL To read from. ""
 indicates the connection stdin().
 |
|local|TRUE, FALSE or an environment, determining where the parsed expressions are evaluated.
 FALSE (the default) corresponds to the user's workspace (the global environment) and TRUE to the environment
 from which source is called.|
|echo|logical; if TRUE, each expression is printed after parsing, before evaluation.|
|printEval|logical; if TRUE, the result of eval(i) is printed for each expression i; defaults to the value of echo.|
|verbose|if TRUE, more diagnostics (than just echo = TRUE) are printed during parsing and evaluation of input,
 including extra info for each expression.|
|promptEcho|character; gives the prompt to be used if echo = TRUE.|
|maxDeparseLength|integer; is used only if echo is TRUE and gives the maximal number of characters output for
 the deparse of a single expression.|
|chdir|logical; if TRUE And file Is a pathname, the R working directory Is temporarily changed to the
 directory containing file for evaluating.|
|encoding|character vector. The encoding(s) to be assumed when file is a character string: see file.
 A possible value is "unknown" when the encoding is guessed: see the ‘Encodings’ section.|
|continueEcho|character; gives the prompt to use on continuation lines if echo = TRUE.|
|skipEcho|integer; how many comment lines at the start of the file to skip if echo = TRUE.|
|keepSource|logical: should the source formatting be retained When echoing expressions, If possible?|

> 
>  Note that running code via source differs in a few respects from entering it at the R command line. Since expressions are not executed
>  at the top level, auto-printing is not done. So you will need to include explicit print calls for things you want to be printed
>  (and remember that this includes plotting by lattice, FAQ Q7.22). Since the complete file is parsed before any of it is run, syntax
>  errors result in none of the code being run. If an error occurs in running a syntactically correct script, anything assigned into the
>  workspace by code that has been run will be kept (just as from the command line), but diagnostic information such as traceback() will
>  contain additional calls to withVisible.
> 
>  All versions Of R accept input from a connection With End Of line marked by LF (As used On Unix), CRLF (As used On DOS/Windows) Or CR
>  (As used On classic Mac OS) And map this To newline. The final line can be incomplete, that Is missing the final End-Of-line marker.
> 
>  If keep.source Is True(the Default In interactive use), the source Of functions Is kept so they can be listed exactly As input.
> 
>  Unlike input from a console, lines In the file Or On a connection can contain an unlimited number Of characters.
> 
>  When skip.echo > 0, that many comment lines at the start of the file will Not be echoed. This does Not affect the execution of the code at all.
>  If there are executable lines within the first skip.echo lines, echoing will start with the first of them.
> 
>  If echo Is True And a deparsed expression exceeds max.deparse.length, that many characters are output followed by .... [TRUNCATED] .
> 
>  [Encodings]
>  By Default the input Is read And parsed In the current encoding Of the R session. This Is usually what it required, but occasionally re-encoding
>  Is needed, e.g. If a file from a UTF-8-Using system Is To be read On Windows (Or vice versa).
> 
>  The rest Of this paragraph applies If file Is an actual filename Or URL (And Not "" nor a connection). If encoding = "unknown", an attempt Is
>  made To guess the encoding: the result Of localeToCharset() Is used As a guide. If encoding has two Or more elements, they are tried In turn
>  until the file/URL can be read without Error In the trial encoding. If an actual encoding Is specified (rather than the Default Or "unknown")
>  In a Latin-1 Or UTF-8 locale Then character strings In the result will be translated To the current encoding And marked As such (see Encoding).
> 
>  If file Is a connection (including one specified by "", it Is Not possible To re-encode the input inside source, And so the encoding argument
>  Is just used To mark character strings In the parsed input In Latin-1 And UTF-8 locales: see parse.
>  

#### TryInit
```csharp
RDotNET.Extensions.VisualBasic.RSystem.TryInit(System.String)
```
Manual set up R init environment.

|Parameter Name|Remarks|
|--------------|-------|
|R_HOME|-|



### Properties

#### RColors
枚举R中所有的颜色代码
#### RServer
The default R Engine server.
