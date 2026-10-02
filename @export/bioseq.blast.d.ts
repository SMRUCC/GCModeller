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
       * run the genome-wide average nucleotide identity(gwANI) calculation for 
       *  a given nucleotide sequence collection
       * 
       * 
        * @param multipleSeq a nucleotide fasta sequence collection for run the gwANI calculation, 
        *  which can be a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) object or a collection of the 
        *  [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object.
        * @return a vector of the [DataSet](cref:T:Microsoft.VisualBasic.Data.Framework.IO.DataSet) gwANI calculation result: each 
        *  element contains the genome-wide average nucleotide identity value and 
        *  its details of one sequence pair in the input sequence collection.
      */
      function gwANI(multipleSeq: object): object;
      /**
       * do the global pairwise sequence alignment via the needleman-wunsch 
       *  algorithm
       * 
       * 
        * @param query a [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) sequence object of the query sequence.
        * @param ref a [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) sequence object of the reference subject 
        *  sequence.
        * @return a factor value that contains the global alignment result: the score 
        *  value is the global alignment score, and the value data is a vector of 
        *  the aligned sequence fragment object.
      */
      function needleman_wunsch(query: object, ref: object): object;
      /**
       * do the local pairwise sequence alignment via the smith-waterman 
       *  algorithm
       * 
       * 
        * @param query a [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) sequence object of the query sequence.
        * @param ref a [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) sequence object of the reference subject 
        *  sequence.
        * @param blosum the amino acid substitution score matrix for the alignment, which can 
        *  be created by the ``blosum`` api in this package module. If this 
        *  parameter is nothing, then the default ``Blosum-62`` score matrix will 
        *  be used.
        * 
        * + default value Is ``null``.
        * @return a [SmithWaterman](cref:T:SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment.SmithWaterman) local alignment result object, from which 
        *  the high score region details can be extracted via the ``HSP`` api.
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
     * @return a [Blosum](cref:T:SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment.Blosum) amino acid substitution score matrix object that 
     *  parsed from the given blosum text data. When this function is called 
     *  with no arguments, then the built-in ``Blosum-62`` score matrix will be 
     *  returned.
   */
   function blosum(file?: string): object;
   /**
    * get the high score region(HSP) from the given alignment result
    * 
    * 
     * @param align a [SmithWaterman](cref:T:SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment.SmithWaterman) local alignment result object, which is 
     *  the output of the ``align.smith_waterman`` api.
     * @param cutoff the similarity score cutoff threshold value in [0,1] for filter the 
     *  candidate HSP regions.
     * @param minW the minimum region size in chars of the candidate HSP regions, the HSP 
     *  region that its size is smaller than this threshold value will be 
     *  ignored.
     * @param as_dataframe cast the HSP result as a data frame object? if this parameter is FALSE, 
     *  then a vector of the [HSP](cref:T:SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment.HSP) object will be returned instead.
     * 
     * + default value Is ``true``.
     * @return a data frame object of the HSP alignment details(the columns are: 
     *  ``query``, ``subject``, ``query_length``, ``subject_length``, 
     *  ``length_query``, ``length_hit``, ``hsp_query``, ``hsp_subject``, 
     *  ``score`` and ``coverage``) when the ``as_dataframe`` parameter is TRUE, 
     *  or a vector of the [HSP](cref:T:SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment.HSP) object when this parameter is 
     *  FALSE.
   */
   function HSP(align: object, cutoff: number, minW: object, as_dataframe?: boolean): object|object;
}
