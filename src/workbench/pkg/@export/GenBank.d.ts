// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.genbankKit

/**
 * NCBI genbank assembly file I/O toolkit
 * 
*/
declare namespace GenBank {
   module read {
      /**
        * @param repliconTable default value is ``false``.
        * @param env default value is ``null``.
      */
      function genbank(file:string, repliconTable?:boolean, env?:object): any;
   }
   module is {
      /**
        * @param env default value is ``null``.
      */
      function plasmid(gb:any, env?:object): any;
   }
   module populate {
      /**
        * @param autoClose default value is ``true``.
        * @param env default value is ``null``.
      */
      function genbank(files:any, autoClose?:boolean, env?:object): any;
   }
   module write {
      /**
        * @param env default value is ``null``.
      */
      function genbank(gb:object, file:string, env?:object): any;
   }
   module as {
      /**
        * @param env default value is ``null``.
      */
      function genbank(x:any, env?:object): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function feature(keyName:string, location:object, data:object, env?:object): any;
   module add {
      /**
      */
      function feature(gb:object, feature:object): any;
      module RNA {
         /**
           * @param env default value is ``null``.
         */
         function gene(gb:object, RNA:any, env?:object): any;
      }
   }
   /**
   */
   function enumerateFeatures(gb:object): any;
   /**
     * @param env default value is ``null``.
   */
   function featureKeys(features:any, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function featureMeta(features:any, attrName:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function addMeta(feature:object, meta:object, env?:object): any;
   module origin {
      /**
        * @param nt default value is ``null``.
        * @param mol_type default value is ``'genomic DNA'``.
      */
      function fasta(gb:object, nt?:object, mol_type?:string): any;
   }
   module getRNA {
      /**
      */
      function fasta(gb:object): any;
   }
   module protein {
      /**
        * @param proteins default value is ``null``.
        * @param env default value is ``null``.
      */
      function fasta(gb:object, proteins?:any, env?:object): any;
   }
}
