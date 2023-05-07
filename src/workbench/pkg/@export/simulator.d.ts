declare namespace simulator {
   module vcell {
      module mass {
         /**
           * @param mass default value is ``5000``.
         */
         function kegg(vcell:object, mass?:number): any;
         /**
         */
         function index(vcell:object): any;
      }
      /**
      */
      function model(vcell:object): any;
      module flux {
         /**
         */
         function index(vcell:object): any;
      }
      /**
      */
      function snapshot(engine:object, massIndex:object, fluxIndex:object, save:string): any;
   }
   /**
   */
   function mass0(vcell:object): any;
   module engine {
      /**
        * @param inits default value is ``null``.
        * @param iterations default value is ``100``.
        * @param time_resolutions default value is ``10000``.
        * @param deletions default value is ``null``.
        * @param dynamics default value is ``null``.
        * @param showProgress default value is ``true``.
      */
      function load(vcell:object, inits?:object, iterations?:object, time_resolutions?:object, deletions?:string, dynamics?:object, showProgress?:boolean): any;
   }
   module dynamics {
      /**
      */
      function default(): any;
   }
   module apply {
      /**
        * @param system default value is ``null``.
      */
      function module_profile(engine:object, profile:object, system?:object): any;
   }
}
