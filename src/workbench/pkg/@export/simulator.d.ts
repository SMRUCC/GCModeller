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
        * @param deletions make a specific gene nodes deletions
        * 
        * + default value Is ``null``.
        * @param dynamics -
        * 
        * + default value Is ``null``.
        * @param showProgress 
        * + default value Is ``true``.
        * @param debug 
        * + default value Is ``false``.
      */
      function load(vcell: object, inits?: object, iterations?: object, time_resolutions?: object, deletions?: string, dynamics?: object, showProgress?: boolean, debug?: boolean): object;
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
     * @return A mass environment for run vcell model in GCModeller
   */
   function mass0(vcell: object): object;
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
      */
      function model(vcell: object): object;
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
