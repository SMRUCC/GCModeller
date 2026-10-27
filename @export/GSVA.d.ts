// export R# package module type define for javascript/typescript language
//
//    imports "GSVA" from "gseakit";
//
// ref=gseakit.GSVA@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Gene Set Variation Analysis for microarray and RNA-seq data
 * 
*/
declare namespace GSVA {
   /**
    * different analysis of the gsva result
    * 
    * 
     * @param gsva the gsva analysis result
     * @param compares the [DataAnalysis](cref:T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.DataAnalysis) comparision collection
     * @return this function returns a array of the [GSVADiff](cref:T:SMRUCC.genomics.Visualize.GSVADiff) object which is compares the different of the gsva score via the limma function. 
     *  for get the different score of gsva score in raw [LimmaTable](cref:T:SMRUCC.genomics.Analysis.HTS.DataFrame.LimmaTable) array type, please use the ``geneExpression::limma`` function for analysis the gsva difference result across two condition groups.
   */
   function diff(gsva: object, compares: object): object;
   /**
    * Gene Set Variation Analysis for microarray and RNA-seq data
    *  
    *  Gene Set Variation Analysis (GSVA) is a non-parametric, unsupervised 
    *  method for estimating variation of gene set enrichment through the
    *  samples of a expression data set. GSVA performs a change in coordinate
    *  systems, transforming the data from a gene by sample matrix to a gene-set
    *  by sample matrix, thereby allowing the evaluation of pathway enrichment 
    *  for each sample. This new matrix of GSVA enrichment scores facilitates
    *  applying standard analytical methods like functional enrichment, 
    *  survival analysis, clustering, CNV-pathway analysis or cross-tissue 
    *  pathway analysis, in a pathway-centric manner.
    *  
    *  main function of the package which estimates activity
    *  scores For Each given gene-Set
    * 
    * > Hänzelmann S., Castelo R. and Guinney J. GSVA: gene set variation analysis
    * >  for microarray and RNA-Seq data. BMC Bioinformatics, 14:7, 2013.
    * 
     * @param expr A raw gene expression data matrix object
     * @param geneSet A gsea enrichment [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model
     * @param name_suffix append the pathway name to the map id as suffix?
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function gsva(expr: any, geneSet: any, name_suffix?: boolean, env?: object): object;
   /**
    * convert to diff data from dataframe
    * 
    * 
     * @param diff -
     * @param pathId -
     * 
     * + default value Is ``'pathNames'``.
     * @param t -
     * 
     * + default value Is ``'t'``.
     * @param pvalue -
     * 
     * + default value Is ``'pvalue'``.
   */
   function matrix_to_diff(diff: object, pathId?: string, t?: string, pvalue?: string): object;
   /**
     * @param verbose default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function mo_layers(layers: any, verbose?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function mogsva(x: object, geneSet: any, opts: object, env?: object): object;
   /**
   */
   function mogsva_opts(): object;
}
