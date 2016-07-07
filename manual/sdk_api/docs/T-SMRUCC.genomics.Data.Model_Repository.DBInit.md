---
title: DBInit
---

# DBInit
_namespace: [SMRUCC.genomics.Data.Model_Repository](N-SMRUCC.genomics.Data.Model_Repository.html)_

这个模块之中的命令当且仅当第一次创建数据库的时候使用，假若在已经创建了数据库的情况之下使用，则会重置整个数据库



### Methods

#### UpdateGenbankEntryInfo
```csharp
SMRUCC.genomics.Data.Model_Repository.DBInit.UpdateGenbankEntryInfo(System.String,SMRUCC.genomics.Data.Model_Repository.SQLEngines.SQLiteIndex)
```
由于文件已经存放于数据库的文件夹之中，故而不会再进行文件的复制操作，函数只需要生成数据库数据就可以了

|Parameter Name|Remarks|
|--------------|-------|
|dir|-|



