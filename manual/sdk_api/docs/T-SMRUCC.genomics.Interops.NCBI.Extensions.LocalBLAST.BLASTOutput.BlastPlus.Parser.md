---
title: Parser
---

# Parser
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.html)_





### Methods

#### __fileSizeTooLarge
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.__fileSizeTooLarge(System.String)
```
单条字符串的最大长度好像只有700MB的内存左右

#### __getParser
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.__getParser(System.String)
```
获取blastn或者blastp的解析器

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### __tryParseUltraLarge
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.__tryParseUltraLarge(System.String,System.Int64,System.Text.Encoding)
```
Dealing with the file size large than 2GB.(当Blast日志文件的大小大于100M的时候，可以使用这个方法进行加载，函数会自动判断日志是否为blastn还是blastp)

|Parameter Name|Remarks|
|--------------|-------|
|path|File path of the blast output file.|
|CHUNK_SIZE|The parameter unit for this value is Byte, so you need to multiply the 1024*1024 to get a MB level value.
 It seems 768MB possibly is the up bound of the Utf8.GetString function. default is 64MB|


#### GetType
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.GetType(System.String,System.Text.Encoding)
```
The target blast output text file is from the blastp or blastn?

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|Encoding|-|


#### LoadBlastOutput
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.LoadBlastOutput(System.String)
```
Load the blast output from a local file, if the exceptions happened during the data loading process, then a 
 null value will be return.
 (读取blast日志文件，当发生错误的时候，函数返回空值)

|Parameter Name|Remarks|
|--------------|-------|
|path|The file path of the blast output|


#### ParsingSizeAuto
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.ParsingSizeAuto(System.String,System.Text.Encoding)
```
The parser worker will be select automatically based on the blast output file size.
 (当blast的输出文件非常大的时候，会自动选择分片段解析读取的方法工作)

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|encoding|-|


#### Transform
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.Transform(System.String,System.Int64,System.Action{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query[]},System.Text.Encoding)
```
处理非常大的blast输出文件的时候所需要的，大小大于10GB的文件建议使用这个方法处理

#### TryParse
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParse(System.String,System.Text.Encoding)
```
Log file size smaller than 512MB is recommended using this loading method, if the log file size is large than 512MB, please using the method @"M:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParseUltraLarge(System.String,System.Int64,System.Text.Encoding)".
 (当blast输出的日志文件比较小的时候，可以使用当前的这个方法进行读取)

|Parameter Name|Remarks|
|--------------|-------|
|Path|Original plant text file path of the blast output file.|


#### TryParseBlastnOutput
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParseBlastnOutput(System.String)
```
Load the blastn output data.(读取blastn日志输出的数据.)

|Parameter Name|Remarks|
|--------------|-------|
|Path|The blastn output file path.|


#### TryParseUltraLarge
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParseUltraLarge(System.String,System.Int64,System.Text.Encoding)
```
Dealing with the file size large than 2GB.(当Blast日志文件的大小大于100M的时候，可以使用这个方法进行加载，函数会自动判断日志是否为blastn还是blastp)

|Parameter Name|Remarks|
|--------------|-------|
|Path|File path of the blast output file.|
|CHUNK_SIZE|The parameter unit for this value is Byte, so you need to multiply the 1024*1024 to get a MB level value.
 It seems 768MB possibly is the up bound of the Utf8.GetString function. default is 64MB|



### Properties

#### CHUNK_SIZE
It seems 768MB possibly is the up bound of the Utf8.GetString function.
