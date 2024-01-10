﻿// export R# package module type define for javascript/typescript language
//
//    imports "OTU_table" from "metagenomics_kit";
//
// ref=metagenomics_kit.OTUTableTools@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace OTU_table {
   /**
    * filter the otu data which has relative abundance greater than the given threshold
    * 
    * 
     * @param x -
     * @param relative_abundance -
   */
   function filter(x: object, relative_abundance: number): object;
   /**
   */
   function relative_abundance(x: object): object;
}
