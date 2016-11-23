# FastQFile
_namespace: [SMRUCC.genomics.SequenceModel.Fastaq](./index.md)_

There is no standard file extension for a FASTQ file, but .fq and .fastq, are commonly used.



### Methods

#### Load
```csharp
SMRUCC.genomics.SequenceModel.Fastaq.FastQFile.Load(System.String,Microsoft.VisualBasic.Text.Encodings)
```
Load the fastq data from a specific file handle.(从一个特定的文件句柄之中加载fastq文件的数据)

|Parameter Name|Remarks|
|--------------|-------|
|path|The file handle of the fastq data.|


#### ToFasta
```csharp
SMRUCC.genomics.SequenceModel.Fastaq.FastQFile.ToFasta(System.Boolean)
```
Convert fastaq data into a fasta data file.


