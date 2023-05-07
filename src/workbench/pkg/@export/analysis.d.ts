// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.Analysis@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * GCModeller virtual cell analysis toolkit.
 * 
*/
declare namespace analysis {
   module union {
      /**
       * union of the profile snapshot list to a matrix dataset.
       * 
       * 
        * @param result -
        * @param setName -
        * 
        * + default value Is ``'mass\metabolome.json'``.
        * @param trim Will delete all of the metabolites row that have no changes
        * 
        * + default value Is ``true``.
      */
      function matrix(result:string, setName?:string, trim?:boolean): object;
   }
   module compound {
      /**
       * set compound names for the kegg metabolites
       * 
       * 
        * @param metabolites -
        * @param names -
      */
      function names(metabolites:object, names:object): object;
   }
   module vcell {
      module mass {
         /**
           * @param env default value Is ``null``.
         */
         function graph(vcell:any, env?:object): object;
      }
   }
}
