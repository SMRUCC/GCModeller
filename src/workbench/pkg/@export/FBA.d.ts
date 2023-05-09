// export R# package module type define for javascript/typescript language
//
// ref=simulators.FBA@simulators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Flux Balance Analysis
 * 
*/
declare namespace FBA {
   /**
    * create FBA model matrix
    * 
    * 
     * @param model -
     * @param terms 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function matrix(model: any, terms?: string, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function objective(matrix: object, target: any, env?: object): object;
   /**
     * @param name default value Is ``'Flux Balance Analysis LppModel'``.
   */
   function lppModel(model: object, name?: string): object;
   /**
    * Solve a FBA matrix model
    * 
    * 
     * @param model -
   */
   function lpsolve(model: object): any;
}
