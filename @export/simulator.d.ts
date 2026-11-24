// export R# package module type define for javascript/typescript language
//
//    imports "simulator" from "vcellkit";
//
// ref=vcellkit.Simulator@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the GCModeller bio-system simulator
 * 
*/
declare namespace simulator {
   module apply {
      /**
       * 
       * 
        * @param engine -
        * @param profile -
        * @param system the omics data type
        * 
        * + default value Is ``null``.
      */
      function module_profile(engine: object, profile: object, system?: object): object;
   }
   /**
   */
   function attach_memorydataset(engine: object): any;
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
        * @param vcell The virtual cell object model, contains the definition of the cellular network graph data
        * @param inits the initial mass environment definition
        * 
        * + default value Is ``null``.
        * @param iterations the number of the iteration loops for run the simulation
        * 
        * + default value Is ``100``.
        * @param time_resolutions the time steps
        * 
        * + default value Is ``10000``.
        * @param dynamics -
        * 
        * + default value Is ``null``.
        * @param showProgress 
        * + default value Is ``true``.
        * @param unit_test 
        * + default value Is ``false``.
        * @param debug 
        * + default value Is ``false``.
      */
      function load(vcell: object, inits?: object, iterations?: object, time_resolutions?: object, dynamics?: object, showProgress?: boolean, unit_test?: boolean, debug?: boolean): object;
   }
   /**
    * Create a new status profile data object with unify mass contents.
    * 
    * > this function works for the data model which is based on the kegg database model
    * 
     * @param vcell -
     * @param mass -
     * 
     * + default value Is ``5000``.
   */
   function kegg_mass(vcell: object, mass?: number): object;
   /**
    * get the initial mass value
    * 
    * 
     * @param vcell the initialize mass value has been defined inside this virtual cell model
     * @param random set random to the molecules, should be a numeric vector that consist with two number as [min, max]. 
     *  both min and max should be positive value.
     * 
     * + default value Is ``null``.
     * @param map 
     * + default value Is ``["kegg","metacyc"]``.
     * @param env 
     * + default value Is ``null``.
     * @return A mass environment for run vcell model in GCModeller
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
    * set the omics data from this function
    * 
    * 
     * @param def -
     * @param env_set -
     * 
     * + default value Is ``null``.
   */
   function set_status(def: object, env_set?: object): object;
   module vcell {
      module flux {
         /**
          * get flux key reference index collection
          * 
          * 
           * @param vcell -
         */
         function index(vcell: object): object;
      }
      module mass {
         /**
          * get mass key reference index collection
          * 
          * 
           * @param vcell -
         */
         function index(vcell: object): object;
      }
      /**
       * create a generic vcell object model from a loaded vcell xml file model
       * 
       * 
        * @param vcell the file model data of the GCModeller vcell
        * @param unit_test 
        * + default value Is ``false``.
      */
      function model(vcell: object, unit_test?: boolean): object;
      /**
       * make a snapshot of the mass and flux data
       * 
       * 
        * @param engine -
        * @param massIndex -
        * @param fluxIndex -
      */
      function snapshot(engine: object, massIndex: object, fluxIndex: object, save: string): ;
   }
}
