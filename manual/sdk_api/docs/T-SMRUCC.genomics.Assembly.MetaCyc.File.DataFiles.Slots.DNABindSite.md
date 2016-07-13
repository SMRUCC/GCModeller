---
title: DNABindSite
---

# DNABindSite
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_

The class describes DNA regions that are binding sites for transcription factors.
 (本对象描述了一个能够与转录因子相结合的DNA片段)




### Properties

#### AbsCenterPos
This slot defines the position on the replicon of the center of this binding site.
#### InvolvedInRegulation
This slot links the binding site to a Regulation frame describing the regulatory 
 interaction in which this binding site participates.
#### SiteLength
This slot defines the extent of a binding site in base pairs. If a value for this 
 slot is omitted, the site length will be computed based on the DNA-Footprint-Size 
 of the binding protein. Thus, a value for this slot should only be supplied here 
 if the site length for a particular transcription factor is not consistent across 
 all its sites.
