// export R# package module type define for javascript/typescript language
//
//    imports "regprecise" from "TRNtoolkit";
//
// ref=TRNtoolkit.RegPrecise@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace regprecise {
   /**
     * @param env default value Is ``null``.
   */
   function join(blast: any, regulators: any, env?: object): object;
   module motif {
      /**
      */
      function raw(regprecise: object): object;
   }
   module read {
      /**
      */
      function motifs(file: string): object;
      /**
      */
      function operon(file: string): object;
      /**
      */
      function regprecise(file: string): object;
      /**
      */
      function regulators(file: string): object;
      /**
      */
      function regulome(xml: string): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function regulators(regulome: object, info: object, env?: object): object;
}
