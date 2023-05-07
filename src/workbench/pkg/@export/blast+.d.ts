// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.blastPlusInterop

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
     * @param dbtype default value is ``["nucl","prot"]``.
     * @param env default value is ``null``.
   */
   function makeblastdb(in:string, dbtype?:any, env?:object): any;
   /**
     * @param evalue default value is ``0.001``.
     * @param n_threads default value is ``2``.
     * @param env default value is ``null``.
   */
   function blastp(query:string, subject:string, output:string, evalue?:number, n_threads?:object, env?:object): any;
   /**
   */
   function blastn(): any;
   /**
   */
   function blastx(): any;
}
