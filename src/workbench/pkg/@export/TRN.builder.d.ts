// export R# package module type define for javascript/typescript language
//
// ref=visualkit.TRNBuilder@visualkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for create a transcription regulation network
 * 
*/
declare namespace TRN.builder {
   module read {
      /**
       * read a footprint site model data file
       * 
       * 
        * @param file -
      */
      function footprints(file: string): object;
      /**
       * read a regulation prediction result file
       * 
       * 
        * @param file -
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
       * save the regulation network data file.
       * 
       * 
        * @param regulationFootprints -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function regulations(regulationFootprints: any, file: string, env?: object): any;
   }
}
