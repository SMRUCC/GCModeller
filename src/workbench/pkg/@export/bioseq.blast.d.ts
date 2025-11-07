// export R# package module type define for javascript/typescript language
//
//    imports "bioseq.blast" from "seqtoolkit";
//
// ref=seqtoolkit.Blast@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Blast search tools
 * 
*/
declare namespace bioseq.blast {
   module align {
      /**
      */
      function gwANI(multipleSeq: object): object;
      /**
       * Do sequence global pairwise alignment
       * 
       * 
        * @param query -
        * @param ref -
      */
      function needleman_wunsch(query: object, ref: object): object;
      /**
       * Do sequence pairwise alignment
       * 
       * 
        * @param query -
        * @param ref -
        * @param blosum -
        * 
        * + default value Is ``null``.
      */
      function smith_waterman(query: object, ref: object, blosum?: object): object;
   }
   /**
    * Parse blosum from the given file data
    * 
    * 
     * @param file The blosum text data or text file path.
     * 
     * + default value Is ``'Blosum-62'``.
   */
   function blosum(file?: string): object;
   /**
    * get the high score region from the given alignment result
    * 
    * 
     * @param align -
     * @param cutoff [0,1] threshold
     * @param minW -
   */
   function HSP(align: object, cutoff: number, minW: object): any;
}
