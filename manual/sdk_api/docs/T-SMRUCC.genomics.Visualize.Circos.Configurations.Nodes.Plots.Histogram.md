---
title: Histogram
---

# Histogram
_namespace: [SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots](N-SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.html)_

Histograms are a type Of track that displays 2D data, which
 associates a value With a genomic position. Line plots, scatter
 plots And heat maps are examples Of other 2D tracks.

 The data format For 2D data Is 

 #chr start End value [options]
 ...
 hs3 196000000 197999999 71.0000
 hs3 198000000 199999999 57.0000
 hs4 0 1999999 28.0000
 hs4 2000000 3999999 40.0000
 hs4 4000000 5999999 59.0000
 ...

 Each histogram Is defined In a ``<plot>`` block within an enclosing ``<plots`` block.

> 
>  Like For links, rules are used To dynamically alter formatting Of
>  Each data point (i.e. histogram bin). Here, I include the ```<rule>```
>  block from a file, which contains the following
>  



### Properties

#### extend_bin
Do Not join histogram bins that Do Not abut.
#### fill_color
Histograms can have both a fill And outline. The Default outline Is 1px thick black.
