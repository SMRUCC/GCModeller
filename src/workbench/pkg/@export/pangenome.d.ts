// export R# package module type define for javascript/typescript language
//
//    imports "pangenome" from "comparative_toolkit";
//
// ref=comparative_toolkit.pangenome@comparative_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * pan-genome analysis toolkit
 * 
*/
declare namespace pangenome {
   /**
    * run pan-genome analysis
    * 
    * 
     * @param pangenome context data for run pan-genome analysis
     * @param orthologSet gene ortholog data table, usually be the bi-direction best hit of the blast result
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function analysis(pangenome: object, orthologSet: object, env?: object): object;
   /**
    * Load the pangenome analysis context
    * 
    * 
     * @param genomes should be a collection of the genome GFF3 feature tables
     * @param soft_core_threshold threshold value for identify the gene as soft core, thres value 1 means core genes
     * 
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
    * generates the html report text for the pan-genome analysis result
    * 
    * 
     * @param result -
   */
   function report_html(result: object): string;
   /**
    * set orthology group for make gene family
    * 
    * 
     * @param x gene ortholog annotation result set
     * @param uf -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function set_ortho_group(x: any, uf: object, env?: object): any;
   /**
    * set species name to the genome gff3 context model
    * 
    * 
     * @param genome -
     * @param source_name -
   */
   function source_id(genome: object, source_name: string): object;
   /**
    * export structure variant result table
    * 
    * 
     * @param result -
     * @param index -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function sv_table(result: object, index?: object, env?: object): object;
}
