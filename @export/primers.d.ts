// export R# package module type define for javascript/typescript language
//
//    imports "primers" from "seqtoolkit";
//
// ref=seqtoolkit.primers@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * PCR primer design tools
 * 
 * > This R# package module provides the api for find the candidate PCR primer 
 * >  design regions from the blastn web search hits:
 * >  
 * >  + ``primer_regions``: find the candidate PCR primer design regions from a 
 * >    given set of the blastn hit records, and then annotate the gene context 
 * >    information of each candidate region via a given genomics feature 
 * >    annotation table(GFF).
*/
declare namespace primers {
   /**
    * find the candidate PCR primer design regions from a set of the blastn hits
    * 
    * 
     * @param blastHits a collection of the blastn hit records for find the candidate primer 
     *  design regions, which can be a pipeline object or a vector of the 
     *  [HitRecord](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast.HitRecord) object.
     * @param maxCoreSpan the maximum span size in nucleotide of the core primer region.
     * 
     * + default value Is ``1000000``.
     * @param eval_cutoff the maximum e-value threshold of the accepted blastn hit records.
     * 
     * + default value Is ``1``.
     * @param primerIds a character vector of the primer id for filter the input blastn hits. 
     *  If this parameter is not specified, then all of the input blastn hits 
     *  will be used for find the candidate regions.
     * 
     * + default value Is ``null``.
     * @param genome a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table, which is 
     *  used for annotate the gene context information of the candidate primer 
     *  regions. If this parameter is not specified, then no gene context 
     *  information will be calculated.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [CandidateRegion](cref:T:SMRUCC.genomics.Analysis.PrimerDesigner.CandidateRegion) object, each element in 
     *  the returned vector is one candidate PCR primer design region with its 
     *  gene context information;
     *  
     *  this function returns a R# error message object if the input blastn hits 
     *  data can not be cast to a collection of the [HitRecord](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast.HitRecord) 
     *  object.
   */
   function primer_regions(blastHits: any, maxCoreSpan?: object, eval_cutoff?: number, primerIds?: any, genome?: object, env?: object): object;
}
