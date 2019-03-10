# genind
_namespace: [RDotNET.Extensions.Bioinformatics.adegenet](./index.md)_

###### adegenet formal class (S4) for individual genotypes
 
 The S4 class genind is used to store individual genotypes.
 It contains several components described In the 'slots' section).
 The summary Of a genind Object invisibly returns a list Of component. 
 The Function .valid.genind Is For internal use. The Function genind 
 creates a genind Object from a valid table Of alleles corresponding 
 To the @tab slot. Note that As In other S4 classes, slots are accessed 
 Using @ instead Of \$.



### Methods

#### nancycats
```csharp
RDotNET.Extensions.Bioinformatics.adegenet.genind.nancycats
```
###### Microsatellites genotypes of 237 cats from 17 colonies of Nancy (France)
 
 This data set gives the genotypes of 237 cats (Felis catus L.) for 9 microsatellites markers. 
 The individuals are divided into 17 colonies whose spatial coordinates are also provided.
> 
>  nancycats is a genind object with spatial coordinates of the colonies as a supplementary components (@xy).
>  
>  Dominique Pontier (UMR CNRS 5558, University Lyon1, France)
>  
>  > Devillard, S.; Jombart, T. & Pontier, D. Disentangling spatial and genetic structure of stray cat (Felis catus L.) colonies in urban habitat using: not all colonies are equal. submitted to Molecular Ecology
>  


### Properties

#### allNames
list having one component per locus, each containing a character vector of alleles names
#### call
the matched call
#### hierarchy
(optional) a hierarchical formula defining the hierarchical levels in the @@strata slot.
#### locFac
locus factor for the columns of tab
#### locNAll
integer vector giving the number of alleles per locus
#### other
(optional) a list containing other information
#### ploidy
an integer indicating the degree of ploidy of the genotypes. Beware: 2 is not an integer, but as.integer(2) is.
#### pop
(optional) factor giving the population of each individual
#### strata
(optional) data frame giving levels of population stratification for each individual
#### tab
matrix integers containing genotypes data for individuals (in rows) for 
 all alleles (in columns). 
 The table differs depending on the @type slot:
 
 + ``codom``: values are numbers of alleles, summing up to the individuals' ploidies.
 + ``PA``: values are presence/absence of alleles.
 
 In all cases, rows And columns are given generic names.
#### type
a character string indicating the type of marker: ``codom`` stands for ``codominant`` 
 (e.g. microstallites, allozymes); ``PA`` stands for ``presence/absence`` (e.g. AFLP).
