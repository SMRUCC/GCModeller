---
title: ModelAPI
---

# ModelAPI
_namespace: [SMRUCC.genomics.Visualize.ComparativeGenomics](N-SMRUCC.genomics.Visualize.ComparativeGenomics.html)_





### Methods

#### __COGsColor``1
```csharp
SMRUCC.genomics.Visualize.ComparativeGenomics.ModelAPI.__COGsColor``1(System.Collections.Generic.IEnumerable{``0},System.Collections.Generic.Dictionary{System.String,System.Drawing.Brush}@)
```
假若**COGsColor**参数为空的话，就会根据PTT里面的注释生成颜色谱

|Parameter Name|Remarks|
|--------------|-------|
|anno|-|
|COGsColor|-|


#### CreateObject
```csharp
SMRUCC.genomics.Visualize.ComparativeGenomics.ModelAPI.CreateObject(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief[],System.Int32,System.String,SMRUCC.genomics.Visualize.ComparativeGenomics.ModelAPI.GetDrawingID,System.Boolean,SMRUCC.genomics.Visualize.ComparativeGenomics.ModelAPI.ICOGsBrush)
```


|Parameter Name|Remarks|
|--------------|-------|
|PTT|-|
|len|-|
|title|-|
|COGsColor|-|
|__getId|Public Delegate Function GetDrawingID(Gene As @"T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief") As @"T:System.String"|



