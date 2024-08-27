// export R# package module type define for javascript/typescript language
//
//    imports "pubmed" from "kb";
//
// ref=kb.pubmed_tools@kb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * PubMed is a free resource supporting the search and retrieval of biomedical and life sciences 
 *  literature with the aim of improving health–both globally and personally.
 *  
 *  The PubMed database contains more than 36 million citations And abstracts Of biomedical 
 *  literature. It does Not include full text journal articles; however, links To the full text 
 *  are often present When available from other sources, such As the publisher's website or 
 *  PubMed Central (PMC).
 *  
 *  Available to the public online since 1996, PubMed was developed And Is maintained by the
 *  National Center for Biotechnology Information (NCBI), at the U.S. National Library of 
 *  Medicine (NLM), located at the National Institutes of Health (NIH).
 * 
*/
declare namespace pubmed {
   /**
    * Parse the document text as a set of article object
    * 
    * 
     * @param text the pubmed database in flat file format, or the xml document content of 
     *  the pubmed article metadata.
     * @param xml 
     * + default value Is ``false``.
   */
   function article(text: string, xml?: boolean): object;
   module parse {
      /**
       * Parse the pubmed article set xml stream data
       * 
       * > batch download of the pubmed data from ncbi ftp server:
       * >  
       * >  > ftp://ftp.ncbi.nlm.nih.gov/pubmed/baseline/
       * 
        * @param file a single file that contains the pubmed article set data, data could be download from the pubmed ftp server in batch.
        * @param tqdm 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function article_set(file: any, tqdm?: boolean, env?: object): any;
   }
   /**
     * @param page default value Is ``1``.
     * @param size default value Is ``2000``.
   */
   function query(keyword: string, page?: object, size?: object): string;
}
