# QueryEngine
_namespace: [SMRUCC.genomics.Data.Repository.NCBI](./index.md)_





### Methods

#### #ctor
```csharp
SMRUCC.genomics.Data.Repository.NCBI.QueryEngine.#ctor(Oracle.LinuxCompatibility.MySQL.ConnectionUri)
```
创建以及测试数据库连接

#### ScanDatabase
```csharp
SMRUCC.genomics.Data.Repository.NCBI.QueryEngine.ScanDatabase(System.String,Microsoft.VisualBasic.Dictionary{Microsoft.VisualBasic.ComponentModel.DataSourceModel.NamedValue{Microsoft.VisualBasic.Data.IO.SearchEngine.Expression}},System.String,System.Int32)
```
Scaner for full NT database that can running on low memory machine.

|Parameter Name|Remarks|
|--------------|-------|
|DATA$|-|
|query|-|
|EXPORT$|-|
|lineBreak%|-|


#### Search
```csharp
SMRUCC.genomics.Data.Repository.NCBI.QueryEngine.Search(System.String)
```
请参考搜索引擎的语法，假若查询里面含有符号的话，会被当作分隔符来看待，所以假若符号也要被匹配出来的话，需要添加双引号

|Parameter Name|Remarks|
|--------------|-------|
|query$|-|



