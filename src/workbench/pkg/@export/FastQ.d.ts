// export R# package module type define for javascript/typescript language
//
//    imports "FastQ" from "rnaseq";
//
// ref=rnaseq.FastQTools@rnaseq, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * FastQ toolkit
 * 
 * > FASTQ format Is a text-based format For storing both a biological sequence 
 * >  (usually nucleotide sequence) And its corresponding quality scores. Both 
 * >  the sequence letter And quality score are Each encoded With a Single ASCII 
 * >  character For brevity. It was originally developed at the Wellcome Trust 
 * >  Sanger Institute To bundle a FASTA formatted sequence And its quality data, 
 * >  but has recently become the de facto standard For storing the output Of 
 * >  high-throughput sequencing instruments such As the Illumina Genome 
 * >  Analyzer.
*/
declare namespace FastQ {
   /**
    * Do short reads assembling
    * 
    * 
     * @param reads should be a set of the sequence data, example as a collection of @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` data.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return the short reads assembling result
   */
   function assemble(reads: any, env?: object): object;
   /**
   */
   function illumina_fastQ_id(fq: object): object;
   /**
    * In FASTQ files, quality scores are encoded into a compact form, 
    *  which uses only 1 byte per quality value. In this encoding, the 
    *  quality score is represented as the character with an ASCII 
    *  code equal to its value + 33.
    * 
    * 
     * @param q should be one or more @``T:SMRUCC.genomics.SequenceModel.FQ.FastQ`` sequence data
     * @param env -
     * 
     * + default value Is ``null``.
     * @return the quality score data of each @``T:SMRUCC.genomics.SequenceModel.FQ.FastQ`` sequence data.
   */
   function quality_score(q: any, env?: object): number;
   module read {
      /**
       * read the fastq file
       * 
       * 
        * @param file -
      */
      function fastq(file: string): object;
   }
   /**
     * @param n default value Is ``100000``.
     * @param len default value Is ``[200,350]``.
     * @param genome_weights default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function simulate_reads(genomes: any, n?: object, len?: any, genome_weights?: object, env?: object): object;
   module write {
      /**
        * @param env default value Is ``null``.
      */
      function fastq(reads: any, file: string, env?: object): boolean;
   }
}
