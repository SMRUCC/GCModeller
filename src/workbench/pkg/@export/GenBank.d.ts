// export R# package module type define for javascript/typescript language
//
//    imports "GenBank" from "seqtoolkit"
//
// ref=seqtoolkit.genbankKit@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * NCBI genbank assembly file I/O toolkit
 * 
*/
declare namespace GenBank {
   module add {
      /**
      */
      function feature(gb: object, feature: object): object;
      module RNA {
         /**
           * @param env default value Is ``null``.
         */
         function gene(gb: object, RNA: any, env?: object): any;
      }
   }
   /**
     * @param env default value Is ``null``.
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
    * enumerate all features in the given NCBI genbank database object
    * 
    * 
     * @param gb a NCBI genbank database object
   */
   function enumerateFeatures(gb: object): object;
   /**
    * create new feature site
    * 
    * 
     * @param location -
     * @param data -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function feature(keyName: string, location: object, data: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function featureKeys(features: any, env?: object): string;
   /**
     * @param env default value Is ``null``.
   */
   function featureMeta(features: any, attrName: string, env?: object): string;
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
        * @param env default value Is ``null``.
      */
      function plasmid(gb: any, env?: object): boolean;
   }
   module origin {
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
      function fasta(gb: object, nt?: object, mol_type?: string): object|object;
   }
   module populate {
      /**
       * populate a list of genbank data objects from a given list of files or stream.
       * 
       * 
        * @param files a list of files or file stream
        * @param autoClose auto close of the @``T:System.IO.Stream`` if the **`files`** contains stream object?
        * 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function genbank(files: any, autoClose?: boolean, env?: object): object;
   }
   module protein {
      /**
       * get or set fasta sequence of all CDS feature in the given genbank assembly file.
       * 
       * 
        * @param gb -
        * @param proteins -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function fasta(gb: object, proteins?: any, env?: object): object;
   }
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
      function genbank(file: string, repliconTable?: boolean, env?: object): any;
   }
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
      function genbank(gb: object, file: string, env?: object): any;
   }
}
