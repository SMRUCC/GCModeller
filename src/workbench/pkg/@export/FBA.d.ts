// export R# package module type define for javascript/typescript language
//
//    imports "FBA" from "simulators";
//
// ref=simulators.FBA@simulators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Flux Balance Analysis
 * 
*/
declare namespace FBA {
   /**
    * convert the flux matrix as the general Linear Programming model
    * 
    * > the flux matrix encoded as the general lpp model via:
    * >  
    * >  1. mapping the flux as the @``P:Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming.LPPModel.variables``
    * >  2. mapping the compound and flux coefficient factor as the @``P:Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming.LPPModel.constraintCoefficients`` data.
    * 
     * @param model -
     * @param name -
     * 
     * + default value Is ``'Flux Balance Analysis LppModel'``.
     * @return a general Linear Programming model
   */
   function lppModel(model: object, name?: string): object;
   /**
    * Solve a FBA matrix model
    * 
    * 
     * @param model -
   */
   function lpsolve(model: object): any;
   /**
    * create FBA model matrix
    * 
    * 
     * @param model should be a GCModeller virtual cell @``T:SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.CellularModule`` model object.
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
}
