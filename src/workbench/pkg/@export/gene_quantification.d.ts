// export R# package module type define for javascript/typescript language
//
//    imports "gene_quantification" from "rnaseq";
//
// ref=rnaseq.Quantification@rnaseq, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace gene_quantification {
   /**
    * 
    * 
     * @param counts A collection of the gene @``T:SMRUCC.genomics.SequenceModel.SAM.featureCount.featureCounts`` data.
     * @param env The R environment.
     * 
     * + default value Is ``null``.
     * @return A collection of gene expression data in TPM format.
   */
   function convert_to_tpm(counts: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function counts_matrix(counts: any, env?: object): object;
   /**
    * Apply DESeq2 Median of Ratios normalization method to the raw counts matrix.
    * 
    * 
     * @param counts The raw counts matrix.
     * @return The normalized counts matrix.
   */
   function deseq2_norm(counts: object): object;
   /**
    * Apply of the edgeR TMM factor normalization method to the raw counts matrix
    * 
    * 
     * @param counts -
     * @param trimFractionM 
     * + default value Is ``0.3``.
     * @param trimFractionA 
     * + default value Is ``0.05``.
   */
   function edgeR_norm(counts: object, trimFractionM?: number, trimFractionA?: number): object;
   /**
     * @param trimFractionM default value Is ``0.3``.
     * @param trimFractionA default value Is ``0.05``.
   */
   function edgeR_tmm(countData: object, trimFractionM?: number, trimFractionA?: number): any;
   /**
     * @param env default value Is ``null``.
   */
   function expression_data(sampledata: any, env?: object): object;
   /**
   */
   function gene_indexstats(file: string): object;
   /**
   */
   function read_featureCounts(file: string): object;
   /**
   */
   function read_genedata(file: string): object;
}
