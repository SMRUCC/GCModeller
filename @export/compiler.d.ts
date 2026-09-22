// export R# package module type define for javascript/typescript language
//
//    imports "compiler" from "vcellkit";
//
// ref=vcellkit.Compiler@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace compiler {
   module assembling {
      /**
        * @param lociAsLocus_tag default value Is ``false``.
      */
      function genome(replicons: object, geneKO: object, lociAsLocus_tag?: boolean): object;
      /**
      */
      function metabolic(cell: object, geneKO: object, repo: object): object;
      /**
      */
      function TRN(model: object, regulations: object): object;
   }
   /**
     * @param logfile default value Is ``'./gcc.log'``.
   */
   function compile_biocyc(biocyc: object, logfile?: string): object;
   /**
     * @param env default value Is ``null``.
   */
   function compile_network(x: object, env?: object): any;
   module geneKO {
      /**
        * @param KOcol default value Is ``'KO'``.
        * @param geneIDcol default value Is ``'ID'``.
        * @param env default value Is ``null``.
      */
      function maps(data: any, KOcol?: string, geneIDcol?: string, env?: object): object;
   }
   /**
   */
   function kegg(compounds: string, maps: string, reactions: string, glycan2Cpd: object): object;
   module vcell {
      /**
        * @param lociAsLocus_tag default value Is ``false``.
        * @param logfile default value Is ``null``.
      */
      function markup(model: object, genomes: object, KEGG: object, regulations: object, lociAsLocus_tag?: boolean, logfile?: string): object;
   }
}
