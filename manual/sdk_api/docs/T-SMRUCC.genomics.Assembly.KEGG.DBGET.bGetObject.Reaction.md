---
title: Reaction
---

# Reaction
_namespace: [SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject](N-SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.html)_

KEGG reaction annotation data.



### Methods

#### __innerOrthParser
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction.__innerOrthParser(System.String)
```
K01509</a> adenosinetriphosphatase [EC:<a href="/dbget-bin/www_bget?ec:3.6.1.3">3.6.1.3</a>]

|Parameter Name|Remarks|
|--------------|-------|
|s|-|


#### FetchTo
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction.FetchTo(System.String[],System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|lstId|-|
|outDIR|-|

_returns: 返回成功下载的对象的数目_

#### GetSubstrateCompounds
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction.GetSubstrateCompounds
```
得到本反应过程对象中的所有的代谢底物的KEGG编号，以便于查询和下载


### Properties

#### ECNum
标号： @"P:SMRUCC.genomics.Assembly.Expasy.Database.Enzyme.Identification"
