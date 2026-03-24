// export R# package module type define for javascript/typescript language
//
//    imports "pangenome" from "seqtoolkit";
//
// ref=seqtoolkit.pangenome@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace pangenome {
   /**
     * @param env default value Is ``null``.
   */
   function analysis(pangenome: object, orthologSet: object, env?: object): object;
   /**
    * Load the pangenome analysis context
    * 
    * 
     * @param genomes should be a collection of the genome GFF feature tables
     * @param soft_core_threshold 
     * + default value Is ``0.95``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function build_context(genomes: any, soft_core_threshold?: number, env?: object): object;
   /**
   */
   function pav_table(result: object): object;
   /**
   */
   function report_html(result: object): string;
   /**
   */
   function source_id(genome: object, source_name: string): object;
   /**
   */
   function sv_table(result: object): object;
}
