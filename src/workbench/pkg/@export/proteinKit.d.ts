// export R# package module type define for javascript/typescript language
//
//    imports "proteinKit" from "seqtoolkit";
//
// ref=seqtoolkit.proteinKit@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * A computational biology toolkit for protein structural analysis and sequence-based modeling. 
 *  This module provides R-language interfaces for predicting secondary structures, parsing molecular 
 *  structure files, and generating graph-based protein sequence fingerprints.
 *  
 *  Key functionalities include:
 *  1. Chou-Fasman secondary structure prediction algorithm implementation
 *  2. Protein Data Bank (PDB) file format parsing
 *  3. K-mer graph construction for sequence pattern analysis
 *  4. Morgan fingerprint generation for structural similarity comparison
 * 
 * > This module bridges biological sequence analysis with graph theory concepts, enabling:
 * >  - Rapid prediction of alpha-helices and beta-sheets from amino acid sequences
 * >  - Structural feature extraction from PDB files
 * >  - Topological representation of proteins as k-mer adjacency graphs
 * >  - Fixed-length hashing of structural patterns for machine learning applications
 * >  
 * >  Dependencies: 
 * >  - Requires R# runtime environment for interop
 * >  - Relies on SMRUCC.genomics libraries for core bioinformatics operations
*/
declare namespace proteinKit {
   /**
    * The Chou-Fasman method is a bioinformatics technique used for predicting the secondary structure of proteins. 
    *  It was developed by Peter Y. Chou and Gerald D. Fasman in the 1970s. The method is based on the observation 
    *  that certain amino acids have a propensity to form specific types of secondary structures, such as alpha-helices, 
    *  beta-sheets, and turns.
    *  
    *  Here's a brief overview of how the Chou-Fasman method works:
    *  
    *  1. **Amino Acid Propensities**: Each amino acid is assigned a set of probability values that reflect its 
    *     tendency to be found in alpha-helices, beta-sheets, and turns. These values are derived from statistical 
    *     analysis of known protein structures.
    *  2. **Sliding Window Technique**: A sliding window of typically 7 to 9 amino acids is moved along the protein 
    *     sequence. At each position, the average propensity for each type of secondary structure is calculated 
    *     for the amino acids within the window.
    *  3. **Thresholds and Rules**: The method uses predefined thresholds and rules to identify regions of the 
    *     protein sequence that are likely to form alpha-helices or beta-sheets based on the calculated propensities. 
    *     For example, a region with a high average propensity for alpha-helix and meeting certain criteria 
    *     might be predicted to form an alpha-helix.
    *  4. **Secondary Structure Prediction**: The method predicts the secondary structure by identifying contiguous 
    *     regions of the sequence that exceed the thresholds for helix or sheet formation. It also takes into 
    *     account the likelihood of turns, which are important for the overall folding of the protein.
    *  5. **Refinement**: The initial predictions are often refined using additional rules and considerations, such 
    *     as the tendency of certain amino acids to stabilize or destabilize specific structures, and the overall 
    *     composition of the protein.
    *     
    *  The Chou-Fasman method was one of the first widely used techniques for predicting protein secondary structure
    *  and played a significant role in the field of structural bioinformatics. However, it has largely been superseded
    *  by more accurate methods, such as those based on machine learning and neural networks, which can take into
    *  account more complex patterns and interactions within protein sequences.
    *  
    *  Despite its limitations, the Chou-Fasman method remains a historical milestone in the understanding of 
    *  protein structure and the development of computational methods for predicting it. It also serves as a 
    *  foundational concept for those learning about protein structure prediction and bioinformatics.
    * 
    * 
     * @param prot a collection of the protein sequence data
     * @param polyaa returns @``T:SMRUCC.genomics.ProteinModel.ChouFasmanRules.StructuralAnnotation`` clr object model if this parameter is set TRUE, otherwise returns 
     *  the string representitive of the chou-fasman structure information.
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function chou_fasman(prot: any, polyaa?: boolean, env?: object): string|object;
   /**
     * @param kmer default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function enzyme_builder(enzymes: any, kmer?: object, env?: object): object;
   /**
    * Calculate the morgan fingerprint based on the k-mer graph data 
    *  
    *  Generates fixed-length molecular fingerprint vectors from k-mer graphs using 
    *  Morgan algorithm with circular topology hashing.
    * 
    * 
     * @param graph The k-mer graph object to fingerprint
     * @param radius Neighborhood radius for structural feature capture. Larger values 
     *  consider more distant node relationships. Default is 3.
     * 
     * + default value Is ``3``.
     * @param len Output vector length (uses modulo hashing). Default 4096.
     * 
     * + default value Is ``4096``.
     * @return Integer array fingerprint where indices represent structural features
   */
   function kmer_fingerprint(graph: object, radius?: object, len?: object): any;
   /**
    * Constructs k-mer adjacency graphs from protein sequence data. Nodes represent k-length 
    *  subsequences, edges connect k-mers appearing consecutively in the sequence.
    * 
    * 
     * @param prot A FASTA sequence or collection of FASTA sequences to process.
     * @param k The subsequence length parameter for k-mer generation. Default is 3.
     * 
     * + default value Is ``3``.
     * @param env The R runtime environment for error handling and resource cleanup.
     * 
     * + default value Is ``null``.
     * @return Returns a single @``T:SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Kmer.KMerGraph`` for single sequence input. Returns a named list 
     *  of KMerGraph objects for multiple sequences. Returns error message for invalid inputs.
   */
   function kmer_graph(prot: any, k?: object, env?: object): object;
   /**
     * @param key default value Is ``null``.
     * @param number default value Is ``-1``.
   */
   function ligands(pdb: object, key?: string, number?: object): object;
   /**
    * parse the pdb struct data from a given document text data
    * 
    * 
     * @param pdb_txt -
     * @param safe -
     * 
     * + default value Is ``false``.
     * @param verbose 
     * + default value Is ``false``.
   */
   function parse_pdb(pdb_txt: string, safe?: boolean, verbose?: boolean): object;
   /**
     * @param as_vector default value Is ``false``.
   */
   function pdb_centroid(pdb: object, as_vector?: boolean): object|number;
   /**
    * get structure models inside the given pdb object
    * 
    * 
     * @param pdb -
   */
   function pdb_models(pdb: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function predict_sequence(model: object, ec_number: any, env?: object): object;
   module read {
      /**
       * Reads a Protein Data Bank (PDB) file and parses it into a PDB object model.
       * 
       * 
        * @param file A file path string or Stream object representing the PDB file to read.
        * @param safe 
        * + default value Is ``false``.
        * @param env The R runtime environment for error handling and resource management.
        * 
        * + default value Is ``null``.
        * @return Returns a parsed @``T:SMRUCC.genomics.Data.RCSB.PDB.PDB`` object if successful. Returns a @``T:SMRUCC.Rsharp.Runtime.Components.Message`` 
        *  error object if file loading fails due to invalid path or format issues.
      */
      function pdb(file: any, safe?: boolean, env?: object): object;
   }
}
