// export R# package module type define for javascript/typescript language
//
//    imports "sabiork" from "biosystem";
//
// ref=biosystem.sabiork_repository@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * sabio-rk data repository
 * 
*/
declare namespace sabiork {
   /**
   */
   function get_kineticis(cache: object, ec_number: string): object;
   /**
   */
   function new(file: string): object;
   /**
   */
   function open(file: string): object;
   /**
   */
   function parseSbml(data: string): object;
   /**
   */
   function query(ec_number: string, cache: object): object;
   /**
    * Create a helper reader for load element model from the sbml document
    * 
    * 
     * @param sbml -
   */
   function sbmlReader(sbml: object): object;
}
