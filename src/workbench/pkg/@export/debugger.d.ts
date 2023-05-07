declare namespace debugger {
   module vcell {
      /**
      */
      function summary(inits:object, model:object, dir:string): any;
   }
   module map {
      /**
        * @param init default value is ``1000``.
      */
      function flux(map:object, reactions:object, init?:number): any;
   }
   module flux {
      /**
        * @param time default value is ``50``.
        * @param resolution default value is ``10000``.
        * @param showProgress default value is ``true``.
      */
      function dynamics(core:object, time?:object, resolution?:object, showProgress?:boolean): any;
      /**
      */
      function load_driver(core:object, mass:object, flux:object): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function test_network(network:any, init0:object, env?:object): any;
}
