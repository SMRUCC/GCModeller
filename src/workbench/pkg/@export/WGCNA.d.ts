// export R# package module type define for javascript/typescript language
//
//    imports "WGCNA" from "phenotype_kit";
//    imports "WGCNA" from "TRNtoolkit";
//
// ref=phenotype_kit.WGCNA@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=TRNtoolkit.WGCNA@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace WGCNA {
   /**
   */
   function applyModuleColors(g: object, modules: object): any;
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
   module read {
      /**
        * @param prefix default value Is ``null``.
      */
      function modules(file: string, prefix?: string): any;
      /**
        * @param threshold default value Is ``0``.
        * @param prefix default value Is ``null``.
      */
      function weightMatrix(file: string, threshold?: number, prefix?: string): object;
   }
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
