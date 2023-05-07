// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.Compiler

/**
 * The GCModeller virtual cell model creator
 * 
*/
declare namespace compiler {
   /**
   */
   function kegg(compounds:string, maps:string, reactions:string, glycan2Cpd:object): any;
   module geneKO {
      /**
        * @param KOcol default value is ``'KO'``.
        * @param geneIDcol default value is ``'ID'``.
        * @param env default value is ``null``.
      */
      function maps(data:any, KOcol?:string, geneIDcol?:string, env?:object): any;
   }
   module assembling {
      /**
        * @param lociAsLocus_tag default value is ``false``.
      */
      function genome(replicons:object, geneKO:object, lociAsLocus_tag?:boolean): any;
      /**
      */
      function metabolic(cell:object, geneKO:object, repo:object): any;
      /**
      */
      function TRN(model:object, regulations:object): any;
   }
   module vcell {
      /**
        * @param lociAsLocus_tag default value is ``false``.
        * @param logfile default value is ``null``.
      */
      function markup(model:object, genomes:object, KEGG:object, regulations:object, lociAsLocus_tag?:boolean, logfile?:string): any;
   }
   module compile {
      /**
        * @param logfile default value is ``'./gcc.log'``.
      */
      function biocyc(biocyc:object, genomes:object, logfile?:string): any;
   }
}
