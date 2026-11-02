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
    * analysis the functional domain on the protein sequence
    * 
    * 
     * @param blastp the diamond blastp annotation result data for extract the protein 
     *  domain information.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [PfamString](cref:T:SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString) protein domain annotation 
     *  result, one result element for each of the protein sequence in the 
     *  input annotation result data.
   */
   function analysis_domains(blastp: any, env?: object): object;
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
     * @param polyaa returns [StructuralAnnotation](cref:T:SMRUCC.genomics.ProteinModel.ChouFasmanRules.StructuralAnnotation) clr object model if this parameter is set TRUE, otherwise returns 
     *  the string representitive of the chou-fasman structure information.
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the chou-fasman secondary structure prediction result of the input 
     *  protein sequence: a character value of the structure annotation string 
     *  or a [StructuralAnnotation](cref:T:SMRUCC.genomics.ProteinModel.ChouFasmanRules.StructuralAnnotation) clr object when there is only 
     *  one sequence in the input collection, or a named list of the 
     *  prediction result of each sequence in the input collection.
   */
   function chou_fasman(prot: any, polyaa?: boolean, env?: object): string|object;
   /**
    * build the enzyme protein sequence transformer model from a given enzyme 
    *  protein sequence collection
    * 
    * 
     * @param enzymes a protein fasta sequence collection for build the transformer model, 
     *  which can be a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) object, a collection of the 
     *  [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object, or a character vector of the raw 
     *  sequence data.
     * @param kmer the k-mer size for tokenize the protein sequence data.
     * 
     * + default value Is ``3``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a [TransformerModel](cref:T:Microsoft.VisualBasic.MachineLearning.Transformer.TransformerModel) protein sequence model that trained 
     *  from the given enzyme sequence collection, which can be used for 
     *  generate the new protein sequence via the ``predict_sequence`` api.
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
     * @return Returns a single [KMerGraph](cref:T:SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Kmer.KMerGraph) for single sequence input. Returns a named list 
     *  of KMerGraph objects for multiple sequences. Returns error message for invalid inputs.
   */
   function kmer_graph(prot: any, k?: object, env?: object): object;
   /**
    * generate the protein sequence data from the given EC number via the 
    *  enzyme transformer model
    * 
    * 
     * @param model a [TransformerModel](cref:T:Microsoft.VisualBasic.MachineLearning.Transformer.TransformerModel) enzyme protein sequence model, which 
     *  is created by the ``enzyme_builder`` api.
     * @param ec_number a character vector of the enzyme commission(EC) number for generate 
     *  the corresponding protein sequence data.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the generated [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) protein sequence 
     *  object, one sequence element for each of the given EC number.
   */
   function predict_sequence(model: object, ec_number: any, env?: object): object;
   module read {
      /**
       * read the table file of pfam protein domain annotation data, and return the [PfamString](cref:T:SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString) object model
       * 
       * 
        * @param file the file path of the pfam protein domain annotation csv table file.
        * @return a vector of the [PfamString](cref:T:SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString) protein domain annotation 
        *  object that loaded from the given table file.
      */
      function pfam_string(file: string): object;
   }
}
