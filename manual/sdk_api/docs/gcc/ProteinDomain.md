# ProteinDomain
_namespace: [gcc](./index.md)_

利用Smart程序分析目标蛋白质结构域，方便进行信号传导网络的重建工作



### Methods

#### GetList
```csharp
gcc.ProteinDomain.GetList(gcc.ProteinDomain.Rule[],Microsoft.VisualBasic.Data.csv.DocumentStream.File)
```


_returns: {DomainId, Protein-UniqueId()}_

#### SMART
```csharp
gcc.ProteinDomain.SMART(System.String,System.String,System.String)
```
以Pfam数据库为准

|Parameter Name|Remarks|
|--------------|-------|
|TargetFile|目标蛋白质序列数据库|
|ExportSaved|保存的结果数据CSV文件的文件路径|



