---
title: Pathway
---

# Pathway
_namespace: [SMRUCC.genomics.Assembly.KEGG.Archives.Csv](N-SMRUCC.genomics.Assembly.KEGG.Archives.Csv.html)_

CSV data model for storage the kegg pathway brief information.(用于向Csv文件保存数据的简单格式的代谢途径数据存储对象)



### Methods

#### LoadData
```csharp
SMRUCC.genomics.Assembly.KEGG.Archives.Csv.Pathway.LoadData(System.String,System.String)
```
将所下载的代谢途径数据转换为CSV文档保存

|Parameter Name|Remarks|
|--------------|-------|
|DIR|-|
|spCode|物种名称的三字母简写，例如xcb|



### Properties

#### Category
Phenotype Category
#### Class
Phenotype Class
#### EntryId
Pathway object KEGG database entry id.
#### PathwayGenes
The enzyme gene which was involved in this pathway catalysts.
