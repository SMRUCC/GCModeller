declare namespace blast+ {
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
