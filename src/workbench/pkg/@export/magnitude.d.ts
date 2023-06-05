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
      */
      function seqPack(mat: object, briefSet?: boolean): any;
   }
}
