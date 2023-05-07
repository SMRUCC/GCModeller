// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.vcellModeller

/**
 * virtual cell network kinetics modeller
 * 
*/
declare namespace modeller {
   module apply {
      /**
        * @param cache default value is ``'./.cache'``.
      */
      function kinetics(vcell:object, cache?:string): any;
   }
   module cacheOf {
      /**
        * @param export default value is ``'./'``.
        * @param ko01000 default value is ``'ko01000'``.
      */
      function enzyme_kinetics(export?:string, ko01000?:string): any;
   }
   module read {
      /**
      */
      function vcell(path:string): any;
   }
   /**
   */
   function zip(vcell:object, file:string): any;
   /**
     * @param env default value is ``null``.
   */
   function kinetics(expr:string, parameters:object, env?:object): any;
   /**
   */
   function kinetics_lambda(kinetics:object): any;
   /**
     * @param args default value is ``null``.
     * @param env default value is ``null``.
   */
   function eval_lambda(kinetics:object, args?:object, env?:object): any;
}
