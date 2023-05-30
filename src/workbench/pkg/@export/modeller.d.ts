// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.vcellModeller@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * virtual cell network kinetics modeller
 * 
*/
declare namespace modeller {
   module apply {
      /**
       * apply the kinetics parameters from the sabio-rk database.
       * 
       * 
        * @param vcell -
        * @param cache 
        * + default value Is ``'./.cache'``.
      */
      function kinetics(vcell: object, cache?: string): object;
   }
   module cacheOf {
      /**
       * create data repository from the sabio-rk database
       * 
       * 
        * @param export 
        * + default value Is ``'./'``.
        * @param ko01000 
        * + default value Is ``'ko01000'``.
      */
      function enzyme_kinetics(export?: string, ko01000?: string): ;
   }
   /**
     * @param args default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function eval_lambda(kinetics: object, args?: object, env?: object): number;
   /**
    * create dynamics kinetics
    * 
    * 
     * @param expr -
     * @param parameters -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function kinetics(expr: string, parameters: object, env?: object): object;
   /**
   */
   function kinetics_lambda(kinetics: object): object;
   module read {
      /**
       * read the virtual cell model file
       * 
       * 
        * @param path -
      */
      function vcell(path: string): object;
   }
   /**
   */
   function zip(vcell: object, file: string): boolean;
}
