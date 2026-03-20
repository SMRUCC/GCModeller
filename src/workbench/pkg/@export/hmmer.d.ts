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
