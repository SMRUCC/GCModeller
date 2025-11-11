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
     * @param qgram default value Is ``6``.
   */
   function genbank_assemblyDb(file: string, qgram?: object): object;
   /**
    * read ncbi ftp index of the genome assembly
    * 
    * 
   */
   function genome_assembly_index(file: string): object;
   /**
    * make the in-memory assembly summary database query by the organism name matches
    * 
    * 
     * @param db -
     * @param q -
     * @param cutoff -
     * 
     * + default value Is ``0.8``.
     * @return a vector of @``T:SMRUCC.genomics.Data.GenBankAssemblyIndex``. and this vector data has the attribute data 
     *  of query ``index`` result with clr type @``T:Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository.FindResult``.
   */
   function query(db: object, q: string, cutoff?: number): object;
}
