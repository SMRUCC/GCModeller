---
title: Writer
---

# Writer
_namespace: [Microsoft.VisualBasic.DocumentFormat.Csv](N-Microsoft.VisualBasic.DocumentFormat.Csv.html)_

@"M:Microsoft.VisualBasic.DocumentFormat.Csv.Writer.Dispose"的时候会自动保存Csv文件的数据



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.Writer.#ctor(Microsoft.VisualBasic.DocumentFormat.Csv.Class,System.String,Microsoft.VisualBasic.TextEncodings.Encodings)
```


|Parameter Name|Remarks|
|--------------|-------|
|cls|Schema maps|
|DIR|
 Dump data to this directory. The index file will using ``#.Csv`` as its default name.
 |
|encoding|Text document encoding of the csv file.|


#### WriteRow
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.Writer.WriteRow(System.Object,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|obj|.NET object for maps to csv data row.|
|i|Uid reference for the external table.|



### Properties

#### __file
File handle for the csv data file.
