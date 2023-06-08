// export R# package module type define for javascript/typescript language
//
//    imports "magnitude" from "phenotype_kit";
//
// ref=phenotype_kit.magnitude@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * HTS expression data simulating for analysis test
 * 
*/
declare namespace magnitude {
   module encode {
      /**
       * tag samples in matrix as sequence profiles
       * 
       * 
        * @param mat -
        * @param briefSet 
        * + default value Is ``true``.
        * @param custom use the custom charset, then the generated sequence
        *  data can only be processed via the SGT algorithm
        * 
        * + default value Is ``null``.
      */
      function seqPack(mat: object, briefSet?: boolean, custom?: string): any;
   }
   module TrIQ {
      /**
       * Apply TrIQ cutoff for each sample
       * 
       * 
        * @param mat -
        * @param q -
        * 
        * + default value Is ``0.8``.
      */
      function apply(mat: object, q?: number): object;
   }
}
