// export R# package module type define for javascript/typescript language
//
//    imports "FastQ" from "rnaseq";
//
// ref=rnaseq.FastQ@rnaseq, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace FastQ {
   /**
     * @param env default value Is ``null``.
   */
   function assemble(reads: any, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function quality_score(q: any, env?: object): number;
}
