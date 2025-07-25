﻿// export R# package module type define for javascript/typescript language
//
//    imports "modeller" from "vcellkit";
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
      */
      function json_model(file: string): object;
      /**
       * read the virtual cell model file
       * 
       * 
        * @param path the model file extension could be:
        *  
        *  xml - small virtual cell model in a xml file
        *  zip - large virtual cell model file save as multiple components in a zip file
        *  json - large virtual cell model file save as json stream file
      */
      function vcell(path: string): object;
   }
   module write {
      /**
       * save the virtual cell model as a large json file
       * 
       * 
        * @param vcell -
        * @param file -
        * @param indent 
        * + default value Is ``true``.
      */
      function json_model(vcell: object, file: string, indent?: boolean): boolean;
      /**
       * save the virtual cell model as zip archive file
       * 
       * 
        * @param vcell -
        * @param file -
      */
      function zip(vcell: object, file: string): boolean;
   }
}
