// export R# package module type define for javascript/typescript language
//
//    imports "NCBI" from "annotationKit";
//
// ref=annotationKit.NCBI@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace NCBI {
   /**
    * read ncbi ftp index of the genome assembly
    * 
    * 
   */
   function genome_assembly_index(file: string): object;
}
