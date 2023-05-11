// export R# package module type define for javascript/typescript language
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
     * @param compares the analysis comparision
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
     * @param geneSet A gsea enrichment background model
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function gsva(expr: any, geneSet: any, env?: object): object;
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
}
