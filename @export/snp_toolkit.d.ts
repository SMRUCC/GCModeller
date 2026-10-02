// export R# package module type define for javascript/typescript language
//
//    imports "snp_toolkit" from "seqtoolkit";
//
// ref=seqtoolkit.snpTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The nucleotide SNP variations analysis toolkit
 * 
 * > This R# package module provides the api for the SNP(simple nucleotide 
 * >  polymorphism) variations analysis based on the multiple sequence 
 * >  alignment of the nucleotide fasta sequence data:
 * >  
 * >  + ``snp_scan``: scan the SNP variation sites from a given multiple 
 * >    sequence alignment fasta file.
*/
declare namespace snp_toolkit {
   /**
    * scan the SNP variation sites from a given multiple sequence alignment 
    *  fasta sequence data
    * 
    * 
     * @param nt a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) multiple sequence alignment nucleotide 
     *  sequence collection for scan the SNP variation sites.
     * @param ref_index the index number of the reference sequence in the input multiple 
     *  sequence alignment collection, which is used as the reference for 
     *  make the SNP calling.
     * @param pureMode only accept the pure SNP variations? if this parameter is TRUE, then 
     *  the SNP sites that contains mixed variation information will be 
     *  ignored.
     * 
     * + default value Is ``false``.
     * @param monomorphic include the monomorphic sites in the SNP scanning result?
     * 
     * + default value Is ``false``.
     * @param vcf_output_filename a by-ref parameter for get the file path of the generated VCF format 
     *  SNP variations output file.
     * 
     * + default value Is ``null``.
     * @return a [SNPsAln](cref:T:SMRUCC.genomics.Analysis.SequenceTools.SNP.SangerSNPs.SNPsAln) SNP variations analysis result object.
   */
   function snp_scan(nt: object, ref_index: string, pureMode?: boolean, monomorphic?: boolean, vcf_output_filename?: object): object;
}
