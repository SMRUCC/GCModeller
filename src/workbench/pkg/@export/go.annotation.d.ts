// export R# package module type define for javascript/typescript language
//
//    imports "go.annotation" from "gokit";
//
// ref=gokit.annotation@gokit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace go.annotation {
   module uniprot {
      /**
        * @param threshold default value Is ``0.8``.
        * @param env default value Is ``null``.
      */
      function ko2go(uniprot: object, threshold?: number, env?: object): object;
   }
}
