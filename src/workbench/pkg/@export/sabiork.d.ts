declare namespace sabiork {
   /**
   */
   function new(file:string): any;
   /**
   */
   function open(file:string): any;
   /**
   */
   function query(ec_number:string, cache:object): any;
   /**
   */
   function get_kineticis(cache:object, ec_number:string): any;
   /**
   */
   function parseSbml(data:string): any;
}
