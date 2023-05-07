// export R# package module type define for javascript/typescript language
//
// ref=phenotype_kit.magnitude

/**
 * HTS expression data simulating for analysis test
 * 
*/
declare namespace magnitude {
   /**
     * @param base default value is ``null``.
     * @param env default value is ``null``.
   */
   function profiles(selector:string, foldchange:number, base?:object, env?:object): any;
}
