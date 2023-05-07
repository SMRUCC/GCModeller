declare namespace annotation.genomics_context {
   module strand {
      /**
        * @param strand default value is ``'+'``.
        * @param env default value is ``null``.
      */
      function filter(genes:any, strand?:any, env?:object): any;
   }
   /**
     * @param strand default value is ``null``.
   */
   function location(left:object, right:object, strand?:any): any;
   module is {
      /**
      */
      function forward(loci:object): any;
   }
   /**
   */
   function offset(loci:object, offset:object): any;
   /**
     * @param note default value is ``null``.
     * @param env default value is ``null``.
   */
   function context(loci:any, distance:object, note?:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function relationship(a:any, b:any, env?:object): any;
}
