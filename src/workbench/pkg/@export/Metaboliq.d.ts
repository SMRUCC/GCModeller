// export R# package module type define for javascript/typescript language
//
//    imports "Metaboliq" from "biosystem";
//
// ref=biosystem.MetaboliqTool@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Liquid In-silico Metabolic Network
 * 
*/
declare namespace Metaboliq {
   /**
    * make liquid network training
    * 
    * 
     * @param model -
     * @param observed -
     * @param enzymeSeries -
     * @param boundarySeries -
     * @param fluxTruth -
     * @return get training loss @``T:SMRUCC.genomics.Analysis.Metaboliq.EpochLoss`` vector data via attr(x, "loss")
   */
   function fit(model: object, config: object, times: any, observed: object, enzymeSeries: object, boundarySeries: object, fluxTruth: object): object;
   /**
     * @param mode default value Is ``null``.
     * @param solver default value Is ``'rk4'``.
   */
   function new(graph: object, mode?: object, solver?: string): object;
   /**
   */
   function predict(model: object, h0: object, times: any, enzymeSeries: object, boundarySeries: object): object;
   /**
     * @param explicit_boundary default value Is ``null``.
   */
   function read_metabolic_graph(file: string, explicit_boundary?: any): object;
   /**
    * read metabolome/enzyme expression time serials data matrix
    * 
    * 
     * @param file -
   */
   function read_timedata(file: string): object;
}
