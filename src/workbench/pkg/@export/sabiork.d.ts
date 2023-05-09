// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.sabiork_repository@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace sabiork {
   /**
   */
   function get_kineticis(cache:object, ec_number:string): any;
   /**
   */
   function new(file:string): object;
   /**
   */
   function open(file:string): object;
   /**
   */
   function parseSbml(data:string): object;
   /**
   */
   function query(ec_number:string, cache:object): any;
}
