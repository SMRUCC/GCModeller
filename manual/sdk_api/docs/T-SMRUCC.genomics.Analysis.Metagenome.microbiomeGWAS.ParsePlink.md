---
title: ParsePlink
---

# ParsePlink
_namespace: [SMRUCC.genomics.Analysis.Metagenome.microbiomeGWAS](N-SMRUCC.genomics.Analysis.Metagenome.microbiomeGWAS.html)_





### Methods

#### parsePlink
```csharp
SMRUCC.genomics.Analysis.Metagenome.microbiomeGWAS.ParsePlink.parsePlink(System.String,System.Int32@,System.Int32@,System.Double[],System.Int32[],System.Double[])
```


|Parameter Name|Remarks|
|--------------|-------|
|plinkBed|plinkBed, the file name of plink bed file|
|NumSample|NumSample, Number of Samples in Plink|
|NumSNP|NumSNP, Number of SNPs in plink|
|distMat|distMat, distance matrix NumSample * NumSample|
|E|E, environment vector with length = NumSample|
|result|result, 12 * NSNP, including
 
 ```
 1:5		#0, #1, #2, #NA MAF of G
 6:10	#0, #1, #2, #NA MAF of GE
 11		SM
 12		SI
 ```
 |



