// export R# package module type define for javascript/typescript language
//
//    imports "operon" from "comparative_toolkit";
//
// ref=comparative_toolkit.operonMapper@comparative_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace operon {
   /**
   */
   function known_operons(): object;
   /**
    * load operon set data from the ODB database
    * 
    * 
     * @param file dataset text file that download from https://operondb.jp/
     * 
     * + default value Is ``null``.
   */
   function operon_set(file?: string): object;
}
