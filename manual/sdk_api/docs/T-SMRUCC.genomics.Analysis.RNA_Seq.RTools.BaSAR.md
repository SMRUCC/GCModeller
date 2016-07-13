---
title: BaSAR
---

# BaSAR
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.RTools](N-SMRUCC.genomics.Analysis.RNA_Seq.RTools.html)_

Package: BaSAR
 Type: Package
 Title: Bayesian Spectrum Analysis in R
 Version: 1.3
 Date: 2012-05-08
 Author: Emma Granqvist, Matthew Hartley and Richard J Morris
 Maintainer: Emma Granqvist <emma.granqvist@jic.ac.uk>
 Description: Bayesian Spectrum Analysis of time series data
 Depends: polynom, orthopolynom
 Suggests: fields
 License: GPL-2
 LazyLoad: yes
 Packaged: 2012-05-08 16:47:24 UTC; granqvie
 Repository: CRAN
 Date/Publication: 2012-05-08 19:23:08

> 
>  citHeader("To cite package 'BaSAR' in publications use:")
>  
>  ## R >= 2.8.0 passes package metadata to citation().
>  if(!exists("meta") || is.null(meta)) meta <- packageDescription("BaSAR")
>  year <- sub("-.*", "", meta$Date)
>  note <- sprintf("R package version %s.", meta$Version)
>  
>  citEntry(entry = "Manual",
>  	 title = {
>               paste("BaSAR: Bayesian Spectrum Analysis of time series data")
>           },
>  	 author = personList(
>             person("Emma", "Granqvist",
>                    email = "emma.granqvist@jic.ac.uk"),
>             person("Matthew", "Hartley",
>                    email = "matthew.hartley@jic.ac.uk"),
>             person("Richard", "Morris",
>                    email = "richard.morris@jic.ac.uk")),
>           year = 2011,
>  	note  = "version 1.1",
>  	 url = "http://CRAN.R-project.org/package=BaSAR",
>  	 textVersion = {
>               paste("Emma Granqvist and Matthew Hartley and Richard J Morris",
>  	           sprintf("(%s).", year),
>                     "BaSAR: Bayesian Spectrum Analysis of time series data",
>                     note)
>           })
>  
>  


### Methods

#### auto
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.BaSAR.auto
```
Automated BaSAR.modelratio

#### Initialize
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.BaSAR.Initialize(System.String)
```
Brief Usage Introduction:
 
 Key functions
 BaSAR.post returns a normalised posterior probability distribution over the chosen range of frequency (ω). 
 This is invoked in the manner:
 
 BaSAR.post(data, start, stop, nsamples, nbackg, tpoints) where data is the time series as a 1D vector, 
 startstop is the range of the period that is of interest (in seconds), nsamples is the number of samples 
 that will be calculated from the posterior, and tpoints is the vector of time points when the data were 
 sampled (in seconds). The interval between the time points does not need to be uniform. 
 
 BaSAR.nest calculates the evidence using nested sampling. Direct comparison of evidences can be used to 
 evaluate models.
 
 BaSAR.modelratio is a model comparison method that uses model ratios to allow the user to compare two 
 models with different background functions. This procedure has been automated in BaSAR.auto. 
 
 For time series in which the dominant frequency changes over time, BaSAR.local can be used to calculate the 
 local frequency by windowing.
 
 The outputs from all functions are the posterior probability distribution over ω. If the user wants to see 
 the results over period instead, there is a helper-function for this called BaSAR.plotperiod.
 
 Table. Key functions in the BaSAR package. 
 
 Function Description
 --------------------------------------------------------------------
 BaSAR.post Normalized posterior probability distribution
 BaSAR.nest Posterior and evidence using nested sampling
 BaSAR.modelratio Model comparison for background trends
 BaSAR.auto Automated BaSAR.modelratio
 BaSAR.local 2D posterior over time and ω by windowing

#### Local
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.BaSAR.Local(System.Double[],System.Int32,System.Int32,System.Int32,System.Int32[],System.Int32,System.Int32)
```
2D posterior over time and ω by windowing, A windowed BSA that computes the frequency locally.
 
 BaSAR.local uses BaSAR.post with windowing, so it computes a local posterior. The window works 
 in the way that at each time point i, the posterior will be calculated using the data from 
 i-window to i+window.

|Parameter Name|Remarks|
|--------------|-------|
|data|data as a 1-dimensional vector|
|start|lower limit of period of interest, in seconds|
|stop|upper limit of period of interest, in seconds|
|nsamples|number of samples within the interval start-stop|
|tpoints|vector of time points, in seconds|
|nbackg|number of background functions to be added to the model|
|window|length of window, in number of data points|

_returns: 
 A list containing:
 
 {omega} {1D vector of the omega sampled}
 {p}     {2D posterior distribution over omega and time}
 _
> 
>  Examples
>  
>  
>  require(fields)
>  # Create time series with changing omega
>  tpoints = seq(from=1, to=200, length=200)
>  dpoints <- c()
>  
>  for (i in 1:200) { dpoints[i] <- sin((0.5+i*0.005)*i) }
>  # Plot time series
>  plot(dpoints, type="l", col="blue", xlab="t", ylab="d(t)")
>  # Run BaSAR with windowing to get 2D posterior over omega and time
>  r <- BaSAR.local(dpoints, 2, 30, 100, tpoints, 0, 10)
>  # Plot the resulting 2D posterior density function
>  # with time on x-axis and omega on y-axis
>  require(fields)
>  image.plot(tpoints,r$omega,r$p, col=rev(heat.colors(100)),
>  ylab=expression(omega),xlab="t")
>  
>  

#### modelratio
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.BaSAR.modelratio
```
Model comparison for background trends

#### nest
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.BaSAR.nest
```
Posterior and evidence using nested sampling

#### post
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.BaSAR.post
```
Normalized posterior probability distribution


