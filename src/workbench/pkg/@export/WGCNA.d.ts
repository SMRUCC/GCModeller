// export R# package module type define for javascript/typescript language
//
//    imports "WGCNA" from "TRNtoolkit";
//
// ref=TRNtoolkit.WGCNA@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace WGCNA {
   /**
     * @param threshold default value Is ``0.3``.
   */
   function interations(g: object, WGCNA: object, modules: object, threshold?: number): any;
   /**
     * @param threshold default value Is ``0.3``.
   */
   function shapeTRN(g: object, WGCNA: object, threshold?: number): any;
}
