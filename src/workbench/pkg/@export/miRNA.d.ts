// export R# package module type define for javascript/typescript language
//
//    imports "miRNA" from "TRNtoolkit";
//
// ref=TRNtoolkit.miRNA@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace miRNA {
   /**
    * --- High-confidence intersection (psRNATarget ∩ TargetFinder) ---
    * 
    * 
     * @param psRNATarget -
     * @param TargetFinder -
     * @param site_tolerance -
     * 
     * + default value Is ``3``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function intersect_targets(psRNATarget: any, TargetFinder: any, site_tolerance?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function miRNA_targets(mapper: object, miRNAs: any, targets: any, env?: object): object;
   /**
     * @param version default value Is ``null``.
     * @param max_expectation default value Is ``5``.
   */
   function psRNATarget(version?: object, max_expectation?: number): object;
   /**
     * @param score_cutoff default value Is ``5``.
   */
   function TargetFinder(score_cutoff?: number): object;
}
