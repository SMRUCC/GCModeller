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
   */
   function effect_decomposition(sem_result: object, as_dataframe?: boolean): object|object;
   /**
     * @param as_dataframe default value Is ``true``.
   */
   function indirect_effect(sem_result: object, boot_result: object, as_dataframe?: boolean): object|object;
   /**
     * @param env default value Is ``null``.
   */
   function make_path(from: any, to: any, env?: object): object;
   /**
     * @param as_dataframe default value Is ``true``.
   */
   function path_coefficient(sem_result: object, as_dataframe?: boolean): object|object;
   /**
     * @param boot default value Is ``500``.
     * @param env default value Is ``null``.
   */
   function sem(model: object, boot?: object, env?: object): object;
   /**
     * @param as_dataframe default value Is ``true``.
   */
   function significance_test(sem_result: object, boot_result: object, as_dataframe?: boolean): object|object;
}
