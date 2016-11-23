# API
_namespace: [RDotNET.Extensions.Bioinformatics.sparcc](./index.md)_

R package computes correlation for relative abundances



### Methods

#### #cctor
```csharp
RDotNET.Extensions.Bioinformatics.sparcc.API.#cctor
```
装载计算脚本

#### sparcc
```csharp
RDotNET.Extensions.Bioinformatics.sparcc.API.sparcc(System.String,System.Int32,System.Double,System.Double)
```
count matrix x should be samples on the rows and OTUs on the colums,
 
 ```R
 assuming dim(x) -> samples by OTUs
 ```

|Parameter Name|Remarks|
|--------------|-------|
|x|The data file path|
|maxIter|-|
|th|-|
|exiter|-|



