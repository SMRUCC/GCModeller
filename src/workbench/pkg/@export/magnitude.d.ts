// export R# package module type define for javascript/typescript language
//
//    imports "magnitude" from "phenotype_kit";
//
// ref=phenotype_kit.magnitude@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * HTS expression data simulating for analysis test
 * 
*/
declare namespace magnitude {
   /**
     * @param base default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function profiles(selector: string, foldchange: number, base?: object, env?: object): any;
}
