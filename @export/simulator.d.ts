// export R# package module type define for javascript/typescript language
//
//    imports "simulator" from "vcellkit";
//
// ref=vcellkit.Simulator@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace simulator {
   module apply {
      /**
        * @param system default value Is ``null``.
      */
      function module_profile(engine: object, profile: object, system?: object): object;
   }
   /**
   */
   function attach_memorydataset(engine: object): any;
   module dynamics {
      /**
      */
      function default(): object;
   }
   module engine {
      /**
        * @param inits default value Is ``null``.
        * @param iterations default value Is ``100``.
        * @param time_resolutions default value Is ``10000``.
        * @param dynamics default value Is ``null``.
        * @param showProgress default value Is ``true``.
        * @param unit_test default value Is ``false``.
        * @param debug default value Is ``false``.
      */
      function load(vcell: object, inits?: object, iterations?: object, time_resolutions?: object, dynamics?: object, showProgress?: boolean, unit_test?: boolean, debug?: boolean): object;
   }
   /**
     * @param mass default value Is ``5000``.
   */
   function kegg_mass(vcell: object, mass?: number): object;
   /**
     * @param random default value Is ``null``.
     * @param map default value Is ``["kegg","metacyc"]``.
     * @param env default value Is ``null``.
   */
   function mass0(vcell: object, random?: any, map?: any, env?: object): object;
   /**
     * @param mass default value Is ``5000``.
   */
   function metacyc_mass(vcell: object, mass?: number): object;
   /**
   */
   function run(engine: object): any;
   /**
     * @param env_set default value Is ``null``.
   */
   function set_status(def: object, env_set?: object): object;
   module vcell {
      module flux {
         /**
         */
         function index(vcell: object): object;
      }
      module mass {
         /**
         */
         function index(vcell: object): object;
      }
      /**
        * @param unit_test default value Is ``false``.
      */
      function model(vcell: object, unit_test?: boolean): object;
      /**
      */
      function snapshot(engine: object, massIndex: object, fluxIndex: object, save: string): ;
   }
}
