# GenBank

NCBI genbank assembly file I/O toolkit

+ [read.genbank](GenBank/read.genbank.1) read the given genbank assembly file.
+ [taxon_id](GenBank/taxon_id.1) get ncbi taxonomy id from the given genbank assembly file.
+ [taxonomy_lineage](GenBank/taxonomy_lineage.1) extract the taxonomy lineage information from the genbank file
+ [as_tabular](GenBank/as_tabular.1) extract all gene features from genbank and cast to tabular data
+ [accession_id](GenBank/accession_id.1) get current genbank assembly accession id
+ [is.plasmid](GenBank/is.plasmid.1) check of the given genbank assembly is the data source of a plasmid or not?
+ [load_genbanks](GenBank/load_genbanks.1) populate a list of genbank data objects from a given list of files or stream.
+ [write.genbank](GenBank/write.genbank.1) save the modified genbank file
+ [as.genbank](GenBank/as.genbank.1) converts tabular data file to genbank assembly object
+ [feature](GenBank/feature.1) create new feature site
+ [add_feature](GenBank/add_feature.1) add feature into a given genbank object
+ [enumerateFeatures](GenBank/enumerateFeatures.1) enumerate all features in the given NCBI genbank database object
+ [featureKeys](GenBank/featureKeys.1) get all feature key names
+ [featureMeta](GenBank/featureMeta.1) extract the feature metadata from a genbank clr feature object
+ [addMeta](GenBank/addMeta.1) add metadata into a given feature object
+ [origin_fasta](GenBank/origin_fasta.1) get, add or replace the genome origin fasta sequence in the given genbank assembly file.
+ [getRNA.fasta](GenBank/getRNA.fasta.1) get all of the RNA gene its gene sequence in fasta sequence format.
+ [export_geneNt_fasta](GenBank/export_geneNt_fasta.1) export gene fasta from the given genbank assembly file
+ [export_protein_fasta](GenBank/export_protein_fasta.1) get or set fasta sequence of all CDS feature in the given genbank assembly file.
+ [add.RNA.gene](GenBank/add.RNA.gene.1) 
