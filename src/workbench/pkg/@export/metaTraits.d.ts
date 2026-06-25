// export R# package module type define for javascript/typescript language
//
//    imports "metaTraits" from "metagenomics_kit";
//
// ref=metagenomics_kit.metaTraitsTool@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace metaTraits {
   module load {
      /**
        * @param env default value Is ``null``.
      */
      function meta_traits(file: string, env?: object): object;
      /**
      */
      function trait_models(repo: string): object;
   }
   /**
     * @param bit_score default value Is ``25``.
     * @param evalue default value Is ``0.01``.
     * @param env default value Is ``null``.
   */
   function make_predicts(models: object, genome: any, bit_score?: number, evalue?: number, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function phenotype_result(predicts: any, models: object, env?: object): object;
}
