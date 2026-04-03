// export R# package module type define for javascript/typescript language
//
//    imports "pangenome" from "comparative_toolkit";
//
// ref=comparative_toolkit.pangenome@comparative_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

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
     * @param index default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function pav_table(result: object, index?: object, env?: object): object;
   /**
   */
   function report_html(result: object): string;
   /**
    * set orthology group for make gene family
    * 
    * 
     * @param x -
     * @param uf -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function set_ortho_group(x: any, uf: object, env?: object): any;
   /**
   */
   function source_id(genome: object, source_name: string): object;
   /**
     * @param index default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function sv_table(result: object, index?: object, env?: object): object;
}
