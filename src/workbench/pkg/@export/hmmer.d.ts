// export R# package module type define for javascript/typescript language
//
//    imports "hmmer" from "seqtoolkit";
//
// ref=seqtoolkit.hmmer@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace hmmer {
   /**
     * @param env default value Is ``null``.
   */
   function hmmer_search(hmmer: object, x: any, env?: object): object;
   /**
   */
   function load_hmmer(x: any): object;
   /**
   */
   function load_interprodb(file: string): object;
   /**
   */
   function parse_hmmer_model(x: string): object;
   /**
    * Parse the kofamscan table output
    * 
    * 
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function parse_kofamscan(file: any, env?: object): object;
}
