---
title: FastaToken
---

# FastaToken
_namespace: [SMRUCC.genomics.SequenceModel.FASTA](N-SMRUCC.genomics.SequenceModel.FASTA.html)_

The FASTA format file of a bimolecular sequence.(Notice that this file is 
 only contains on sequence.)
 FASTA格式的生物分子序列文件。(但是请注意：文件中只包含一条序列的情况，假若需要自定义所生成的FASTA文件的标题的格式，请复写@"M:SMRUCC.genomics.SequenceModel.FASTA.FastaToken.ToString"方法)



### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|path|File path of a fasta sequence.|


#### Complement
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Complement(SMRUCC.genomics.SequenceModel.FASTA.FastaToken)
```
Gets the complement sequence data of a specific fasta sequence.(获取某一条核酸序列的互补序列)

|Parameter Name|Remarks|
|--------------|-------|
|FASTA|The target fasta sequence object should be a nucleotide sequence, or this function will returns a null value.
 (目标FASTA对象必须为核酸序列，否则返回空值)|


#### Copy
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Copy
```
Copy data to a new FASTA object.(将本对象的数据拷贝至一个新的FASTA序列对象中)

#### Copy``1
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Copy``1
```
Copy the value to a new specific type fasta object.

#### CopyTo``1
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.CopyTo``1(``0@)
```
Copy the value in current fasta object into another fasta object.(将当前的序列数据复制到目标序列数据对象之中)

|Parameter Name|Remarks|
|--------------|-------|
|FastaObject|The target fasta object will be copied to, if the value is null of this fasta 
 object, then this function will generate a new fasta sequence object.(假若值为空，则会创建一个新的序列对象)|


#### Equals
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Equals(System.Object)
```
The Fasta sequence equals on both sequence data and title information.

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|


#### GenerateDocument
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.GenerateDocument(System.Int32,System.Boolean)
```
Generate a FASTA file data text string.(将这个FASTA对象转换为文件格式以方便进行存储)

|Parameter Name|Remarks|
|--------------|-------|
|overrides|是否使用@"M:SMRUCC.genomics.SequenceModel.FASTA.FastaToken.ToString"方法进行标题的复写，假若为假，则默认使用Attributes属性进行标题的生成，
 因为在继承类之中可能会复写ToString函数以生成不同的标题格式，则可以使用这个参数来决定是否使用复写的格式。|
|LineBreak|大于0的数值会换行，小于或者等于0的数值不会换行|


#### GrepTitle
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.GrepTitle(Microsoft.VisualBasic.Text.TextGrepMethod)
```
You can using this function to convert the title from current format into another format.(使用这个方法将Fasta序列对象的标题从当前的格式转换为另外一种格式)

|Parameter Name|Remarks|
|--------------|-------|
|MethodPointer|-|


#### Load
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Load(System.String)
```
Load a single sequence fasta file object, if the target file path is not exists on the file system or 
 the file format is not correct, then this function will returns a null object value. 
 (这是一个安全的函数：从文件之中加载一条Fasta序列，当目标文件**File**不存在或者没有序列数据的时候，会返回空值)

|Parameter Name|Remarks|
|--------------|-------|
|File|目标序列文件的文件路径|


#### LoadNucleotideData
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.LoadNucleotideData(System.String,System.Boolean)
```
Load the fasta sequence file as a nucleotide sequence from a specific **path**, the function will returns 
 a null value if the sequence contains some non-nucleotide character.
 (从一条序列的FASTA文件之中加载一条核酸序列，当含有非法字符的时候，会返回空文件)

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|Explicit|当拥有空格数据的时候，假若参数为真，则会返回空文件，反之不会做任何处理|


#### op_Explicit
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.op_Explicit(SMRUCC.genomics.SequenceModel.FASTA.FastaToken)~System.String
```
Generate the document text value for this fasta object.

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|


#### op_Implicit
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.op_Implicit(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature)~SMRUCC.genomics.SequenceModel.FASTA.FastaToken
```
Convert the specific feature data in Genbank database into a fasta sequence.

|Parameter Name|Remarks|
|--------------|-------|
|Feature|只是从这个特性对象之中得到蛋白质序列|


#### ParseFromStream
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.ParseFromStream(System.Collections.Generic.IEnumerable{System.String})
```
Parsing a fasta sequence object from a collection of string value.(从字符串数据之中解析出Fasta序列数据)

|Parameter Name|Remarks|
|--------------|-------|
|stream|-|


#### Reverse
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Reverse
```
Reverse the sequence data of the current fasta sequence object and then returns the new created fasta object.

#### SaveAsOneLine
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.SaveAsOneLine(System.String,System.Text.Encoding)
```
过长的序列不每隔60个字符进行一次换行，而是直接使用一行数据进行存储

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|
|encoding|-|


#### SaveTo
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.SaveTo(System.Int32,System.String,System.Text.Encoding)
```
Save the current fasta sequence object into the file system. smaller than 1 will means no line break in the saved fasta sequence.

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|
|encoding|-|


#### ToString
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.ToString
```
Get the title of this fasta object.(返回FASTA对象的标题，在所返回的值之中不包含有fasta标题之中的第一个字符>)

#### TryParse
```csharp
SMRUCC.genomics.SequenceModel.FASTA.FastaToken.TryParse(System.String)
```
Try parsing a fasta sequence object from a string chunk value.(尝试从一个字符串之中解析出一个fasta序列数据)

|Parameter Name|Remarks|
|--------------|-------|
|strData|The string text value which is in the Fasta format.(FASTA格式的序列文本)|



### Properties

#### _InnerList
方便通过@"M:SMRUCC.genomics.SequenceModel.FASTA.FastaToken.AddAttribute(System.String)"[Add接口]向@"P:SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Attributes"[Attribute列表]中添加数据
#### Attributes
The attribute header of this FASTA file. The fasta header usually have some format which can be parsed by some 
 specific loader and gets some well organized information about the sequence. The format of the header is 
 usually different between each biological database.(这个FASTA文件的属性头，标题的格式通常在不同的数据库之间是具有很大差异的)
#### HaveGaps
Does this sequence contains some gaps?(这条序列之中是否包含有空格？)
#### Length
Get the sequence length of this Fasta object.(获取序列的长度)
#### SequenceData
The sequence data that contains in this FASTA file.
 (包含在这个FASTA文件之中的序列数据)
#### Title
The first character ">" is not included in the title string data.(标题之中是不包含有FASTA数据的第一个>字符的)
