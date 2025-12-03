// export R# package module type define for javascript/typescript language
//
//    imports "kmers" from "metagenomics_kit";
//    imports "kmers" from "seqtoolkit";
//
// ref=metagenomics_kit.KmersTool@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=seqtoolkit.kmersTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace kmers {
   /**
    * generate sequence k-mer count data matrix
    * 
    * 
     * @param x -
     * @param k -
     * 
     * + default value Is ``3``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function kmers_matrix(x: any, k?: object, env?: object): object;
}
