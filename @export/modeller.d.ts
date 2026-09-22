// export R# package module type define for javascript/typescript language
//
//    imports "modeller" from "vcellkit";
//
// ref=vcellkit.vcellModeller@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace modeller {
   module apply {
      /**
        * @param cache default value Is ``'./.cache'``.
      */
      function kinetics(vcell: object, cache?: string): object;
   }
   module cacheOf {
      /**
        * @param export default value Is ``'./'``.
        * @param ko01000 default value Is ``'ko01000'``.
      */
      function enzyme_kinetics(export?: string, ko01000?: string): ;
   }
   /**
     * @param args default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function eval_lambda(kinetics: object, args?: object, env?: object): number;
   /**
     * @param env default value Is ``null``.
   */
   function kinetics(expr: string, parameters: object, env?: object): object;
   /**
   */
   function kinetics_lambda(kinetics: object): object;
   module read {
      /**
      */
      function json_model(file: string): object;
      /**
      */
      function vcell(path: string): object;
   }
   /**
   */
   function taxonomy_info(model: object): object;
   module write {
      /**
        * @param indent default value Is ``true``.
      */
      function json_model(vcell: object, file: string, indent?: boolean): boolean;
      /**
      */
      function zip(vcell: object, file: string): boolean;
   }
}
