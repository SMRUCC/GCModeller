## Install &amp; Configs

If you are running GCModeller on a Linux/macOS platform, please install the latest mono environment first, and then run script **[install.sh](./install.sh)** for configuring the runtime.

## Using GCModeller CLI tools

For start using GCModeller tools, typing 
```bash
GCModeller --ls

# All of the available GCModeller commands were listed below.
#
# For getting the available function in the GCModeller program, 
# try typing:    <command> ?
# For getting the manual document in the GCModeller program,
# try typing:    <command> man
#
#
# Listed 25 available GCModeller commands:
#
#  CARMEN               
#  Circos               Tools for generates the circos drawing model file for the circos
#                       perl script.
#  Cytoscape            Cytoscape model generator and visualization tools utils for GCModeller
#                      
#
#  FBA                  
#  GCModeller           
#  KEGG_tools           KEGG web services API tools.
#  MEME                 A wrapper tools for the NCBR meme tools, this is a powerfull
#                       tools for reconstruct the regulation in the bacterial genome.
#                      
#
#  NCBI_tools           Tools collection for handling NCBI data, includes: nt/nr database,
#                       NCBI taxonomy analysis, OTU taxonomy analysis, genbank database,
#                       and sequence query tools.
#  PLAS                 
#  PhenoTree            Cellular phenotype analysis tools.
#  RNA-seq              
#  RegPrecise           
#  SMART                SMART protein domain structure tools CLI interface.
#  Settings             GCModeller configuration console.
#  Shoal                This module define the shoal commandlines for the command line
#                       interpreter.
#  Spiderman            Tools for analysis the biological network.
#  VirtualFootprint     
#  Xfam                 Xfam Tools (Pfam, Rfam, iPfam)
#  gcc                  gcc=GCModeller Compiler; Compiler program for the GCModeller
#                       virtual cell system model
#  localblast           Wrapper tools for the ncbi blast+ program and the blast output
#                       data analysis program.
#  mpl                  
#  pitr                 Tools for analysis the protein interaction relationship.
#  seqtools             Sequence operation utilities
#  vcsm                 
#  venn                 Tools for creating venn diagram model for the R program and venn
#                       diagram visualize drawing.
``` 
for listing all of the avaliable **GCModeller** tools, and then typing 
```
<tool_name> ??
``` 
for listing all of the avaiable CLI API in the specific tool, and by typing 
```
<tool_name> ??[API_name]
``` 
for details help information of the API command.

###### Example for getting helps

```
$ NCBI_tools ??
```

![](./images/ToolsHelp-Example.png)
