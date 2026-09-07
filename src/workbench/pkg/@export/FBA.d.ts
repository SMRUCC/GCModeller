// export R# package module type define for javascript/typescript language
//
//    imports "FBA" from "biosystem";
//
// ref=biosystem.FBA@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

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
     * @return a tuple list of the FBA lpp solver result:
     *  
     *  + objective, target objective function value
     *  + flux, the flux distribution result tuple list, key name is the flux id and the value is the flux value. 
     *  
     *  additional, the original .NET CLR @``T:Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming.LPPSolution`` object is attached inside the result object attribute ``lpp``, which could be get from the result object via ``attr(x)`` function..
   */
   function lpsolve(model: object): object;
   /**
    * create FBA model matrix
    * 
    * 
     * @param model should be a GCModeller @``T:SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2.VirtualCell`` or @``T:SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.CellularModule`` model object, or a collection of the kegg @``T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction``.
     * @param terms 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function matrix(model: any, terms?: any, env?: object): object;
   /**
    * set lpp objective targets for FBA analysis
    * 
    * 
     * @param matrix -
     * @param target should be a character vector of the target reaction id
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function objective(matrix: object, target: any, env?: object): object;
}
