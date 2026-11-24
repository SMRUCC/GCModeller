# GSEA

### The GCModeller GSEA toolkit
 
 ##### Gene set enrichment analysis
 
 Gene Set enrichment analysis (GSEA) (also called functional enrichment 
 analysis Or pathway enrichment analysis) Is a method To identify classes
 Of genes Or proteins that are over-represented In a large Set Of genes 
 Or proteins, And may have an association With disease phenotypes. The
 method uses statistical approaches To identify significantly enriched 
 Or depleted groups Of genes. Transcriptomics technologies And proteomics 
 results often identify thousands Of genes which are used For the analysis.

+ [read.enrichment](GSEA/read.enrichment.1) Read the enrichment analysis result table from a csv file.
+ [enrichment](GSEA/enrichment.1) ### do GSEA enrichment analysis
+ [fisher](GSEA/fisher.1) Fisher exact test for the enrichment of a single gene set term.
+ [enrichment.go](GSEA/enrichment.go.1) Do the GO (Gene Ontology) GSEA enrichment analysis.
+ [write.enrichment](GSEA/write.enrichment.1) Save the enrichment analysis result as a csv table file.
+ [as.KOBAS_terms](GSEA/as.KOBAS_terms.1) Convert GSEA enrichment result from GCModeller output format to KOBAS output format
+ [enrichment.go_dag](GSEA/enrichment.go_dag.1) Create the network graph data of the GO enrichment result for the Cytoscape.
+ [enrichment.draw.go_dag](GSEA/enrichment.draw.go_dag.1) Draw the GO enrichment DAG graph as a raster image.
+ [cast_enrichs](GSEA/cast_enrichs.1) Create the GCModeller enrichment analysis result objects from the raw data vectors.
+ [to_enrichment_terms](GSEA/to_enrichment_terms.1) Cast a ``dataframe`` object into the CLR enrichment result objects.
