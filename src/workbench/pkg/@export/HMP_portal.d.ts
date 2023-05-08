// export R# package module type define for javascript/typescript language
//
// ref=metagenomics_kit.HMP@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * An internal ``HMP`` client for download data files from ``https://portal.hmpdacc.org/`` website
 * 
*/
declare namespace HMP_portal {
   /**
    * run file downloads
    * 
    * 
     * @param files -
     * @param outputdir -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function fetch(files:any, outputdir:string, env?:object): any;
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function manifest(file:string, env?:object): object;
   }
}
