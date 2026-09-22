// export R# package module type define for javascript/typescript language
//
//    imports "primers" from "seqtoolkit";
//
// ref=seqtoolkit.primers@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace primers {
   /**
     * @param maxCoreSpan default value Is ``1000000``.
     * @param eval_cutoff default value Is ``1``.
     * @param primerIds default value Is ``null``.
     * @param genome default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function primer_regions(blastHits: any, maxCoreSpan?: object, eval_cutoff?: number, primerIds?: any, genome?: object, env?: object): object;
}
