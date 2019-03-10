# API
_namespace: [RDotNET.Extensions.Bioinformatics.adegenet](./index.md)_





### Methods

#### df2genind
```csharp
RDotNET.Extensions.Bioinformatics.adegenet.API.df2genind(Microsoft.VisualBasic.Data.csv.DocumentStream.File,System.String,System.String,System.String[],System.String[],System.String,System.String,System.Int32,System.String,System.String,System.String)
```
Convert a ``data.frame`` of allele data to a @``T:RDotNET.Extensions.Bioinformatics.adegenet.genind`` object.
 
 The function ``df2genind`` converts a ``data.frame`` (or a matrix) into a ``genind`` object. The ``data.frame`` must meet the following requirements:
 
 - genotypes are in row (one row per genotype)
 - markers/loci are in columns
 - each element Is a string of characters coding alleles, ideally separated by a character string (argument sep); if no separator Is used, the number of characters coding alleles must be indicated (argument ncode).
 
 (这个函数在这里所返回来的是@``T:RDotNET.Extensions.Bioinformatics.adegenet.genind``对象在R之中的变量的名称)

|Parameter Name|Remarks|
|--------------|-------|
|X|a matrix or a data.frame containing allelle data only (see decription)|
|sep|a character string separating alleles. See details.|
|ncode|an optional integer giving the number of characters used for coding one genotype at one locus. If not provided, this is determined from data.|
|indNames|an optional character vector giving the individuals names; if NULL, taken from rownames of X.|
|locNames|an optional character vector giving the markers names; if NULL, taken from colnames of X.|
|pop|an optional factor giving the population of each individual.|
|NAchar|a vector of character strings which are to be treated as NA|
|ploidy|an integer indicating the degree of ploidy of the genotypes.|
|type|a character string indicating the type of marker: 'codom' stands for 'codominant' (e.g. microstallites, allozymes); 'PA' stands for 'presence/absence' markers (e.g. AFLP, RAPD).|
|strata|an optional data frame that defines population stratifications for your samples. This is especially useful if you have a hierarchical or factorial sampling design.|
|hierarchy|a hierarchical formula that explicitely defines hierarchical levels in your strata. see hierarchy for details.|


_returns: an object of the class genind for df2genind; a matrix of biallelic genotypes for genind2df_
> 
>  See genind2df to convert genind objects back to such a data.frame.
> 
>  === Details for the sep argument ===
>  this character Is directly used In reguar expressions Like gsub, And thus require some characters To be 
>  preceeded by Double backslashes. For instance, "/" works but "|" must be coded As "\|".
>  
>  #### Examples
> 
>  ```R
>  ## simple example
>  df <- data.frame(locusA=c("11","11","12","32"),
>  locusB=c(NA,"34","55","15"),locusC=c("22","22","21","22"))
>  row.names(df) <- .genlab("genotype",4)
>  df
> 
>  obj <- df2genind(df, ploidy=2, ncode=1)
>  obj
>  obj@tab
> 
>  ## converting a genind as data.frame
>  genind2df(obj)
>  genind2df(obj, sep="/")
>  ```
>  


