---
title: ValueTrackData
---

# ValueTrackData
_namespace: [SMRUCC.genomics.Visualize.Circos.TrackDatas](N-SMRUCC.genomics.Visualize.Circos.TrackDatas.html)_

Tracks such As scatter plot, line plot, histogram Or heat map, associate a value With Each range. The input To this kind Of track would be
 
 ```
 # scatter, line, histogram And heat maps require a value
 chr12 1000 5000 0.25
 ```

> 
>  In addition to the chromosome, range And (if applicable) value, each data point can be annotated with formatting parameters that control how the point Is drawn. 
>  The parameters need to be compatible with the track type for which the file Is destined. Thus, a scatter plot data point might have
>  
>  ```
>  chr12 1000 5000 0.25 glyph_size=10p,glyph=circle
>  ```
>  



