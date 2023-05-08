// export R# package module type define for javascript/typescript language
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
    * Application to create BLAST databases
    * 
    * 
     * @param dbtype Molecule type of target db
     * 
     * + default value Is ``["nucl","prot"]``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function makeblastdb(in:string, dbtype?:any, env?:object): any;
   /**
    * Protein-Protein BLAST
    * 
    * 
     * @param evalue 
     * + default value Is ``0.001``.
     * @param n_threads 
     * + default value Is ``2``.
     * @param env 
     * + default value Is ``null``.
   */
   function blastp(query:string, subject:string, output:string, evalue?:number, n_threads?:object, env?:object): any;
   /**
   */
   function blastn(): any;
   /**
   */
   function blastx(): any;
}
