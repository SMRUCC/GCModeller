declare namespace OBO {
   module read {
      /**
      */
      function obo(path:string): any;
   }
   module filter {
      /**
      */
      function is_obsolete(obo:object): any;
   }
   /**
   */
   function filter_properties(obo:object, excludes:string): any;
   module write {
      /**
        * @param excludes default value is ``null``.
      */
      function obo(obo:object, path:string, excludes?:string): any;
   }
}
