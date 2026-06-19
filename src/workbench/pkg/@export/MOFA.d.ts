// export R# package module type define for javascript/typescript language
//
//    imports "MOFA" from "phenotype_kit";
//
// ref=phenotype_kit.MOFATools@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace MOFA {
   /**
     * @param opts default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function create_mofa(data: object, opts?: object, env?: object): any;
   /**
   */
   function elbo_trace(model: object): any;
   /**
   */
   function reconstruct(model: object): object;
   /**
   */
   function run_mofa(model: object): object;
}
