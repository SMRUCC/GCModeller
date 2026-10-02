# annotation.genomics

Genomics context data annotation toolkit
> This R# package module provides the api for read/write and manipulate the 
>  genomics context annotation data:
>  
>  + read the genomics context annotation table file: ``read.gff``, 
>    ``read.gtf``, ``read.nucmer``;
>  + export the genomics context annotation data: ``write.gff3``, 
>    ``as.tabular``, ``write.PTT_tabular``;
>  + query the genomics context features: ``gff_features``, 
>    ``source_features``, ``type_features``, ``genes_features``, ``upstream``;
>  + extract the sequence region data: ``extract_gff_seqs``.

+ [read.gtf](annotation.genomics/read.gtf.1) read the gtf format genomics context annotation table file
+ [read.gff](annotation.genomics/read.gff.1) read the gff3 file
+ [write.gff3](annotation.genomics/write.gff3.1) save the genomics feature annotation table object as a gff3 format 
+ [source_features](annotation.genomics/source_features.1) get the genomics features from the given gff table by its source name
+ [type_features](annotation.genomics/type_features.1) get the genomics features from the given gff table by its feature type
+ [gff_features](annotation.genomics/gff_features.1) get gff features by id reference
+ [as.tabular](annotation.genomics/as.tabular.1) export the gene annotation data as the tabular format table object
+ [as.geneTable](annotation.genomics/as.geneTable.1) export the PTT tabular table object as a collection of the gene table 
+ [as.PTT](annotation.genomics/as.PTT.1) export the genbank database file object as a PTT tabular table object
+ [upstream](annotation.genomics/upstream.1) Create the upstream location
+ [genes_features](annotation.genomics/genes_features.1) Extract all gene features from a given genomics context assembly data
+ [write.PTT_tabular](annotation.genomics/write.PTT_tabular.1) write the genomics context annotation data as the PTT tabular format 
+ [read.nucmer](annotation.genomics/read.nucmer.1) read the nucmer alignment delta format file
+ [extract_gff_seqs](annotation.genomics/extract_gff_seqs.1) extract the sequence region data of the genomics features in the given 
