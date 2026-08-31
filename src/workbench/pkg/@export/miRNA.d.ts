// export R# package module type define for javascript/typescript language
//
//    imports "miRNA" from "TRNtoolkit";
//
// ref=TRNtoolkit.miRNA@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * miRNA/siRNA target gene prediction toolkit
 * 
 * > This R# package module provides the toolkit for predict the target genes of 
 * >  the miRNA/siRNA small RNA sequence:
 * >  
 * >  + ``psRNATarget`` and ``TargetFinder``: create the miRNA target site match 
 * >    algorithm object;
 * >  + ``miRNA_targets``: run the target site match of the given miRNA sequence 
 * >    against the candidate target mRNA/CDS sequence collection;
 * >  + ``intersect_targets``: take the intersection of the two algorithm result for 
 * >    create the high confidence target site set.
 * >  
 * >  the generated match result is a collection of the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.siRNAHit`` 
 * >  object, which can be converted to a data frame via the ``as.data.frame`` api, 
 * >  or be saved as a csv table file via the ``write.csv`` api.
*/
declare namespace miRNA {
   /**
    * 对 blastn 预筛结果做 psRNATarget 风格的打分过滤。
    * 
    * 
     * @param blastn 由 ``parse_blastn`` 或 ``mirna_blastn`` 解析出的 HSP 集合。
     * @param evalueCutoff BLAST e-value 预筛阈值（与 blastn 命令行 -evalue 同量级）。
     *  注意它与 ``maxExpectation`` 量纲不同：前者是 BLAST 统计显著性，后者是打分期望值。
     * 
     * + default value Is ``1000``.
     * @param maxExpectation psRNATarget 期望分上限（越低越好）。
     * 
     * + default value Is ``5``.
     * @param minHitLength 最小 HSP 长度。
     * 
     * + default value Is ``17``.
     * @param seedStart 
     * + default value Is ``2``.
     * @param seedEnd 
     * + default value Is ``13``.
     * @param maxSeedMm 
     * + default value Is ``2``.
     * @param maxTotalMm 
     * + default value Is ``8``.
     * @param maxGu 
     * + default value Is ``7``.
     * @param verbose 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
   */
   function blastn_filter(blastn: any, evalueCutoff?: number, maxExpectation?: number, minHitLength?: object, seedStart?: object, seedEnd?: object, maxSeedMm?: object, maxTotalMm?: object, maxGu?: object, verbose?: boolean, env?: object): object;
   /**
    * --- High-confidence intersection (psRNATarget ∩ TargetFinder) ---
    * 
    * 
     * @param psRNATarget the target site match result of the psRNATarget algorithm, which is a 
     *  collection of the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.siRNAHit`` data.
     * @param TargetFinder the target site match result of the TargetFinder algorithm, which is a 
     *  collection of the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.siRNAHit`` data.
     * @param site_tolerance the coordinate alignment tolerance(in nt) of the target site location on the 
     *  mRNA sequence: two match result will be treated as the same target site if 
     *  their site interval is overlapped with each other in this tolerance range, by 
     *  default is 3nt.
     * 
     * + default value Is ``3``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.siRNAHit`` high confidence target site data: the 
     *  match result that is reported by both of the psRNATarget and the 
     *  TargetFinder algorithm.
     *  
     *  the merged site data of each target site: the ``Source`` property is marked 
     *  as ``Intersection(psRNATarget+TargetFinder)``, the ``StartSite``/``EndSite`` 
     *  property is the union range of the two algorithm result, the 
     *  ``MismatchCount``/``WobbleCount``/``GapCount`` property is the max value of 
     *  the two algorithm result, the ``TranslationInhibition`` property is TRUE when 
     *  any of the two algorithm result is a translation inhibition candidate, and 
     *  the ``Alignment`` property contains the expectation value of the psRNATarget 
     *  and the penalty score of the TargetFinder;
     *  
     *  this function returns a R# error message object if the input data can not be 
     *  cast to a collection of the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.siRNAHit`` data.
   */
   function intersect_targets(psRNATarget: any, TargetFinder: any, site_tolerance?: object, env?: object): object;
   /**
    * 
    * 
     * @param mirna -
     * @param geneset -
     * @param ncbi_blast folder dir path for the ncbi blast+
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function mirna_blastn(mirna: any, geneset: any, ncbi_blast?: string, env?: object): object;
   /**
    * make matches of the miRNA target genes
    * 
    * 
     * @param mapper the target site match algorithm object, which could be created by the 
     *  ``psRNATarget`` or the ``TargetFinder`` api of this package module.
     * @param miRNAs a collection of the miRNA sequence
     * @param targets a collection of the mRNA/CDS sequence of the candidate genes
     * @param parallel 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a set of the miRNA to target gene matches result, a match result network edges with match score as weights
     *  
     *  each @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.siRNAHit`` object in the generated result collection is a 
     *  match of one miRNA sequence to one target site of the candidate mRNA 
     *  sequence: the ``miRNA`` and the ``Target`` property is the sequence id of the 
     *  small RNA and the target mRNA, the ``StartSite``/``EndSite`` property is the 
     *  1-based site location on the target mRNA sequence, and the ``Expectation`` 
     *  property is the match score(the lower the better);
     *  
     *  this function returns NULL if the miRNA sequence collection or the target 
     *  sequence collection is empty, or the input data can not be cast to a fasta 
     *  sequence collection.
   */
   function miRNA_targets(mapper: object, miRNAs: any, targets: any, parallel?: boolean, env?: object): object;
   /**
    * 
    * 
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function parse_blastn(file: any, env?: object): object;
   /**
    * create the psRNATarget algorithm object for predict the miRNA target site
    * 
    * 
     * @param version the schema version of the psRNATarget algorithm: the ``V1_2011`` schema uses 
     *  the seed region of the 2-8nt site of the miRNA sequence, and the 
     *  ``V2_2017`` schema(the default) uses the seed region of the 2-13nt site.
     * 
     * + default value Is ``null``.
     * @param max_expectation the maximum expectation value cutoff of the target site match: the candidate 
     *  match result that its position weighted expectation value is greater than 
     *  this cutoff will be ignored(the expectation value is the lower the better).
     * 
     * + default value Is ``5``.
     * @return a @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.psRNATarget`` algorithm object, which implements the 
     *  @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.miRNAMapper`` interface, so that it can be used by the 
     *  ``miRNA_targets`` api for run the target site match.
   */
   function psRNATarget(version?: object, max_expectation?: number): object;
   /**
    * create the TargetFinder algorithm object for predict the miRNA target site
    * 
    * 
     * @param score_cutoff the score cutoff of the target site match: the candidate match result that 
     *  its position weighted penalty score is greater than this cutoff will be 
     *  ignored(the penalty score is the lower the better), the recommended value 
     *  is 4.0 for the strict mode, 5.0 for the standard mode and 7.0 for the loose 
     *  mode.
     * 
     * + default value Is ``5``.
     * @return a @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.TargetFinder`` 
     *  algorithm object, which implements the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.miRNAMapper`` interface, so 
     *  that it can be used by the ``miRNA_targets`` api for run the target site 
     *  match.
   */
   function TargetFinder(score_cutoff?: number): object;
}
