# QuerySource
_namespace: [SMRUCC.genomics.Assembly.KEGG.WebServices](./index.md)_

Meta data for query KEGG database

> 
>  The example format as:
>  
>  ```
>  Nostoc sp. PCC 7120
>  #
>  alr4156
>  alr4157
>  alr1320
>  all0862
>  all2134
>  all2133
>  ......
>  ```
>  


### Methods

#### QuerySpCode
```csharp
SMRUCC.genomics.Assembly.KEGG.WebServices.QuerySource.QuerySpCode(System.Boolean)
```
Gets the brief code of the organism name in the KEGG database.
 (获取得到KEGG数据库里面的物种的简称)

|Parameter Name|Remarks|
|--------------|-------|
|offline|Work in offline mode?|



### Properties

#### genome
The genome name.
#### locusId
The list of gene locus id that using for the query.
