# API
_namespace: [RDotNET.Extensions.Bioinformatics.hierfstat](./index.md)_





### Methods

#### pairwise_fst
```csharp
RDotNET.Extensions.Bioinformatics.hierfstat.API.pairwise_fst(System.String,System.String,System.String)
```
Wrapper for fst estimator from hierfstat package (from adegenet)
 
 ```R
 pairwise.fst(x, pop = NULL, res.type = c("dist", "matrix"))
 ```
 
 The function fstat is a wrapper for varcomp.glob of the package hierfstat. For Fst, Fis and Fit, an alternative is offered by 
 Fst from the pagas package (see example).
 Let A And B be two populations of population sizes n_A And n_B, with expected heterozygosity (averaged over loci) Hs(A) And Hs(B), 
 respectively. We denote Ht the expected heterozygosity of a population pooling A And B. Then, the pairwise Fst between A And B 
 Is computed as
 
 ```
 Fst(A,B) = \frac{(Ht - (n_A Hs(A) + n_B Hs(B))/(n_A + n_B) )}{Ht} 
 ```

|Parameter Name|Remarks|
|--------------|-------|
|x|an object of class genind.(其实在这里是R对象的变量名称)|
|pop|a factor giving the 'population' of each individual. If NULL, pop is seeked from pop(x). Note that the term population refers in fact to any grouping of individuals'.|
|resType|the type of result to be returned: a dist object, or a symmetric matrix|


_returns: A vector, a matrix, or a dist object containing F statistics._


