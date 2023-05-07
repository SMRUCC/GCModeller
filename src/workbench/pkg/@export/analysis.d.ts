// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.Analysis

/**
 * GCModeller virtual cell analysis toolkit.
 * 
*/
declare namespace analysis {
   module union {
      /**
        * @param setName default value is ``'mass\metabolome.json'``.
        * @param trim default value is ``true``.
      */
      function matrix(result:string, setName?:string, trim?:boolean): any;
   }
   module compound {
      /**
      */
      function names(metabolites:object, names:object): any;
   }
   module vcell {
      module mass {
         /**
           * @param env default value is ``null``.
         */
         function graph(vcell:any, env?:object): any;
      }
   }
}
