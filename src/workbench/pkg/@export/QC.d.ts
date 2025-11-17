// export R# package module type define for javascript/typescript language
//
//    imports "QC" from "rnaseq";
//
// ref=rnaseq.QC@rnaseq, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace QC {
   /**
     * @param quality default value Is ``20``.
     * @param env default value Is ``null``.
   */
   function trim_low_quality(reads: any, quality?: object, env?: object): any;
   /**
    * trim the primers or adapters sequence at read header region
    * 
    * 
     * @param reads -
     * @param headers -
     * @param exact_match 
     * + default value Is ``true``.
     * @param cutoff 
     * + default value Is ``0.85``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function trim_reads_headers(reads: any, headers: any, exact_match?: boolean, cutoff?: number, env?: object): any;
}
