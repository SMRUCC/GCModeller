// export R# package module type define for javascript/typescript language
//
// ref=simulators.FBA

/**
 * Flux Balance Analysis
 * 
*/
declare namespace FBA {
   /**
     * @param terms default value is ``null``.
     * @param env default value is ``null``.
   */
   function matrix(model:any, terms?:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function objective(matrix:object, target:any, env?:object): any;
   /**
     * @param name default value is ``'Flux Balance Analysis LppModel'``.
   */
   function lppModel(model:object, name?:string): any;
   /**
   */
   function lpsolve(model:object): any;
}
