declare namespace bioseq.blast {
   /**
     * @param file default value is ``'Blosum-62'``.
   */
   function blosum(file?:string): any;
   module align {
      /**
        * @param blosum default value is ``null``.
      */
      function smith_waterman(query:object, ref:object, blosum?:object): any;
      /**
      */
      function needleman_wunsch(query:object, ref:object): any;
      /**
      */
      function gwANI(multipleSeq:object): any;
   }
   /**
   */
   function HSP(align:object, cutoff:number, minW:object): any;
}
