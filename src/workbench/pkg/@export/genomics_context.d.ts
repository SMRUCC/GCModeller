// export R# package module type define for javascript/typescript language
//
//    imports "genomics_context" from "seqtoolkit";
//
// ref=seqtoolkit.context@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the tools for processing of the genomics context information
 * 
*/
declare namespace genomics_context {
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
   function context(loci: any, distance: object, note?: string, env?: object): object;
   /**
    * filter genes by given strand direction
    * 
    * 
     * @param genes a collection of the gene model object which is subclass of @``T:SMRUCC.genomics.ComponentModel.Annotation.IGeneBrief``
     * @param strand the nucleotide sequence strand direction, value could be +, -, forward, reverse.
     * 
     * + default value Is ``'+'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filter_strand(genes: any, strand?: any, env?: object): any;
   module is {
      /**
       * assert that does the given nucleotide location is in forward direction?
       * 
       * 
        * @param loci a target nucleotide location
      */
      function forward(loci: object): boolean;
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
   function location(left: object, right: object, strand?: any): any;
   /**
    * do offset of the given location
    * 
    * 
     * @param loci -
     * @param offset -
   */
   function offset(loci: object, offset: object): object;
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
   function relationship(a: any, b: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function set_context(sites: any, genomics: object, env?: object): any;
}
