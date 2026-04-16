// export R# package module type define for javascript/typescript language
//
//    imports "go.annotation" from "gokit";
//
// ref=gokit.annotation@gokit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace go.annotation {
   module uniprot {
      /**
       * export ko to go mapping data from the uniprot database.
       * 
       * 
        * @param uniprot the data reader of the uniprot xml database file.
        * @param threshold the supports coverage threshold value.
        * 
        * + default value Is ``0.8``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function ko2go(uniprot: object, threshold?: number, env?: object): object;
   }
}
