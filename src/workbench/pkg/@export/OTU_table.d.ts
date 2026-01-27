// export R# package module type define for javascript/typescript language
//
//    imports "OTU_table" from "metagenomics_kit";
//
// ref=metagenomics_kit.OTUTableTools@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Tools for handling OTU table data
 * 
 * > ### Operational taxonomic unit (OTU)
 * >  
 * >  OTU's are used to categorize bacteria based on sequence similarity.
 * >  
 * >  In 16S metagenomics approaches, OTUs are cluster of similar sequence variants of the 
 * >  16S rDNA marker gene sequence. Each of these cluster is intended to represent a 
 * >  taxonomic unit of a bacteria species or genus depending on the sequence similarity 
 * >  threshold. Typically, OTU cluster are defined by a 97% identity threshold of the 16S 
 * >  gene sequences to distinguish bacteria at the genus level.
 * > 
 * >  Species separation requires a higher threshold Of 98% Or 99% sequence identity, Or 
 * >  even better the use Of exact amplicon sequence variants (ASV) instead Of OTU sequence 
 * >  clusters.
*/
declare namespace OTU_table {
   module as {
      /**
       * Create expression matrix data from a given otu table
       * 
       * 
        * @param otu_table -
      */
      function hts_matrix(otu_table: object): object;
      /**
       * convert the mothur rank tree as the OTU table
       * 
       * 
        * @param x -
        * @param id 
        * + default value Is ``'OTU_num'``.
        * @param taxonomy 
        * + default value Is ``'taxonomy'``.
        * @param env 
        * + default value Is ``null``.
      */
      function OTU_table(x: any, id?: string, taxonomy?: string, env?: object): object;
   }
   /**
    * combine of two batch data
    * 
    * 
     * @param batch1 -
     * @param batch2 -
   */
   function batch_combine(batch1: object, batch2: object): object;
   /**
    * filter the otu data which has relative abundance greater than the given threshold
    * 
    * 
     * @param x -
     * @param relative_abundance -
   */
   function filter(x: object, relative_abundance: number): object;
   /**
   */
   function impute_missing(x: object): any;
   /**
     * @param filter_missing default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function make_otu_table(samples: any, taxonomy_tree: object, filter_missing?: boolean, env?: object): object;
   /**
   */
   function median_scale(x: object): any;
   /**
    * cast the expression matrix to the otu data
    * 
    * 
     * @param x an expression matrix which use the biom taxonomy string as feature unique id reference.
   */
   function otu_from_matrix(x: object): object;
   module read {
      /**
      */
      function OTUdata(file: string): object;
      /**
       * read 16s OTU table
       * 
       * 
        * @param file -
        * @param sum_duplicated sum all OTU data if theirs taxonomy information is the same
        * 
        * + default value Is ``false``.
        * @param OTUTaxonAnalysis 
        * + default value Is ``false``.
      */
      function OTUtable(file: string, sum_duplicated?: boolean, OTUTaxonAnalysis?: boolean): object;
   }
   /**
    * Transform abundance data in an otu_table to relative abundance, sample-by-sample. 
    *  
    *  Transform abundance data into relative abundance, i.e. proportional data. This is 
    *  an alternative method of normalization and may not be appropriate for all datasets,
    *  particularly if your sequencing depth varies between samples.
    * 
    * 
     * @param x -
   */
   function relative_abundance(x: object): object;
}
