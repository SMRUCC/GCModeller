# Blastn MapPlot using GCModeller

### CommandLine Help

Typing this command in your commandline console, and then the GCModeller tool will output such commandline help information:

```python
mapplot ? /Visualize.blastn.alignment

#  Help for command '/Visualize.blastn.alignment':
#
#  Information:  Blastn result alignment visualization from the NCBI web blast. This tools is only works for
#                a plasmid blastn search result or a small gene cluster region in a large genome.
#  Usage:        MapPlot /Visualize.blastn.alignment /in <alignmentTable.txt> /genbank <genome.gb> [/ORF.catagory <catagory.tsv> /local /out <image.png>]
#  Example:      CLI usage example not found!
#
#  Arguments:
#  ============================
#
#   /genbank         Description:  Provides the target genome coordinates for the blastn map plots.
#
#                    Example:      /genbank <term_string>
#
#   /local           Description:  The file for ``/in`` parameter is a local blastn output result
#                                  file?
#
#                    Example:      /local <term_string>
#
#   /ORF.catagory    Description:  Using for the ORF shape color render, in a text file and each
#                                  line its text format like: ``geneID``<TAB>``COG/KOG/GO/KO``
#
#                    Example:      /ORF.catagory <term_string>
```

This map plot commandline have 2 required parameters:

+ ``/in`` parameter for input the blastn alignment result, it can be a alignmentTable file from the NCBI web blast output, or a local blastn output, its meaning based on the optional parameter ``/local`` is presented or not.
+ ``/genbank`` parameter provides the genome coordinate information for the alignment hits and gene map plot.

And it also can take 3 optional parameter:

+ ``/ORF.catagory`` for the color profiling of the gene map plot, its format is in a keyvalue pair form like
  ```
  geneID<TAB><categoryID>
  geneID<TAB><categoryID>
  geneID<TAB><categoryID>
  geneID<TAB><categoryID>
  ...
  ```
  Where the geneID is the ``locus_tag`` value in the gene section in your genbank file, example like:
  ```
  gene            2709..3347
                  /gene="parA"
                  /locus_tag="XOCp0004"
  ```
  corresponding geneID value in this category file is:
  ```
  XOCp0004	D	COG1192
  ```
  **If this parameter is ignored not presented in the commandline, then all of your gene objects in the gene map plot will be white color filled.**
+ ``/out`` is the output image file path.
+ ``/local``, as we mentioned earlier, this parameter defined the input data type of the parameter ``/in``.