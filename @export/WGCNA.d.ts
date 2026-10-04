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
 * Workflow for make TRN data model based on the WGCNA co-expression network and TF id list.
 * 
*/
declare namespace WGCNA {
   /**
   */
   function applyModuleColors(g: object, modules: object): any;
   /**
    * build the GRN prior network from the expression matrix or the cached bicor correlation store.
    * 
    * > performance notes: when the `bicor` store is present, the candidate
    * >  edges and p-values are read from the cached correlation matrix and the
    * >  WGCNA module map is resolved through a three-level cache (explicit
    * >  `modules` argument => `{storeFile}.modules` sidecar with
    * >  fingerprint validation => fresh blockwise run with automatic cache
    * >  write-back), so repeated calls with different thresholds do not re-run
    * >  either the correlation computation or the WGCNA module detection.
    * 
     * @param x gene expression matrix (genes x samples)
     * @param TF transcription factor gene id vector
     * @param opts pipeline options
     * @param bicor optional cached bicor correlation matrix store (from `open_bicor`).
     *  When present, the candidate edges are read from the store instead of
     *  re-computing the correlation matrix.
     * 
     * + default value Is ``null``.
     * @param modules optional cached WGCNA module map (from `open_modules`). When Nothing and
     *  **bicor** is present, a sidecar cache `{store}.modules` is
     *  tried automatically (with fingerprint validation); on cache miss the WGCNA
     *  blockwise module detection runs once and the result is written back to the
     *  sidecar file.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the assembled [GRNBuildResult](cref:T:SMRUCC.genomics.Analysis.CellPhenotype.RegulationNetwork.GRNBuildResult): the prior regulatory network
     *  split by the WGCNA modules (each module holds a
     *  [PriorNetwork](cref:T:SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork) of [RegulatoryEdge](cref:T:SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge) edges
     *  carrying TF, target gene, regulation type, confidence and evidence tags),
     *  together with the build summary statistics. Use
     *  `result.ToPriorNetwork()` / `result.ToPriorNetwork(minConfidence)`
     *  to merge it into a single network for the downstream DBN or GNN modeling.
   */
   function build_grn(x: object, TF: any, opts: object, bicor?: object, modules?: object, env?: object): object;
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
     * @param x should be an HTS expression matrix object of gene features in rows and sample id in columns or the adjacency matrix which is read via ``read.adjacency`` function.
     * @param adjacency -
     * 
     * + default value Is ``0.6``.
     * @param pca_layout 
     * + default value Is ``true``.
     * @param args additional parameters for create correlation network based on the adjacency matrix directly: ``membership`` for gene to modulee membership correlation result.
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function cor_network(x: any, adjacency?: number, pca_layout?: boolean, args?: object, env?: object): object|object;
   /**
    * Get the correlation matrix of the given expression data matrix, and then calculate the correlation value and p-value of the given id1 and id2.
    * 
    * 
     * @param expr -
     * @param id1 -
     * 
     * + default value Is ``null``.
     * @param id2 -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function expr_cor(expr: any, id1?: any, id2?: any, env?: object): object|object;
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
   function interations(g: object, WGCNA: object, modules: object, threshold?: number): object;
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
     * @param id_subset 
     * + default value Is ``null``.
   */
   function load_TOM_graph(edges: string, nodes: string, threshold?: number, prefix?: string, id_subset?: any): object;
   /**
     * @param env default value Is ``null``.
   */
   function phenotype_matrix(x: any, env?: object): any;
   /**
    * Build bnlearn prior network based on the WGCNA co-expression network and TF id list.
    * 
    * 
     * @param edges -
     * @param TF -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function prior_network(edges: any, TF: any, env?: object): object;
   module read {
      /**
      */
      function adjacency(file: string): object;
      /**
      */
      function module_cor(file: string): any;
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
       * > imports a network edge table file that export from WGCNA TOM module, with data headers: 
       * >  
       * > 
       * > 
       * >  fromNode
       * > 
       * >  toNode
       * > 
       * >  weight
       * > 
       * >  direction
       * > 
       * >  fromAltName
       * > 
       * >  toAltName
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
    * read network edges table which is save via igraph package
    * 
    * 
     * @param file -
     * @param cor_thres -
     * 
     * + default value Is ``0.65``.
     * @param env 
     * + default value Is ``null``.
   */
   function read_wgcna_edges(file: string, cor_thres?: number, env?: object): object;
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
   function shapeTRN(g: object, WGCNA: object, threshold?: number): object;
   /**
    * locate the STRING protein interaction data files inside a STRING database
    *  folder and attach them to the GRN pipeline options.
    * 
    * > STRING protein interactions are used by the GRN pipeline in two configurable
    * >  ways: as a confidence re-weighting evidence for the existing co-expression
    * >  edges, and/or as a topology completion source that adds the protein
    * >  interaction pairs missing in the co-expression network (see
    * >  `GRN_opts` fields `stringAddEdges`, `stringMinScore` and
    * >  `stringWeight`).
    * 
     * @param opts the GRN pipeline options object (usually created via `new("GRN_opts", ...)`
     *  and possibly piped through other option helpers). This function mutates and
     *  returns the same options object.
     * @param string_db the directory of the extracted STRING database files (for example
     *  `K:\hsa_grn\string-db`). The links file is picked with the priority:
     *  compact `9606.protein.links.v*.txt` (3 columns) > detailed version >
     *  any other `*protein.links*.txt`; the `.gz` archives and the
     *  physical-subset files are always skipped. The aliases file
     *  (`*protein.aliases*.txt`) is located as well and used to build the
     *  Ensembl gene id => STRING protein id mapping automatically.
     * @return the same [GRNBuildOptions](cref:T:SMRUCC.genomics.Analysis.CellPhenotype.RegulationNetwork.GRNBuildOptions) object with the
     *  `stringLinks` and `stringAliases` file paths filled in.
     *  When no links file is found, the STRING protein interaction evidence is
     *  disabled (a warning is printed).
   */
   function string_links(opts: object, string_db: string): object;
}
