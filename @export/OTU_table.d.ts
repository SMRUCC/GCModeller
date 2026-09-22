// export R# package module type define for javascript/typescript language
//
//    imports "OTU_table" from "metagenomics_kit";
//
// ref=metagenomics_kit.OTUTableTools@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace OTU_table {
   module as {
      /**
        * @param taxon_as_id default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function hts_matrix(otu_table: any, taxon_as_id?: boolean, env?: object): object;
      /**
        * @param id default value Is ``'OTU_num'``.
        * @param taxonomy default value Is ``'taxonomy'``.
        * @param env default value Is ``null``.
      */
      function OTU_table(x: any, id?: string, taxonomy?: string, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function average(x: any, sampleinfo: any, env?: object): object;
   /**
   */
   function batch_combine(batch1: object, batch2: object): object;
   /**
     * @param prevalence default value Is ``0.8``.
     * @param abundance default value Is ``0.0001``.
     * @param detectionLimit default value Is ``1E-05``.
     * @param sampleinfo default value Is ``null``.
     * @param top_n default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function core_microbiome(x: any, prevalence?: number, abundance?: number, detectionLimit?: number, sampleinfo?: any, top_n?: object, env?: object): object;
   /**
     * @param cutoff default value Is ``0.01``.
     * @param k default value Is ``10``.
     * @param sampleinfo default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function dominant_species(x: any, cutoff?: number, k?: object, sampleinfo?: any, env?: object): object;
   /**
     * @param args default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function drawUPGMATree(tree: object, args?: object, env?: object): any;
   /**
   */
   function filter(x: object, relative_abundance: number): object;
   /**
     * @param filter_missing default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function make_otu_table(samples: any, taxonomy_tree: object, filter_missing?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function make_repseq_id(otus: any, rep: any, env?: object): object;
   /**
     * @param equals default value Is ``0.85``.
     * @param gt default value Is ``0.6``.
     * @param rank_colors default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function makeTreeGraph(otus: any, equals?: number, gt?: number, rank_colors?: object, env?: object): object;
   /**
     * @param as_graph default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function makeUPGMATree(otus: any, as_graph?: boolean, env?: object): object;
   /**
   */
   function median_scale(x: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function merge_phyloseq(batch1: any, batch2: any, env?: object): object;
   /**
   */
   function otu_from_matrix(x: object): object;
   module read {
      /**
      */
      function OTUdata(file: string): object;
      /**
        * @param sum_duplicated default value Is ``false``.
        * @param OTUTaxonAnalysis default value Is ``false``.
      */
      function OTUtable(file: string, sum_duplicated?: boolean, OTUTaxonAnalysis?: boolean): object;
      /**
      */
      function rankdata(file: string): object;
   }
   /**
   */
   function relative_abundance(x: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function sample_id(x: any, env?: object): any;
   /**
   */
   function set_MAG_data(tax: object, abundance: object): object;
   /**
     * @param rank default value Is ``null``.
     * @param sum_duplicates default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function set_taxonomyName(x: any, rank?: object, sum_duplicates?: boolean, env?: object): any;
}
