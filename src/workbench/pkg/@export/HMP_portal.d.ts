// export R# package module type define for javascript/typescript language
//
// ref=metagenomics_kit.HMP

/**
 * An internal ``HMP`` client for download data files from ``https://portal.hmpdacc.org/`` website
 * 
*/
declare namespace HMP_portal {
   /**
     * @param env default value is ``null``.
   */
   function fetch(files:any, outputdir:string, env?:object): any;
   module read {
      /**
        * @param env default value is ``null``.
      */
      function manifest(file:string, env?:object): any;
   }
}
