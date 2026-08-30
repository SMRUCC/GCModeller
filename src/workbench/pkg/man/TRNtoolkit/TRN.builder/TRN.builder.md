# TRN.builder

tools for create a transcription regulation network
> This R# package module provides the toolkit for build the transcription 
>  regulation network(TRN) from the regulon database and the motif site scan 
>  result:
>  
>  + ``open_motifdb``: open the position weight matrix(PWM) motif database;
>  + ``motif_search``: scan the TF binding site(TFBS) motif site on the given 
>    promoter/upstream sequence regions;
>  + ``regulation.footprint``: create the regulation footprint(regulation network 
>    edges) from the regulator mapping data(bbh), the motif site data and the 
>    regprecise regulon database;
>  + ``read.regulations``/``write.regulations``: read and save the regulation 
>    footprint data table;
>  + ``read.footprints``: read the motif site(footprint site) table data.

+ [open_motifdb](TRN.builder/open_motifdb.1) open the motif database
+ [motif_search](TRN.builder/motif_search.1) scan the TF binding site motif on the given sequence regions
+ [read.footprints](TRN.builder/read.footprints.1) read a footprint site model data file
+ [read.regulations](TRN.builder/read.regulations.1) read a regulation prediction result file
+ [write.regulations](TRN.builder/write.regulations.1) save the regulation network data file.
+ [regulation.footprint](TRN.builder/regulation.footprint.1) create the regulation footprint(regulation network edges) from the regulator 
