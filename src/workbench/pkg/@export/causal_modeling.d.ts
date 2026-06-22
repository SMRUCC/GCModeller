// export R# package module type define for javascript/typescript language
//
//    imports "causal_modeling" from "phenotype_kit";
//
// ref=phenotype_kit.causal_modeling@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace causal_modeling {
   /**
     * @param latents default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function as_causalmodel(data: any, path: any, latents?: any, env?: object): object;
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
    * make full connected path for PLS-PM latent symbols
    * 
    * 
     * @param manifest -
     * @param from class name of the from node
     * @param to class name of the to node
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function make_full_path(manifest: any, from: any, to: any, env?: object): any;
   /**
    * 
    * 
     * @param manifest_names a set of the feature id for make the manifest symbol to the latent definition or a vector of the @``T:SMRUCC.genomics.Analysis.Microarray.LatentSymbol``
     * @param latent_name -
     * 
     * + default value Is ``null``.
     * @param mode -
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function make_latent(manifest_names: any, latent_name?: string, mode?: object, env?: object): object;
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
     * @param target_manifests default value Is ``5``.
     * @param corr_thres default value Is ``0.8``.
     * @param mad_pool_size default value Is ``10``.
   */
   function reduce_manifest(manifest: object, target_manifests?: object, corr_thres?: number, mad_pool_size?: object): string;
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
