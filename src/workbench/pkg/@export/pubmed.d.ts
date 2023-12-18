// export R# package module type define for javascript/typescript language
//
//    imports "pubmed" from "kb";
//
// ref=kb.pubmed@kb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
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
