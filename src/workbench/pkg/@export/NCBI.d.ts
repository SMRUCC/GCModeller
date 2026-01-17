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
    * create the index db
    * 
    * 
     * @param file -
     * @param repo_dir -
   */
   function create_assemblyDb(file: string, repo_dir: string): ;
   /**
    * load the index db
    * 
    * 
     * @param repo_dir -
     * @param qgram -
     * 
     * + default value Is ``6``.
   */
   function genbank_assemblyDb(repo_dir: string, qgram?: object): object;
   /**
    * read ncbi ftp index of the genome assembly
    * 
    * 
     * @param make_accession_index 
     * + default value Is ``false``.
   */
   function genome_assembly_index(file: string, make_accession_index?: boolean): object;
   /**
    * make data subset via a given collection of the acession id
    * 
    * 
     * @param repo -
     * @param accession_ids -
   */
   function index_subset(repo: object, accession_ids: any): any;
   /**
    * make the in-memory assembly summary database query by the organism name matches
    * 
    * 
     * @param db -
     * @param q -
     * @param cutoff -
     * 
     * + default value Is ``0.8``.
     * @param best_match 
     * + default value Is ``false``.
     * @return a vector of @``T:SMRUCC.genomics.Data.GenBankAssemblyIndex``. and this vector data has the attribute data 
     *  of query ``index`` result with clr type @``T:Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository.FindResult``.
   */
   function query(db: object, q: string, cutoff?: number, best_match?: boolean): object;
}
