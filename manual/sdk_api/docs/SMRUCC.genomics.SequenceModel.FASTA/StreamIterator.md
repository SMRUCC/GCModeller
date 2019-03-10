# StreamIterator
_namespace: [SMRUCC.genomics.SequenceModel.FASTA](./index.md)_

读取超大型的fasta文件所需要的一个数据对象



### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.FASTA.StreamIterator.#ctor(System.String)
```
从指定的文件之中构建一个读取超大型的fasta文件所需要的一个数据对象

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### __loops
```csharp
SMRUCC.genomics.SequenceModel.FASTA.StreamIterator.__loops(System.Collections.Generic.List{System.String})
```
Loops on each block of data

|Parameter Name|Remarks|
|--------------|-------|
|stream|-|


#### ReadStream
```csharp
SMRUCC.genomics.SequenceModel.FASTA.StreamIterator.ReadStream
```
Read all sequence from the fasta file.

#### SeqSource
```csharp
SMRUCC.genomics.SequenceModel.FASTA.StreamIterator.SeqSource(System.String,System.String[],System.Boolean)
```
全部都是使用@``T:SMRUCC.genomics.SequenceModel.FASTA.StreamIterator``对象来进行读取的

|Parameter Name|Remarks|
|--------------|-------|
|handle|File path or directory.|


#### Split
```csharp
SMRUCC.genomics.SequenceModel.FASTA.StreamIterator.Split(System.Int32)
```
子集里面的序列元素的数目

|Parameter Name|Remarks|
|--------------|-------|
|size|-|



### Properties

#### DefaultSuffix
默认的Fasta文件拓展名列表
