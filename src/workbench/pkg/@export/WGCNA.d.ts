// export R# package module type define for javascript/typescript language
//
//    imports "WGCNA" from "phenotype_kit";
//    imports "WGCNA" from "TRNtoolkit";
//
// ref=phenotype_kit.WGCNA@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=TRNtoolkit.WGCNA@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * WGCNA, which stands for Weighted Gene Co-expression Network Analysis, is a systems biology method used to describe the 
 *  correlation patterns among genes across different samples. It is particularly useful for identifying modules of 
 *  co-expressed genes, which can then be correlated with external sample traits such as clinical features or environmental 
 *  conditions. Here's a brief overview of how WGCNA works and its applications:
 *  
 *  ### Key Concepts:
 *  
 *  1. **Co-expression Networks**: Genes that are co-expressed across different conditions or samples are likely to be 
 *     functionally related. WGCNA constructs a network where nodes represent genes, and edges represent the pairwise 
 *     correlations between genes.
 *  2. **Weighted Networks**: Traditional correlation-based networks use Pearson or Spearman correlations, which are 
 *     unweighted. WGCNA uses a weighted approach, often employing the soft thresholding of the correlation matrix to 
 *     transform it into a weighted adjacency matrix. This weighting helps to amplify strong correlations and diminish 
 *     weak ones, making the network more robust to noise.
 *  3. **Modules**: Groups of highly correlated genes are identified as modules. These modules are clusters of genes 
 *     that have similar expression profiles across the samples and are often enriched for specific biological functions 
 *     or pathways.
 *  4. **Topological Overlap Matrix (TOM)**: WGCNA uses the TOM to measure the network connectivity of genes, which 
 *     considers not only direct connections but also shared neighbors. This helps in identifying modules more accurately.
 *  5. **Eigengenes**: Each module can be represented by an eigengene, which is the first principal component of the gene
 *     expression profiles within the module. The eigengene serves as a representative of the module's expression pattern.
 *     
 *  ### Steps in WGCNA:
 *  
 *  1. **Data Preprocessing**: This includes filtering out low-quality genes, normalizing expression data, and handling missing values.
 *  2. **Network Construction**: Calculate the pairwise correlation matrix and apply soft thresholding to create a weighted adjacency matrix.
 *  3. **Module Detection**: Use hierarchical clustering or other clustering methods on the TOM to identify modules of co-expressed genes.
 *  4. **Module Eigengenes**: Compute the eigengene for each module to represent its expression pattern.
 *  5. **Relating Modules to External Traits**: Correlate module eigengenes with external sample traits to identify which modules are associated with specific conditions or phenotypes.
 *  6. **Functional Enrichment Analysis**: Perform gene ontology (GO) or pathway enrichment analysis on the genes within each 
 *     module to infer their biological functions.
 *     
 *  ### Applications:
 *  
 *  - **Disease Biomarker Discovery**: Identifying gene modules associated with disease states can lead to the discovery of novel biomarkers.
 *  - **Understanding Disease Mechanisms**: By analyzing the functions of co-expressed gene modules, researchers can gain insights into the molecular mechanisms underlying diseases.
 *  - **Drug Target Identification**: Modules that are significantly altered in disease states may contain potential drug targets.
 *  - **Comparative Analysis**: WGCNA can be used to compare gene expression patterns across different species, tissues, or conditions.
 *  
 *  ### Tools and Software:
 *  
 *  WGCNA is implemented in R, and there is a comprehensive package available for users to perform the analysis. The package provides 
 *  functions for all steps of the analysis, from data preprocessing to module detection and trait correlation.
 *  
 *  ### Limitations:
 *  
 *  - **Sample Size**: WGCNA requires a sufficient number of samples to reliably detect co-expression patterns.
 *  - **Interpretation**: While WGCNA can identify co-expressed modules, interpreting their biological significance often requires additional functional validation.
 *  - **Computational Intensity**: The analysis can be computationally intensive, especially with large datasets.
 *  
 *  WGCNA is a powerful tool for exploring gene co-expression patterns and has been widely used in genomics research to uncover 
 *  the underlying biology of complex traits and diseases.
 * 
 * 
*/
declare namespace WGCNA {
   /**
   */
   function applyModuleColors(g: object, modules: object): any;
   /**
    * export a dataframe of the node information with connectivity value
    * 
    * 
     * @param x -
   */
   function connectivity(x: object): any;
   /**
    * Create correlation network based on WGCNA method
    * 
    * 
     * @param x should be an expression matrix object of gene features in rows and sample id in columns
     * @param adjacency -
     * 
     * + default value Is ``0.6``.
     * @param pca_layout 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function cor_network(x: object, adjacency?: number, pca_layout?: boolean, env?: object): object;
   /**
    * append protein iteration network based on the WGCNA weights.
    * 
    * 
     * @param g -
     * @param WGCNA -
     * @param modules -
     * @param threshold -
     * 
     * + default value Is ``0.3``.
   */
   function interations(g: object, WGCNA: object, modules: object, threshold?: number): any;
   /**
    * load network graph from the WGCNA exportNetworkToCytoscape function exports
    * 
    * 
     * @param edges -
     * @param nodes -
     * @param threshold -
     * 
     * + default value Is ``0``.
     * @param prefix 
     * + default value Is ``null``.
   */
   function load_TOM_graph(edges: string, nodes: string, threshold?: number, prefix?: string): object;
   module read {
      /**
       * load TOM module network nodes
       * 
       * 
        * @param file the TOM network nodes text file, should be a tsv file of the cytoscape network export result
        * @param prefix 
        * + default value Is ``null``.
        * @param result_modules 
        * + default value Is ``false``.
      */
      function modules(file: any, prefix?: string, result_modules?: boolean): any;
      /**
       * read the TOM correlation network matrix file
       * 
       * 
        * @param file -
        * @param threshold -
        * 
        * + default value Is ``0``.
        * @param prefix a prefix to the fromNode and toNode id
        * 
        * + default value Is ``null``.
        * @param as_matrix 
        * + default value Is ``false``.
      */
      function weight_matrix(file: any, threshold?: number, prefix?: string, as_matrix?: boolean): object|object;
   }
   /**
     * @param prefix default value Is ``null``.
   */
   function read_clusters(file: string, prefix?: string): object;
   /**
    * filter regulation network by WGCNA result weights
    * 
    * 
     * @param g -
     * @param WGCNA -
     * @param threshold -
     * 
     * + default value Is ``0.3``.
   */
   function shapeTRN(g: object, WGCNA: object, threshold?: number): any;
}
