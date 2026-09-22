// export R# package module type define for javascript/typescript language
//
//    imports "bioseq.blast" from "seqtoolkit";
//
// ref=seqtoolkit.Blast@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace bioseq.blast {
   module align {
      /**
      */
      function gwANI(multipleSeq: object): object;
      /**
      */
      function needleman_wunsch(query: object, ref: object): object;
      /**
        * @param blosum default value Is ``null``.
      */
      function smith_waterman(query: object, ref: object, blosum?: object): object;
   }
   /**
     * @param file default value Is ``'Blosum-62'``.
   */
   function blosum(file?: string): object;
   /**
     * @param as_dataframe default value Is ``true``.
   */
   function HSP(align: object, cutoff: number, minW: object, as_dataframe?: boolean): object|object;
}
