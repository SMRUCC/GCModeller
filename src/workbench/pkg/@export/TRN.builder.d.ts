// export R# package module type define for javascript/typescript language
//
//    imports "TRN.builder" from "TRNtoolkit"
//
// ref=TRNtoolkit.TRNBuilder@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace TRN.builder {
   module read {
      /**
      */
      function footprints(file: string): object;
      /**
      */
      function regulations(file: string): object;
   }
   module regulation {
      /**
        * @param env default value Is ``null``.
      */
      function footprint(regulators: any, motifLocis: object, regprecise: object, env?: object): object;
   }
   module write {
      /**
        * @param env default value Is ``null``.
      */
      function regulations(regulationFootprints: any, file: string, env?: object): any;
   }
}
