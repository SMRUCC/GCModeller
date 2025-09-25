# proteinKit

A computational biology toolkit for protein structural analysis and sequence-based modeling. 
 This module provides R-language interfaces for predicting secondary structures, parsing molecular 
 structure files, and generating graph-based protein sequence fingerprints.
 
 Key functionalities include:
 1. Chou-Fasman secondary structure prediction algorithm implementation
 2. Protein Data Bank (PDB) file format parsing
 3. K-mer graph construction for sequence pattern analysis
 4. Morgan fingerprint generation for structural similarity comparison
> This module bridges biological sequence analysis with graph theory concepts, enabling:
>  - Rapid prediction of alpha-helices and beta-sheets from amino acid sequences
>  - Structural feature extraction from PDB files
>  - Topological representation of proteins as k-mer adjacency graphs
>  - Fixed-length hashing of structural patterns for machine learning applications
>  
>  Dependencies: 
>  - Requires R# runtime environment for interop
>  - Relies on SMRUCC.genomics libraries for core bioinformatics operations

+ [chou_fasman](proteinKit/chou_fasman.1) The Chou-Fasman method is a bioinformatics technique used for predicting the secondary structure of proteins. 
+ [parse_pdb](proteinKit/parse_pdb.1) parse the pdb struct data from a given document text data
+ [read.pdb](proteinKit/read.pdb.1) Reads a Protein Data Bank (PDB) file and parses it into a PDB object model.
+ [pdb_models](proteinKit/pdb_models.1) get structure models inside the given pdb object
+ [pdb_centroid](proteinKit/pdb_centroid.1) 
+ [ligands](proteinKit/ligands.1) 
+ [kmer_graph](proteinKit/kmer_graph.1) Constructs k-mer adjacency graphs from protein sequence data. Nodes represent k-length 
+ [kmer_fingerprint](proteinKit/kmer_fingerprint.1) Calculate the morgan fingerprint based on the k-mer graph data 
+ [enzyme_builder](proteinKit/enzyme_builder.1) 
+ [predict_sequence](proteinKit/predict_sequence.1) 
