// export R# package module type define for javascript/typescript language
//
// ref=visualkit.WGCNA@visualkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace WGCNA {
   /**
    * append protein iteration network based on the WGCNA weights.
    * 
    * 
     * @param g -
     * @param WGCNA -
     * @param modules -
     * @param threshold -
     * 
     * + default value Is ``0.3``.
   */
   function interations(g: object, WGCNA: object, modules: object, threshold?: number): any;
   /**
    * filter regulation network by WGCNA result weights
    * 
    * 
     * @param g -
     * @param WGCNA -
     * @param threshold -
     * 
     * + default value Is ``0.3``.
   */
   function shapeTRN(g: object, WGCNA: object, threshold?: number): any;
}
