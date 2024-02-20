// export R# package module type define for javascript/typescript language
//
//    imports "OTU_table" from "metagenomics_kit";
//
// ref=metagenomics_kit.OTUTableTools@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Tools for handling OTU table data
 * 
 * > ### Operational taxonomic unit (OTU)
 * >  
 * >  OTU's are used to categorize bacteria based on sequence similarity.
 * >  
 * >  In 16S metagenomics approaches, OTUs are cluster of similar sequence variants of the 
 * >  16S rDNA marker gene sequence. Each of these cluster is intended to represent a 
 * >  taxonomic unit of a bacteria species or genus depending on the sequence similarity 
 * >  threshold. Typically, OTU cluster are defined by a 97% identity threshold of the 16S 
 * >  gene sequences to distinguish bacteria at the genus level.
 * > 
 * >  Species separation requires a higher threshold Of 98% Or 99% sequence identity, Or 
 * >  even better the use Of exact amplicon sequence variants (ASV) instead Of OTU sequence 
 * >  clusters.
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
    * Transform abundance data in an otu_table to relative abundance, sample-by-sample. 
    *  
    *  Transform abundance data into relative abundance, i.e. proportional data. This is 
    *  an alternative method of normalization and may not be appropriate for all datasets,
    *  particularly if your sequencing depth varies between samples.
    * 
    * 
     * @param x -
   */
   function relative_abundance(x: object): object;
}
