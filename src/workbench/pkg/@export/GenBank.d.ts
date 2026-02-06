// export R# package module type define for javascript/typescript language
//
//    imports "GenBank" from "seqtoolkit";
//
// ref=seqtoolkit.genbankKit@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * NCBI genbank assembly file I/O toolkit
 * 
*/
declare namespace GenBank {
   /**
    * get current genbank assembly accession id
    * 
    * 
     * @param genbank -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function accession_id(genbank: any, env?: object): string;
   module add {
      module RNA {
         /**
           * @param env default value Is ``null``.
         */
         function gene(gb: object, RNA: any, env?: object): object;
      }
   }
   /**
    * add feature into a given genbank object
    * 
    * 
     * @param gb -
     * @param feature -
   */
   function add_feature(gb: object, feature: object): object;
   /**
    * add metadata into a given feature object
    * 
    * 
     * @param feature -
     * @param meta -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function addMeta(feature: object, meta: object, env?: object): object;
   module as {
      /**
       * converts tabular data file to genbank assembly object
       * 
       * 
        * @param x -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function genbank(x: any, env?: object): object;
   }
   /**
    * extract all gene features from genbank and cast to tabular data
    * 
    * 
     * @param gbff -
     * @param ORF 
     * + default value Is ``true``.
   */
   function as_tabular(gbff: object, ORF?: boolean): object;
   /**
    * enumerate all features in the given NCBI genbank database object
    * 
    * 
     * @param gb a NCBI genbank database object
     * @param keys 
     * + default value Is ``null``.
   */
   function enumerateFeatures(gb: object, keys?: string): object;
   /**
    * export gene fasta from the given genbank assembly file
    * 
    * > fasta title is build with a string template, there are some reserved template keyword for this function:
    * >  
    * >  1. ncbi_taxid - is the ncbi taxonomy id that extract from the genbank assembly
    * >  2. lineage - taxonomy lineage in biom style string, which is extract from the genbank assembly its source information
    * >  3. gb_asm_id - the ncbi accession id of the genbank assembly
    * >  4. nucl_loc - the nucleotide sequence location on the genomics sequence
    * >  
    * >  ##### about the salmon duplicated id error
    * >  
    * >  if you encounter this error while build sequence index by using salmon tool, please set the ``unique.names`` parameter to TRUE
    * >  
    * >  ```
    * >  counted k-mers for 110000 transcripts[2026-01-06 15:08:42.160] [puff::index::jointLog] [error] In FixFasta, two references with the same name but different sequences: AM295250.SCA_1840. We require that all input records have a unique name up to the first whitespace (or user-provided separator) character.
    * >  ```
    * 
     * @param gb -
     * @param title -
     * 
     * + default value Is ``'<gb_asm_id>.<locus_tag> <nucl_loc> <product>|<lineage>'``.
     * @param key 
     * + default value Is ``["gene","CDS"]``.
     * @param required 
     * + default value Is ``null``.
     * @param unique_names processing the possible duplicated header as unique id, this option is usually when you use this function for build a sequence database for salmon tool.
     * 
     * + default value Is ``false``.
   */
   function export_geneNt_fasta(gb: object, title?: string, key?: any, required?: string, unique_names?: boolean): object;
   /**
    * create new feature site
    * 
    * 
     * @param keyName -
     * @param location -
     * @param data -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function feature(keyName: string, location: object, data?: object, env?: object): object;
   /**
    * get all feature key names
    * 
    * 
     * @param features a collection of the genbank feature object or a genbank clr object.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function featureKeys(features: any, env?: object): string;
   /**
    * extract the feature metadata from a genbank clr feature object
    * 
    * 
     * @param features -
     * @param attrName 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function featureMeta(features: any, attrName?: string, env?: object): string;
   module getRNA {
      /**
       * get all of the RNA gene its gene sequence in fasta sequence format.
       * 
       * 
        * @param gb -
      */
      function fasta(gb: object): object;
   }
   module is {
      /**
       * check of the given genbank assembly is the data source of a plasmid or not?
       * 
       * 
        * @param gb -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function plasmid(gb: any, env?: object): boolean;
   }
   /**
    * populate a list of genbank data objects from a given list of files or stream.
    * 
    * > this function supports of read assembly data directly from *.gz genbank archive file.
    * 
     * @param files a list of files or file stream
     * @param autoClose auto close of the @``T:System.IO.Stream`` if the **`files`** contains stream object?
     * 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function load_genbanks(files: any, autoClose?: boolean, env?: object): object;
   /**
    * get, add or replace the genome origin fasta sequence in the given genbank assembly file.
    * 
    * 
     * @param gb -
     * @param nt -
     * 
     * + default value Is ``null``.
     * @param mol_type -
     * 
     * + default value Is ``'genomic DNA'``.
     * @return if the ``**`nt`**`` parameter is nothing, 
     *  means get fasta sequence, otherwise is add/update fasta 
     *  sequence in the genbank assembly, the returns type of 
     *  the api will change from the getted fasta sequence to 
     *  the modified genbank assembly object.
   */
   function origin_fasta(gb: object, nt?: object, mol_type?: string): object|object;
   /**
    * get or set fasta sequence of all CDS feature in the given genbank assembly file.
    * 
    * 
     * @param gb -
     * @param proteins set the genbank feature CDS protein sequence if this value is existsed.
     * 
     * + default value Is ``null``.
     * @param title 
     * + default value Is ``null``.
     * @param filter_empty Filter out the empty protein sequence when do export of the protein sequence data.
     * 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function protein_seqs(gb: object, proteins?: any, title?: string, filter_empty?: boolean, env?: object): object;
   module read {
      /**
       * read the given genbank assembly file.
       * 
       * 
        * @param file the file path of the given genbank assembly file.
        * @param repliconTable -
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function genbank(file: string, repliconTable?: boolean, env?: object): object;
   }
   /**
    * get ncbi taxonomy id from the given genbank assembly file.
    * 
    * 
     * @param gb -
   */
   function taxon_id(gb: object): any;
   /**
    * extract the taxonomy lineage information from the genbank file
    * 
    * 
     * @param gb -
   */
   function taxonomy_lineage(gb: object): object;
   module write {
      /**
       * save the modified genbank file
       * 
       * 
        * @param gb -
        * @param file the file path of the genbank assembly file to write data.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function genbank(gb: object, file: string, env?: object): boolean;
   }
}
