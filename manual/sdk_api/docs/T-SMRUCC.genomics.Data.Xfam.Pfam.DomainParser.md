---
title: DomainParser
---

# DomainParser
_namespace: [SMRUCC.genomics.Data.Xfam.Pfam](N-SMRUCC.genomics.Data.Xfam.Pfam.html)_





### Methods

#### __domainFilter
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.DomainParser.__domainFilter(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.SubjectHit,System.Double,System.Double,System.Double)
```
Domain只是比对上query序列的一部分，所以不应该要求要覆盖全长，而是能够将hit本身给覆盖住就可以了
 由于只是比对上一部分，所以identities不可能会很高，一般是在20%到60%之间，
 也就是说identities可能在这里并不能作为一个判断的标准了，因为query序列越长，则identities则可能越低，但是那个domain还是可能能够真实存在的

 一般而言，gaps和evalue是成正比的，gaps值越大，则evalue越大，所以这里使用evalue就可以筛掉了

|Parameter Name|Remarks|
|--------------|-------|
|hit|-|
|evalue|-|
|coverage|-|
|identities|暂时无用|


#### __groupAndTrimOverlap
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.DomainParser.__groupAndTrimOverlap(System.Collections.Generic.IEnumerable{SMRUCC.genomics.ProteinModel.DomainModel},System.Int32)
```
分组并去掉重叠的数据，这个主要是针对Pfam-A的比对数据，对于CDD数据库可能没有什么用途

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|lenOffset|-|


#### __trimOverlaps
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.DomainParser.__trimOverlaps(SMRUCC.genomics.ProteinModel.DomainModel[],System.Int32)
```
按照长度值将重叠的结构域清除掉，只留下大的结构域，因为在这之前都是经过阈值筛选了的，所以都是满足条件了的，
 这里只选择比较大的结构域，但是这样子会有什么问题么？有重叠的时候在KEGG上面是首先显示出比较大的结构域的

|Parameter Name|Remarks|
|--------------|-------|
|domains|-|


#### Parser
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.DomainParser.Parser(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query,System.Double,System.Double,System.Double,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|queryIteration|-|
|evalue|-|
|coverage|-|
|identities|暂时无用|
|offset|-|



