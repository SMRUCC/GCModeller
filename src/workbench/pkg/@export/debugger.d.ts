// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.Debugger@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace debugger {
   module vcell {
      /**
      */
      function summary(inits:object, model:object, dir:string): ;
   }
   module map {
      /**
       * create dynamics model from a kegg pathway map
       * 
       * 
        * @param map -
        * @param reactions -
        * @param init -
        * 
        * + default value Is ``1000``.
      */
      function flux(map:object, reactions:object, init?:number): object;
   }
   module flux {
      /**
        * @param time default value Is ``50``.
        * @param resolution default value Is ``10000``.
        * @param showProgress default value Is ``true``.
      */
      function dynamics(core:object, time?:object, resolution?:object, showProgress?:boolean): object;
      /**
      */
      function load_driver(core:object, mass:object, flux:object): object;
   }
   /**
    * run network dynamics
    * 
    * 
     * @param network the target network graph model
     * @param init0 the system initial conditions
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function test_network(network:any, init0:object, env?:object): object;
}
