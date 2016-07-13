---
title: OverwritesColors
---

# OverwritesColors
_namespace: [SMRUCC.genomics.Visualize.Circos.Configurations](N-SMRUCC.genomics.Visualize.Circos.Configurations.html)_

Use ``@"P:SMRUCC.genomics.Visualize.Circos.Configurations.Circos.chromosomes_color"`` to change
 the color Of the ideograms. This approach works well When the only
 thing you want To Do Is change the color Of the segments. 

 Another way To achieve this Is To actually redefine the colors which
 are used To color the ideograms. The benefit Of doing this Is that
 whenever you refer To the color (which you can use by Using the name
 Of the chromosome), you Get the custom value.

 If you Then look In the human karyotype file linked To above, you'll see
 that Each chromosome's color is ``chrN`` where N is the number of the
 chromosome. Thus, hs1 has color chr1, hs2 has color chr2, And so
 On. For convenience, a color can be referenced Using 'chr' and 'hs'
 prefixes (chr1 And hs1 are the same color).

 Colors are redefined by overwriting color definitions, which are
 found In the ``<colors>`` block. This block Is included below from the
 colors_fonts_patterns.conf file, which contains all the Default
 definitions. To overwrite colors, use a "*" suffix And provide a New
 value, which can be a lookup To another color.




