// export R# package module type define for javascript/typescript language
//
//    imports "annotation.genomics" from "seqtoolkit";
//
// ref=seqtoolkit.genomics@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Genomics context data annotation toolkit
 * 
 * > This R# package module provides the api for read/write and manipulate the 
 * >  genomics context annotation data:
 * >  
 * >  + read the genomics context annotation table file: ``read.gff``, 
 * >    ``read.gtf``, ``read.nucmer``;
 * >  + export the genomics context annotation data: ``write.gff3``, 
 * >    ``as.tabular``, ``write.PTT_tabular``;
 * >  + query the genomics context features: ``gff_features``, 
 * >    ``source_features``, ``type_features``, ``genes_features``, ``upstream``;
 * >  + extract the sequence region data: ``extract_gff_seqs``.
*/
declare namespace annotation.genomics {
   module as {
      /**
       * export the PTT tabular table object as a collection of the gene table 
       *  records
       * 
       * 
        * @param PTT a [PTT](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT) tabular table object for export as the gene table 
        *  records.
        * @return a vector of the [GeneTable](cref:T:SMRUCC.genomics.ComponentModel.Annotation.GeneTable) gene table record object.
      */
      function geneTable(PTT: object): object;
      /**
       * export the genbank database file object as a PTT tabular table object
       * 
       * 
        * @param gb a [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank database file object for export 
        *  as the PTT tabular table.
        * @return a [PTT](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT) tabular table object of the gene annotation data 
        *  in the given genbank database file.
      */
      function PTT(gb: object): object;
      /**
       * export the gene annotation data as the tabular format table object
       * 
       * 
        * @param genes a vector of the [GeneBrief](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) gene annotation object for 
        *  export as the tabular table.
        * @param title the title text of the generated tabular table.
        * 
        * + default value Is ``'n/a'``.
        * @param size the genome size in bp of the generated tabular table.
        * 
        * + default value Is ``0``.
        * @param format the tabular table format of the generated output. Only the ``PTT`` 
        *  format is implemented at this moment.
        * 
        * + default value Is ``'PTT|GFF|GTF'``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a [PTT](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT) tabular table object of the given gene annotation 
        *  data;
        *  
        *  this function returns a R# error message object if the given table 
        *  format is not supported.
      */
      function tabular(genes: object, title?: string, size?: object, format?: string, env?: object): object;
   }
   /**
    * extract the sequence region data of the genomics features in the given 
    *  gff3 table from the reference sequence data
    * 
    * 
     * @param gff3 a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table object, 
     *  which provides the location region information of the target sequence 
     *  extraction.
     * @param seqs the reference sequence data source for extract the feature region 
     *  sequence, which can be a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) object, a collection 
     *  of the [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object, or a character vector of the raw 
     *  sequence data.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) feature region sequence object;
     *  
     *  this function returns NULL if the given reference sequence data can 
     *  not be cast to a fasta sequence collection.
   */
   function extract_gff_seqs(gff3: object, seqs: any, env?: object): any;
   /**
    * Extract all gene features from a given genomics context assembly data
    * 
    * 
     * @param genome the genomics context assembly data for extract its gene features, 
     *  which can be a [PTT](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT) tabular table object or a 
     *  [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank database file object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [GeneBrief](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) gene annotation object;
     *  
     *  this function returns a R# error message object if the given genomics 
     *  context data is not a supported data model.
   */
   function genes_features(genome: any, env?: object): object;
   /**
    * get gff features by id reference
    * 
    * 
     * @param gff a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table object for 
     *  query the feature data.
     * @param id a character vector of the feature id for get the subset of the target 
     *  features. If this parameter is not specified, then all of the features 
     *  inside the given gff table will be returned.
     * 
     * + default value Is ``null``.
     * @return a vector of the [Feature](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.Feature) genomics feature object.
   */
   function gff_features(gff: object, id?: any): any;
   module read {
      /**
       * read the gff3 file
       * 
       * 
        * @param file the file path of the gff3 format genomics context annotation table 
        *  file.
        * @return a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table object that 
        *  loaded from the given gff3 table file.
      */
      function gff(file: string): object;
      /**
       * read the gtf format genomics context annotation table file
       * 
       * 
        * @param file the file path of the gtf format genomics context annotation table 
        *  file.
        * @return a vector of the [GeneBrief](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) gene annotation object that 
        *  loaded from the given gtf table file.
      */
      function gtf(file: string): object;
      /**
       * read the nucmer alignment delta format file
       * 
       * 
        * @param file the file path of the nucmer alignment delta format file.
        * @return a [DeltaFile](cref:T:SMRUCC.genomics.Visualize.SyntenyVisualize.DeltaFile) nucmer alignment result object that loaded 
        *  from the given delta file.
      */
      function nucmer(file: string): object;
   }
   /**
    * get the genomics features from the given gff table by its source name
    * 
    * 
     * @param gff a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table object for 
     *  query the feature data.
     * @param source the source name of the target features, example as ``ena``.
     * @return a vector of the [Feature](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.Feature) genomics feature object that its 
     *  source name matches the given source name.
   */
   function source_features(gff: object, source: string): object;
   /**
    * get the genomics features from the given gff table by its feature type
    * 
    * 
     * @param gff a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table object for 
     *  query the feature data.
     * @param type the feature type of the target features, example as ``gene``, ``CDS`` 
     *  or ``mRNA``.
     * @return a vector of the [Feature](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.Feature) genomics feature object that its 
     *  feature type matches the given type name.
   */
   function type_features(gff: object, type: string): object;
   /**
    * Create the upstream location
    * 
    * 
     * @param context th gene element location context data
     * @param length bit length of the upstream location
     * 
     * + default value Is ``200``.
     * @param is_relative_offset Does the generates context upstream location is relative to the 
     *  given context start position or the enitre context region move
     *  by upstream offset bits?
     * 
     * + default value Is ``true``.
     * @return a vector of the [NucleotideLocation](cref:T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation) upstream region 
     *  location of each gene in the given gene context data collection.
   */
   function upstream(context: object, length?: object, is_relative_offset?: boolean): object;
   module write {
      /**
       * save the genomics feature annotation table object as a gff3 format 
       *  table file
       * 
       * 
        * @param gff a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table object for 
        *  save to the target file.
        * @param file the file path of the generated gff3 format table file.
        * @return a boolean value of the file save result: TRUE means the genomics 
        *  feature annotation data has been written into the target file 
        *  successfully.
      */
      function gff3(gff: object, file: string): boolean;
      /**
       * write the genomics context annotation data as the PTT tabular format 
       *  table file
       * 
       * 
        * @param genomics the genomics context annotation data for write, which can be a 
        *  [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank database file object, a 
        *  [PTT](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT) tabular table object, or a collection of the 
        *  [GeneBrief](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) gene annotation object.
        * @param file the file path of the generated PTT tabular table file. If this 
        *  parameter is not specified, then the table data will be written into 
        *  the standard output console.
        * 
        * + default value Is ``null``.
        * @param encoding the text encoding value of the generated PTT tabular table file.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a ZERO number value when the tabular table data has been written 
        *  successfully;
        *  
        *  this function returns a R# error message object if the given genomics 
        *  context data is nothing or can not be cast to a supported data model.
      */
      function PTT_tabular(genomics: any, file?: string, encoding?: object, env?: object): any;
   }
}
