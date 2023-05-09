// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.Simulator@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace simulator {
   module apply {
      /**
        * @param system default value Is ``null``.
      */
      function module_profile(engine:object, profile:object, system?:object): object;
   }
   module dynamics {
      /**
       * Create the default cell dynamics parameters
       * 
       * 
      */
      function default(): object;
   }
   module engine {
      /**
       * create a new virtual cell engine
       * 
       * 
        * @param vcell -
        * @param inits -
        * 
        * + default value Is ``null``.
        * @param iterations 
        * + default value Is ``100``.
        * @param time_resolutions 
        * + default value Is ``10000``.
        * @param deletions 
        * + default value Is ``null``.
        * @param dynamics -
        * 
        * + default value Is ``null``.
        * @param showProgress 
        * + default value Is ``true``.
      */
      function load(vcell:object, inits?:object, iterations?:object, time_resolutions?:object, deletions?:string, dynamics?:object, showProgress?:boolean): object;
   }
   /**
   */
   function mass0(vcell:object): object;
   module vcell {
      module flux {
         /**
         */
         function index(vcell:object): object;
      }
      module mass {
         /**
         */
         function index(vcell:object): object;
         /**
          * Create a new status profile data object with unify mass contents.
          * 
          * 
           * @param vcell -
           * @param mass 
           * + default value Is ``5000``.
         */
         function kegg(vcell:object, mass?:number): object;
      }
      /**
       * create a generic vcell object model from a loaded vcell xml file model
       * 
       * 
        * @param vcell the file model data of the GCModeller vcell
      */
      function model(vcell:object): object;
      /**
      */
      function snapshot(engine:object, massIndex:object, fluxIndex:object, save:string): ;
   }
}
