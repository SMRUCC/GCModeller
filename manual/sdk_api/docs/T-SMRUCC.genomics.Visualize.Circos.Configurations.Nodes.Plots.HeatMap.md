---
title: HeatMap
---

# HeatMap
_namespace: [SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots](N-SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.html)_

Heat maps are used for data types which associate a value with a genomic position, Or region. 
 As such, this track uses the same data format As histograms.

 The track linearly maps a range Of values [min,max] onto a list Of colors c[n], i=0..N.

 f = (value - min) / ( max - min )
 n = N * f




### Properties

#### color
Colors are defined by a combination of lists or CSV. Color lists
 exist For all Brewer palletes (see etc/colors.brewer.lists.conf) As
 well As For N-Step hue (hue-sN, e.g. hue-s5 =
 hue000,hue005,hue010,...) And N-color hue (hue-sN, e.g. hue-3 =
 hue000,hue120,hue140).
#### scale_log_base
If scale_log_base is used, the mapping is not linear, but a power law 

 n = N * f**(1/scale_log_base)

 When scale_log_base > 1 the dynamic range For values close To min Is expanded. 
 When scale_log_base < 1 the dynamic range For values close To max Is expanded.
