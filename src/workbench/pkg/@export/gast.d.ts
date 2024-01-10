// export R# package module type define for javascript/typescript language
//
//    imports "gast" from "metagenomics_kit";
//
// ref=metagenomics_kit.gastTools@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * gast 16s data analysis tools, combine with the mothur workflow
 * 
*/
declare namespace gast {
   module OTU {
      /**
       * assign OTU its taxonomy result
       * 
       * 
        * @param blastn a collection of the alignment @``T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query`` that parsed 
        *  from the ncbi blastn alignment result text file.
        * @param taxonomy a collection of the @``T:SMRUCC.genomics.Analysis.Metagenome.greengenes.otu_taxonomy`` data.
        * @param min_pct 
        * + default value Is ``0.97``.
        * @param gast_consensus 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function taxonomy(blastn: any, OTUs: object, taxonomy: object, min_pct?: number, gast_consensus?: boolean, env?: object): object;
   }
   module parse {
      /**
       * Parse the greengenes taxonomy file
       * 
       * 
        * @param tax the file path of the greengenes taxonomy mapping file.
        * @return a tuple list of the otu taxonomy information
      */
      function greengenes_tax(tax: string): object;
      /**
       * parse the OTU data file
       * 
       * 
        * @param OTU_rep_fasta ``OTU.rep.fasta`` query source comes from the mothur workflow.
        * @param removes_lt 
        * + default value Is ``0.0001``.
      */
      function mothur_OTUs(OTU_rep_fasta: string, removes_lt?: number): object;
   }
}
