// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.context@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace annotation.genomics_context {
   module strand {
      /**
       * filter genes by given strand direction
       * 
       * 
        * @param genes -
        * @param strand -
        * 
        * + default value Is ``'+'``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function filter(genes:any, strand?:any, env?:object): any;
   }
   /**
    * create a new nucleotide location object
    * 
    * 
     * @param left -
     * @param right -
     * @param strand -
     * 
     * + default value Is ``null``.
   */
   function location(left:object, right:object, strand?:any): any;
   module is {
      /**
       * the given nucleotide location is in forward direction
       * 
       * 
        * @param loci -
      */
      function forward(loci:object): boolean;
   }
   /**
    * do offset of the given location
    * 
    * 
     * @param loci -
     * @param offset -
   */
   function offset(loci:object, offset:object): object;
   /**
    * Create a new context model of a specific genomics feature site.
    * 
    * 
     * @param loci -
     * @param distance -
     * @param note -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function context(loci:any, distance:object, note?:string, env?:object): object;
   /**
    * get the segment relationship of two location
    * 
    * 
     * @param a -
     * @param b -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function relationship(a:any, b:any, env?:object): object;
}
