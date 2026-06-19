// export R# package module type define for javascript/typescript language
//
//    imports "IMMO" from "phenotype_kit";
//
// ref=phenotype_kit.IMMOTool@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * IMMO (Integration Model for Incomplete Multi-Omics)
 * 
*/
declare namespace IMMO {
   /**
   */
   function latent_data(model: object, preparedData: object): object;
   /**
   */
   function make_omics_impute(model: object, preparedData: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function prepared_data(data: object, env?: object): object;
   /**
   */
   function reconstruct(model: object, preparedData: object): object;
   /**
     * @param config default value Is ``null``.
   */
   function train(preparedData: object, config?: object): object;
}
