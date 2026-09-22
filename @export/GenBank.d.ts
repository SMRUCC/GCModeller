// export R# package module type define for javascript/typescript language
//
//    imports "GenBank" from "seqtoolkit";
//
// ref=seqtoolkit.genbankKit@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace GenBank {
   /**
     * @param env default value Is ``null``.
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
   */
   function add_feature(gb: object, feature: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function addMeta(feature: object, meta: object, env?: object): object;
   module as {
      /**
        * @param env default value Is ``null``.
      */
      function genbank(x: any, env?: object): object;
   }
   /**
     * @param ORF default value Is ``true``.
   */
   function as_tabular(gbff: object, ORF?: boolean): object;
   /**
     * @param env default value Is ``null``.
   */
   function assembly_level(gb: any, env?: object): object;
   /**
     * @param keys default value Is ``null``.
   */
   function enumerateFeatures(gb: object, keys?: string): object;
   /**
     * @param title default value Is ``'<gb_asm_id>.<locus_tag> <nucl_loc> <product>|<lineage>'``.
     * @param key default value Is ``["gene","CDS"]``.
     * @param required default value Is ``null``.
     * @param unique_names default value Is ``false``.
   */
   function export_geneNt_fasta(gb: object, title?: string, key?: any, required?: string, unique_names?: boolean): object;
   /**
     * @param data default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function feature(keyName: string, location: object, data?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function featureKeys(features: any, env?: object): string;
   /**
     * @param attrName default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function featureMeta(features: any, attrName?: string, env?: object): string;
   module getRNA {
      /**
      */
      function fasta(gb: object): object;
   }
   module is {
      /**
        * @param env default value Is ``null``.
      */
      function plasmid(gb: any, env?: object): boolean;
   }
   /**
     * @param extract_genomics default value Is ``false``.
     * @param autoClose default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function load_genbanks(files: any, extract_genomics?: boolean, autoClose?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function moltype(gb: any, env?: object): object;
   /**
     * @param nt default value Is ``null``.
     * @param mol_type default value Is ``'genomic DNA'``.
   */
   function origin_fasta(gb: object, nt?: object, mol_type?: string): object|object;
   /**
     * @param proteins default value Is ``null``.
     * @param title default value Is ``null``.
     * @param filter_empty default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function protein_seqs(gb: object, proteins?: any, title?: string, filter_empty?: boolean, env?: object): object;
   module read {
      /**
        * @param repliconTable default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function genbank(file: string, repliconTable?: boolean, env?: object): object;
   }
   /**
   */
   function read_genetable(file: string): object;
   /**
   */
   function taxon_id(gb: object): object;
   /**
   */
   function taxonomy_lineage(gb: object): object;
   module write {
      /**
        * @param env default value Is ``null``.
      */
      function genbank(gb: object, file: string, env?: object): boolean;
   }
}
