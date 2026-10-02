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
     * @param genbank a collection of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly 
     *  object for get its accession id.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a character vector of the ncbi accession id of each genbank assembly;
     *  
     *  this function returns a R# error message object if the input genbank 
     *  data can not be cast to a collection of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) 
     *  object.
   */
   function accession_id(genbank: any, env?: object): string;
   module add {
      module RNA {
         /**
          * add the RNA gene feature data into the given genbank assembly by the 
          *  blastn mapping result
          * 
          * 
           * @param gb the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for add the 
           *  RNA gene feature data.
           * @param RNA the blastn mapping result data of the RNA sequence, which can be a 
           *  vector of the [BlastnMapping](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping.BlastnMapping) object or a pipeline object 
           *  that produces this mapping data.
           * @param env the R# runtime environment object.
           * 
           * + default value Is ``null``.
           * @return the modified [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object;
           *  
           *  this function returns a R# error message object if the input RNA 
           *  mapping data is not a supported data model.
         */
         function gene(gb: object, RNA: any, env?: object): object;
      }
   }
   /**
    * add feature into a given genbank object
    * 
    * 
     * @param gb the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for add the 
     *  target feature site.
     * @param feature the [Feature](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature) genbank feature object for add, which is 
     *  created by the ``feature`` api.
     * @return the modified [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object.
   */
   function add_feature(gb: object, feature: object): object;
   /**
    * add metadata into a given feature object
    * 
    * 
     * @param feature the target [Feature](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature) genbank feature object for add the 
     *  metadata.
     * @param meta a R# list object of the metadata: the name of each list element is the 
     *  qualifier name, and the element value is the qualifier value.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the modified [Feature](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature) genbank feature object.
   */
   function addMeta(feature: object, meta: object, env?: object): object;
   module as {
      /**
       * converts tabular data file to genbank assembly object
       * 
       * 
        * @param x the tabular data object for convert as the genbank assembly object. 
        *  Only the [PTT](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT) tabular table object is supported at this 
        *  moment.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object that converted 
        *  from the given tabular data;
        *  
        *  this function returns a R# error message object if the given tabular 
        *  data type is not supported.
      */
      function genbank(x: any, env?: object): object;
   }
   /**
    * extract all gene features from genbank and cast to tabular data
    * 
    * 
     * @param gbff a [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for extract its 
     *  gene features.
     * @param ORF export the gene features with its ORF region data?
     * 
     * + default value Is ``true``.
     * @return a vector of the [GeneTable](cref:T:SMRUCC.genomics.ComponentModel.Annotation.GeneTable) gene table record object.
   */
   function as_tabular(gbff: object, ORF?: boolean): object;
   /**
    * get the assembly level evidence annotation of the given genbank 
    *  assembly
    * 
    * 
     * @param gb a collection of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly 
     *  object for get its assembly level evidence.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [AssemblyLevelEvidence](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.AssemblyLevelEvidence) assembly level 
     *  evidence object;
     *  
     *  this function returns a R# error message object if the input genbank 
     *  data can not be cast to a collection of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) 
     *  object.
   */
   function assembly_level(gb: any, env?: object): object;
   /**
    * enumerate all features in the given NCBI genbank database object
    * 
    * 
     * @param gb a NCBI genbank database object
     * @param keys a character vector of the feature key name for filter the target 
     *  features. If this parameter is not specified, then all of the features 
     *  inside the given genbank object will be returned.
     * 
     * + default value Is ``null``.
     * @return a vector of the [Feature](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature) genbank feature object.
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
    * > ```
    * >  counted k-mers for 110000 transcripts[2026-01-06 15:08:42.160] [puff::index::jointLog] [error] In FixFasta, two references with the same name but different sequences: AM295250.SCA_1840. We require that all input records have a unique name up to the first whitespace (or user-provided separator) character.
    * >  ```
    * 
     * @param gb the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for export 
     *  its gene nucleotide sequence data.
     * @param title the fasta headers title template string for export of the gene 
     *  sequence data.
     * 
     * + default value Is ``'<gb_asm_id>.<locus_tag> <nucl_loc> <product>|<lineage>'``.
     * @param key the feature key name for extract the gene sequence data, example as 
     *  ``gene`` or ``CDS``.
     * 
     * + default value Is ``["gene","CDS"]``.
     * @param required a character vector of the template keyword name for filter the gene 
     *  export: the gene that its template data is missing any of the required 
     *  keyword will be skipped.
     * 
     * + default value Is ``null``.
     * @param unique_names processing the possible duplicated header as unique id, this option is usually when you use this function for build a sequence database for salmon tool.
     * 
     * + default value Is ``false``.
     * @return a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) gene nucleotide sequence collection object.
   */
   function export_geneNt_fasta(gb: object, title?: string, key?: any, required?: string, unique_names?: boolean): object;
   /**
    * create new feature site
    * 
    * 
     * @param keyName the feature key name, example as ``gene``, ``CDS`` or ``tRNA``.
     * @param location the location region of the target feature site.
     * @param data a R# list object of the feature qualifiers data: the name of each list 
     *  element is the qualifier name, and the element value is a character 
     *  vector of the qualifier value.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a new [Feature](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature) genbank feature site object.
   */
   function feature(keyName: string, location: object, data?: object, env?: object): object;
   /**
    * get all feature key names
    * 
    * 
     * @param features a collection of the genbank feature object or a genbank clr object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a character vector of the feature key name;
     *  
     *  this function returns a R# error message object if the input features 
     *  data can not be cast to a collection of the genbank feature object.
   */
   function featureKeys(features: any, env?: object): string;
   /**
    * extract the feature metadata from a genbank clr feature object
    * 
    * 
     * @param features a collection of the genbank feature object for extract its metadata 
     *  information.
     * @param attrName 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a named list of the feature metadata or a character vector of the 
     *  specific qualifier value;
     *  
     *  this function returns a R# error message object if the input features 
     *  data can not be cast to a collection of the genbank feature object.
   */
   function featureMeta(features: any, attrName?: string, env?: object): string;
   module getRNA {
      /**
       * get all of the RNA gene its gene sequence in fasta sequence format.
       * 
       * 
        * @param gb the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for extract 
        *  its RNA gene sequence data.
        * @return a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) RNA gene sequence collection object.
      */
      function fasta(gb: object): object;
   }
   module is {
      /**
       * check of the given genbank assembly is the data source of a plasmid or not?
       * 
       * 
        * @param gb the ncbi genbank assembly data for make the plasmid source check, 
        *  which can be a vector of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) object or a named 
        *  list of this assembly object.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a boolean value of the plasmid source check result when the input is a 
        *  vector of the genbank assembly object, or a named list of the check 
        *  result when the input is a named list;
        *  
        *  this function returns a R# error message object if the input genbank 
        *  data can not be cast to a collection of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) 
        *  object.
      */
      function plasmid(gb: any, env?: object): boolean;
   }
   /**
    * populate a list of genbank data objects from a given list of files or stream.
    * 
    * > this function supports of read assembly data directly from *.gz genbank archive file.
    * 
     * @param files a list of files or file stream
     * @param extract_genomics only returns the genomics chromosome data? set this parameter value to TRUE will filter out the plasmid, mitochondrion, plastid type sequence data.
     *  set this parameter value to TRUE if use this data source for pan-genome analysis.
     * 
     * + default value Is ``false``.
     * @param autoClose auto close of the [Stream](cref:T:System.IO.Stream) if the **files** contains stream object?
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a lazy pipeline collection of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank 
     *  assembly object;
     *  
     *  this function returns a R# error message object if the given file list 
     *  is nothing.
   */
   function load_genbanks(files: any, extract_genomics?: boolean, autoClose?: boolean, env?: object): object;
   /**
    * get the molecule type evidence annotation of the given genbank 
    *  assembly
    * 
    * 
     * @param gb a collection of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly 
     *  object for get its molecule type evidence.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [MolTypeEvidence](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.MolTypeEvidence) molecule type evidence 
     *  object;
     *  
     *  this function returns a R# error message object if the input genbank 
     *  data can not be cast to a collection of the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) 
     *  object.
   */
   function moltype(gb: any, env?: object): object;
   /**
    * get, add or replace the genome origin fasta sequence in the given genbank assembly file.
    * 
    * 
     * @param gb the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for get, add 
     *  or replace its genome origin sequence data.
     * @param nt the fasta sequence object for update the genome origin sequence data.
     * 
     * + default value Is ``null``.
     * @param mol_type the molecule type of the genome origin sequence, default value is 
     *  ``genomic DNA``.
     * 
     * + default value Is ``'genomic DNA'``.
     * @return if the ``**nt**`` parameter is nothing, 
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
     * @param gb the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for get or 
     *  set its CDS protein sequence data.
     * @param proteins set the genbank feature CDS protein sequence if this value is existsed.
     * 
     * + default value Is ``null``.
     * @param title the fasta headers title template string for export of the protein 
     *  sequence data.
     * 
     * + default value Is ``null``.
     * @param filter_empty Filter out the empty protein sequence when do export of the protein sequence data.
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a collection of the [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) protein sequence data or a 
     *  lazy pipeline collection of this sequence data when the ``proteins`` 
     *  parameter is not specified, or the modified 
     *  [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object when the 
     *  ``proteins`` parameter is specified;
     *  
     *  this function returns a R# error message object if the given protein 
     *  sequence data can not be cast to a fasta sequence collection.
   */
   function protein_seqs(gb: object, proteins?: any, title?: string, filter_empty?: boolean, env?: object): object;
   module read {
      /**
       * read the given genbank assembly file.
       * 
       * 
        * @param file the file path of the given genbank assembly file.
        * @param repliconTable read the replicon table data of the genbank assembly file?
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object that read from 
        *  the given genbank assembly file;
        *  
        *  this function returns a R# error message object if the given file is 
        *  not exists.
      */
      function genbank(file: string, repliconTable?: boolean, env?: object): object;
   }
   /**
    * read gene table from the given csv tabular data file
    * 
    * 
     * @param file the file path of the target csv table file, which could be de-serialized as [GeneTable](cref:T:SMRUCC.genomics.ComponentModel.Annotation.GeneTable) array.
     * @return a vector of the [GeneTable](cref:T:SMRUCC.genomics.ComponentModel.Annotation.GeneTable) gene table record object that 
     *  loaded from the given csv table file.
   */
   function read_genetable(file: string): object;
   /**
    * get ncbi taxonomy id from the given genbank assembly file.
    * 
    * 
     * @param gb the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for extract 
     *  its ncbi taxonomy id.
     * @return the ncbi taxonomy id
   */
   function taxon_id(gb: object): object;
   /**
    * extract the taxonomy lineage information from the genbank file
    * 
    * 
     * @param gb a [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for extract its 
     *  taxonomy lineage information.
     * @return a [genbankKit.taxonomy()](cref:M:seqtoolkit.genbankKit.taxonomy(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File)) taxonomy lineage object.
   */
   function taxonomy_lineage(gb: object): object;
   module write {
      /**
       * save the modified genbank file
       * 
       * 
        * @param gb the [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object for save to 
        *  the target file.
        * @param file the file path of the genbank assembly file to write data.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a boolean value of the file save result: TRUE means the genbank 
        *  assembly data has been written into the target file successfully;
        *  
        *  this function returns a R# error message object if the input genbank 
        *  assembly data is nothing.
      */
      function genbank(gb: object, file: string, env?: object): boolean;
   }
}
