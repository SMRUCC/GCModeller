---
title: WebForm
---

# WebForm
_namespace: [SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers](N-SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers.html)_

KEGG 网页表格的数据解析方法，在Value之中可能会有重复的Key数据出现



### Methods

#### GetValue
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers.WebForm.GetValue(System.String)
```
获取某一个字段的数据

|Parameter Name|Remarks|
|--------------|-------|
|KeyWord|网页的表格之中的最左端的字段名|


#### RegexReplace
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers.WebForm.RegexReplace(System.String,System.String[])
```
将符合目标规则的字符串替换为空字符串

|Parameter Name|Remarks|
|--------------|-------|
|strData|-|
|ExprCollection|-|



### Properties

#### _strData
Entry, {trim_formatted, non-process}
