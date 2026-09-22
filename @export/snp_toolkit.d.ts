// export R# package module type define for javascript/typescript language
//
//    imports "snp_toolkit" from "seqtoolkit";
//
// ref=seqtoolkit.snpTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace snp_toolkit {
   /**
     * @param pureMode default value Is ``false``.
     * @param monomorphic default value Is ``false``.
     * @param vcf_output_filename default value Is ``null``.
   */
   function snp_scan(nt: object, ref_index: string, pureMode?: boolean, monomorphic?: boolean, vcf_output_filename?: object): object;
}
