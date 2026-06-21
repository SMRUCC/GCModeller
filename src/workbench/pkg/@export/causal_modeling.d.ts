// export R# package module type define for javascript/typescript language
//
//    imports "causal_modeling" from "phenotype_kit";
//
// ref=phenotype_kit.causal_modeling@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace causal_modeling {
   /**
     * @param env default value Is ``null``.
   */
   function as_causalmodel(data: any, path: any, env?: object): object;
   /**
     * @param as_dataframe default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function effect_decomposition(result: any, as_dataframe?: boolean, env?: object): object|object;
   /**
     * @param as_dataframe default value Is ``true``.
   */
   function endogenous_latents(result: object, as_dataframe?: boolean): object|object;
   /**
     * @param as_dataframe default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function indirect_effect(result: any, boot_result: object, as_dataframe?: boolean, env?: object): object|object;
   /**
     * @param mode default value Is ``null``.
   */
   function make_latent(latentName: string, manifestNames: any, mode?: object): object;
   /**
     * @param paths default value Is ``null``.
     * @param from default value Is ``null``.
     * @param to default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function make_path(paths?: object, from?: any, to?: any, env?: object): object;
   /**
     * @param as_dataframe default value Is ``true``.
   */
   function measurement_model(result: object, as_dataframe?: boolean): object|object;
   /**
     * @param as_dataframe default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function path_coefficient(result: any, as_dataframe?: boolean, env?: object): object|object;
   /**
     * @param boot default value Is ``500``.
     * @param env default value Is ``null``.
   */
   function plspm(model: object, boot?: object, env?: object): object;
   /**
     * @param boot default value Is ``500``.
     * @param env default value Is ``null``.
   */
   function sem(model: object, boot?: object, env?: object): object;
   /**
     * @param as_dataframe default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function significance_test(result: any, boot_result: object, as_dataframe?: boolean, env?: object): object|object;
}
