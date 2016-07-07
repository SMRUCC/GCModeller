---
title: TrackData
---

# TrackData
_namespace: [SMRUCC.genomics.Visualize.Circos.TrackDatas](N-SMRUCC.genomics.Visualize.Circos.TrackDatas.html)_

Data for tracks is loaded from a plain-text file. Each data point is stored on a 
 separate line, except for links which use two lines per link.
 
 The definition Of a data point within a track Is based On the genomic range, 
 which Is a combination Of chromosome And start/End position.

> 
>  Data for tracks is loaded from a plain-text file. Each data point is stored on a separate line, except for links which use two lines per link.
>  The definition Of a data point within a track Is based On the genomic range, which Is a combination Of chromosome And start/End position. 
>  For example,
>  
>  ```
>  # the basis for a data point Is a range
>  chr12 1000 5000
>  ```
>  
>  All data values, regardless Of track type, will be positioned Using a range rather than a Single position. To explicitly specify a Single position, 
>  use a range With equal start And End positions.
>  
>  + Tracks such As scatter plot, line plot, histogram Or heat map, associate a value With Each range. The input To this kind Of track would be
>  
>  ```
>  # scatter, line, histogram And heat maps require a value
>  chr12 1000 5000 0.25
>  ```
>  
>  + The exception Is a stacked histogram, which associates a list Of values With a range.
>  
>  ```
>  # stacked histograms take a list of values
>  chr12 1000 5000 0.25,0.35,0.60
>  ```
>  
>  + The value For a text track Is interpreted As a text label (other tracks require that this field be a floating point number).
>  
>  ```
>  # value for text tracks Is interpreted as text
>  chr12 1000 5000 geneA
>  ```
>  
>  + The tile track does Not take a value—only a range.
>  
>  ```
>  chr12 1000 5000
>  ```
>  
>  + Finally, links are a special track type which associates two ranges together. The format For links Is analogous To other data types, 
>  except now two coordinates are specified.
>  
>  ```
>  chr12 1000 5000 chr15 5000 7000
>  ```
>  
>  + In addition to the chromosome, range And (if applicable) value, each data point can be annotated with formatting parameters that control how the point Is drawn. 
>  The parameters need to be compatible with the track type for which the file Is destined. Thus, a scatter plot data point might have
>  
>  ```
>  chr12 1000 5000 0.25 glyph_size=10p,glyph=circle
>  ```
>  
>  whereas a histogram data point might include the Option To fill the data value's bin
>  
>  ```
>  chr12 1000 5000 0.25 fill_color=orange
>  ```
>  
>  + Other features, such As URLs, can be associated With any data point. For URLs the parameter can contain parsable fields (e.g. [start]) which 
>  are populated automatically With the data point's associated property.
>  
>  ```
>  # the URL for this point would be
>  # http://domain.com/script?start=1000&end=5000&chr=chr12
>  chr12 1000 5000 0.25 url=http//domain.com/script?start=[start]&end=[end]&chr=[chr]
>  ```
>  


### Methods

#### ToString
```csharp
SMRUCC.genomics.Visualize.Circos.TrackDatas.TrackData.ToString
```
Using @"M:SMRUCC.genomics.Visualize.Circos.TrackDatas.TrackData.ToString" method for creates tracks data document.


### Properties

#### chr
Chromosomes name
