// export R# package module type define for javascript/typescript language
//
//    imports "TRN.builder" from "TRNtoolkit";
//
// ref=TRNtoolkit.TRNBuilder@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for create a transcription regulation network
 * 
*/
declare namespace TRN.builder {
   /**
     * @param family default value Is ``null``.
     * @param identities_cutoff default value Is ``0.8``.
     * @param minW default value Is ``0.85``.
     * @param top default value Is ``3``.
     * @param permutation default value Is ``2500``.
     * @param tqdm_bar default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function motif_search(db: object, search_regions: any, family?: any, identities_cutoff?: number, minW?: number, top?: object, permutation?: object, tqdm_bar?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function open_motifdb(file: any, env?: object): object;
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
