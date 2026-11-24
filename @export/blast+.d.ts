// export R# package module type define for javascript/typescript language
//
//    imports "blast+" from "seqtoolkit";
//
// ref=seqtoolkit.blastPlusInterop@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Basic Local Alignment Search Tool
 *  
 *  NCBI blast+ wrapper
 *  
 *  BLAST finds regions Of similarity between biological 
 *  sequences. The program compares nucleotide Or protein 
 *  sequences To sequence databases And calculates the 
 *  statistical significance.
 * 
*/
declare namespace blast_ {
   /**
    * Nucleotide-Nucleotide BLAST
    * 
    * > this api is not implemented at this moment.
    * 
   */
   function blastn(): any;
   /**
    * Protein-Protein BLAST
    * 
    * 
     * @param query the file path of the query protein fasta sequence file.
     * @param subject the file path of the subject protein sequence database file.
     * @param output the file path of the blastp alignment result output file.
     * @param evalue the e-value threshold of the accepted blastp hits.
     * 
     * + default value Is ``0.001``.
     * @param n_threads the thread number for run the blastp program.
     * 
     * + default value Is ``2``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a character value of the standard output log message of the ``blastp`` 
     *  program run, and the alignment result data will be saved into the 
     *  given output file.
   */
   function blastp(query: string, subject: string, output: string, evalue?: number, n_threads?: object, env?: object): any;
   /**
    * Translated Query Vs. Protein Database
    * 
    * > this api is not implemented at this moment.
    * 
   */
   function blastx(): any;
   /**
    * Application to create BLAST databases
    * 
    * 
     * @param dbtype Molecule type of target db
     * 
     * + default value Is ``["nucl","prot"]``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a character value of the standard output log message of the 
     *  ``makeblastdb`` program run.
   */
   function makeblastdb(in: string, dbtype?: any, env?: object): any;
}
