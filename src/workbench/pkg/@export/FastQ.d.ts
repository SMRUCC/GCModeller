// export R# package module type define for javascript/typescript language
//
//    imports "FastQ" from "rnaseq";
//
// ref=rnaseq.FastQ@rnaseq, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

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
     * @param reads -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function assemble(reads: any, env?: object): any;
   /**
    * In FASTQ files, quality scores are encoded into a compact form, 
    *  which uses only 1 byte per quality value. In this encoding, the 
    *  quality score is represented as the character with an ASCII 
    *  code equal to its value + 33.
    * 
    * 
     * @param q -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function quality_score(q: any, env?: object): number;
}
