// export R# package module type define for javascript/typescript language
//
//    imports "pubmed" from "kb";
//
// ref=kb.pubmed@kb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

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
    * parse the text data as the article information
    * 
    * 
     * @param text text data in pubmed format
   */
   function parse(text: any): object;
}
