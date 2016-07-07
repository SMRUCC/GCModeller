---
title: Ideogram
---

# Ideogram
_namespace: [SMRUCC.genomics.Visualize.Circos.Configurations.Nodes](N-SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.html)_






### Properties

#### band_stroke_thickness
# cytogenetic bands
#### fill
Ideograms can be drawn as filled, outlined, or both. When filled,
 the color will be taken from the last field In the karyotype file,
 Or Set by chromosomes_colors. Color names are discussed In

 http://www.circos.ca/documentation/tutorials/configuration/configuration_files

 When ``stroke_thickness=0p`` Or If the parameter Is missing, the ideogram Is
 has no outline And the value Of stroke_color Is Not used.
#### fill_bands
# in order to fill the bands with the color defined in the karyotype
 # file you must set fill_bands
#### fill_color
# the default chromosome color is set here and any value
 # defined in the karyotype file overrides it
#### label_font
see ``etc/fonts.conf`` for list of font names
#### label_radius
if ideogram radius is constant, and you'd like labels close to image edge, 
 use the ``dims()`` Function To access the size Of the image
 
 ``
 label_radius = dims(image,radius) - 60p
 ``
#### radius
Fractional radius position of chromosome ideogram within image.
 
 Spacing between ideograms. Suffix "r" denotes a relative value. It
 Is relative To circle circumference (e.g. space Is 0.5% Of
 circumference).
#### show_bands
# show_bands determines whether the outline of cytogenetic bands
 # will be seen
#### stroke_color
# ideogram border color
#### thickness
thickness (px) of chromosome ideogram
 
 Thickness of ideograms, which can be absolute (e.g. pixels, "p"
 suffix) Or relative ("r" suffix). When relative, it Is a fraction Of
 image radius.
