﻿// export R# package module type define for javascript/typescript language
//
//    imports "BIOM_kit" from "metagenomics_kit";
//
// ref=metagenomics_kit.BIOMkit@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the BIOM file toolkit
 * 
*/
declare namespace BIOM_kit {
   module biom {
      /**
       * get the taxonomy information from the BIOM matrix
       * 
       * 
        * @param biom -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function taxonomy(biom: any, env?: object): object;
      /**
       * union merge multiple biom matrix
       * 
       * 
        * @param tables -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function union(tables: any, env?: object): object;
   }
   module read {
      /**
       * read matrix data from a given BIOM file.
       * 
       * 
        * @param file -
        * @param denseMatrix 
        * + default value Is ``true``.
        * @param suppressErr 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function matrix(file: any, denseMatrix?: boolean, suppressErr?: boolean, env?: object): object;
   }
}
