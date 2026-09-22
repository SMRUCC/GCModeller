// export R# package module type define for javascript/typescript language
//
//    imports "blast+" from "seqtoolkit";
//
// ref=seqtoolkit.blastPlusInterop@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace blast_ {
   /**
   */
   function blastn(): any;
   /**
     * @param evalue default value Is ``0.001``.
     * @param n_threads default value Is ``2``.
     * @param env default value Is ``null``.
   */
   function blastp(query: string, subject: string, output: string, evalue?: number, n_threads?: object, env?: object): any;
   /**
   */
   function blastx(): any;
   /**
     * @param dbtype default value Is ``["nucl","prot"]``.
     * @param env default value Is ``null``.
   */
   function makeblastdb(in: string, dbtype?: any, env?: object): any;
}
