---
title: DbFileSystemObject
---

# DbFileSystemObject
_namespace: [SMRUCC.genomics.Data.Model_Repository](N-SMRUCC.genomics.Data.Model_Repository.html)_

The object which its entry information was stores in the repository database but data stores on the filesystem.



### Methods

#### GetPath
```csharp
SMRUCC.genomics.Data.Model_Repository.DbFileSystemObject.GetPath(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|RepositoryRoot|数据源的根目录|


#### VerifyData
```csharp
SMRUCC.genomics.Data.Model_Repository.DbFileSystemObject.VerifyData(System.String,System.String)
```
本对象所指向的文件不存在或者哈希值比对不上，都会返回False


