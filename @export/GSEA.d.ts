// export R# package module type define for javascript/typescript language
//
//    imports "GSEA" from "gseakit";
//
// ref=gseakit.GSEA@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * ### The GCModeller GSEA toolkit
 *  
 *  ##### Gene set enrichment analysis
 *  
 *  Gene Set enrichment analysis (GSEA) (also called functional enrichment 
 *  analysis Or pathway enrichment analysis) Is a method To identify classes
 *  Of genes Or proteins that are over-represented In a large Set Of genes 
 *  Or proteins, And may have an association With disease phenotypes. The
 *  method uses statistical approaches To identify significantly enriched 
 *  Or depleted groups Of genes. Transcriptomics technologies And proteomics 
 *  results often identify thousands Of genes which are used For the analysis.
 * 
*/
declare namespace GSEA {
   module as {
      /**
       * Convert GSEA enrichment result from GCModeller output format to KOBAS output format
       * 
       * 
        * @param enrichment the GCModeller enrichment analysis result set.
        * @param database the database label that will be written into the ``Database`` column of the KOBAS table. Default ``n/a``.
        * 
        * + default value Is ``'n/a'``.
        * @return an array of the [EnrichmentTerm](cref:T:SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm) objects in the KOBAS table format.
      */
      function KOBAS_terms(enrichment: object, database?: string): object;
   }
   /**
    * Create the GCModeller enrichment analysis result objects from the raw data vectors.
    * 
    * 
     * @param term the term id vector.
     * @param name the term name vector, its length should be equals to the ``term`` vector.
     * @param pvalue the p-value vector of each term.
     * @param geneIDs a named list of the member gene id vector of each term, the name of the list slot
     *  should be the term id. Default ``NULL`` means no member gene id.
     * 
     * + default value Is ``null``.
     * @param desc the description vector of each term. Default ``NULL``.
     * 
     * + default value Is ``null``.
     * @param score the enrichment score vector of each term. Default ``NULL``.
     * 
     * + default value Is ``null``.
     * @param fdr the adjusted p-value (FDR) vector of each term. Default ``NULL``.
     * 
     * + default value Is ``null``.
     * @param cluster the cluster size vector of each term. Default ``NULL``.
     * 
     * + default value Is ``null``.
     * @param enriched the enriched gene id vector joined by ``;`` of each term. Default ``NULL``.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return an array of the [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult) objects.
   */
   function cast_enrichs(term: string, name: string, pvalue: number, geneIDs?: object, desc?: string, score?: number, fdr?: number, cluster?: object, enriched?: string, env?: object): object;
   module enrichment {
      module draw {
         /**
          * Draw the GO enrichment DAG graph as a raster image.
          * 
          * 
           * @param go_enrichment the GO enrichment analysis result set.
           * @param go the GO obo database that is used for creating the DAG graph.
           * @return the raster image data of the GO enrichment DAG graph.
         */
         function go_dag(go_enrichment: object, go: object): object;
      }
      /**
       * Do the GO (Gene Ontology) GSEA enrichment analysis.
       * 
       * 
        * @param background the GSEA background model that contains the GO terms, for example the model created by ``dag.background``.
        * @param geneSet the gene id list for the enrichment analysis.
        * @param go the go database object can be the raw go obo data or the DAG graph model.
        * @param showProgress prints the progress message? Default ``TRUE``.
        * 
        * + default value Is ``true``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return an array of the [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult) objects, which is sorted by
        *  the ``FDR`` value in ascending order.
      */
      function go(background: object, geneSet: string, go: any, showProgress?: boolean, env?: object): object;
      /**
       * Create the network graph data of the GO enrichment result for the Cytoscape.
       * 
       * 
        * @param go_enrichment the GO enrichment analysis result set.
        * @param go the GO obo database that is used for creating the DAG graph.
        * @return the network node and edge tables of the GO enrichment DAG graph.
      */
      function go_dag(go_enrichment: object, go: object): object;
   }
   /**
    * Fisher exact test for the enrichment of a single gene set term.
    * 
    * 
     * @param list the gene id list that are observed in the target term.
     * @param geneSet all member gene ids of the target term.
     * @param background the background size information, it could be an integer value to 
     *  indicates that the total unique size of the enrichment background 
     *  or a unique id character vector that contains all member 
     *  information of the background.
     * @param term the term id (or the term name) that will be written into the result object. Default ``unknown``.
     * 
     * + default value Is ``'unknown'``.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult) object of the given term.
   */
   function fisher(list: string, geneSet: string, background: any, term?: string, env?: object): object;
   module read {
      /**
       * Read the enrichment analysis result table from a csv file.
       * 
       * 
        * @param file the csv file path of the enrichment analysis result, the file should be written
        *  by the ``write.enrichment`` function.
        * @return an array of the [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult) objects.
      */
      function enrichment(file: string): object;
   }
   /**
    * Cast a ``dataframe`` object into the CLR enrichment result objects.
    * 
    * 
     * @param x the dataframe object that contains the enrichment result data, the row names
     *  should be the term id and the required columns are ``name``, ``pvalue`` and
     *  ``geneIDs`` (the member gene ids joined by ``;``).
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return an array of the [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult) objects.
   */
   function to_enrichment_terms(x: object, env?: object): object;
   module write {
      /**
       * Save the enrichment analysis result as a csv table file.
       * 
       * 
        * @param enrichment should be a set of the enrichment analysis result, both of the
        *  [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult) and the [EnrichmentTerm](cref:T:SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm) array are
        *  accepted.
        * @param file the output csv file path.
        * @param format the output table format of the [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult) data:
        *  ``GCModeller`` (default) or ``KOBAS``.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return this function returns ``TRUE`` when the file is saved successfully.
      */
      function enrichment(enrichment: any, file: string, format?: object, env?: object): boolean;
   }
}
