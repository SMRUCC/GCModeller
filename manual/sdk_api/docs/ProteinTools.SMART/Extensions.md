# Extensions
_namespace: [ProteinTools.SMART](./index.md)_





### Methods

#### ContainsAny
```csharp
ProteinTools.SMART.Extensions.ContainsAny(System.String,System.String[],Microsoft.VisualBasic.CompareMethod)
```
只要目标字符串之中包含列表中的任意一个元素就返回真

|Parameter Name|Remarks|
|--------------|-------|
|Text|-|
|Keywords|-|
|SenseCase|-|


#### ContainsKeyword
```csharp
ProteinTools.SMART.Extensions.ContainsKeyword(System.String,System.String[],Microsoft.VisualBasic.CompareMethod)
```
必须所有的元素都包含在内才返回真

|Parameter Name|Remarks|
|--------------|-------|
|Text|-|
|Keywords|-|
|SenseCase|-|


#### GetAllHits
```csharp
ProteinTools.SMART.Extensions.GetAllHits(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Legacy.BLASTOutput)
```
获取所有Hit对象的标识号

#### Install
```csharp
ProteinTools.SMART.Extensions.Install(SMRUCC.genomics.Assembly.NCBI.CDD.Database,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InitializeParameter)
```


_returns: 返回安装成功的数据库的数目_


