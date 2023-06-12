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
        * @param env 
        * + default value Is ``null``.
      */
      function seqPack(mat: object, briefSet?: boolean, custom?: string, env?: object): any;
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
        * @param axis default value ``1`` means apply the cutoff for each sample column data,
        *  alternative value ``2`` means apply the cutoff for each gene data row.
        * 
        * + default value Is ``1``.
      */
      function apply(mat: object, q?: number, axis?: object): object;
   }
}
