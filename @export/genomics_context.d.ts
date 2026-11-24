// export R# package module type define for javascript/typescript language
//
//    imports "genomics_context" from "seqtoolkit";
//
// ref=seqtoolkit.context@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the tools for processing of the genomics context information
 * 
*/
declare namespace genomics_context {
   /**
    * Create a new context model of a specific genomics feature site.
    * 
    * 
     * @param loci the location of the target genomics feature site, which can be a gene 
     *  model object([IGeneBrief](cref:T:SMRUCC.genomics.ComponentModel.Annotation.IGeneBrief)), a contig object([Contig](cref:T:SMRUCC.genomics.SequenceModel.NucleotideModels.Contig)), 
     *  or a [NucleotideLocation](cref:T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation) nucleotide location object.
     * @param distance the distance value in nucleotide for the context model build.
     * @param note the note text of the generated context model. If this parameter is not 
     *  specified, then the text of the given loci object will be used.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a [Context](cref:T:SMRUCC.genomics.ContextModel.Context) genomics feature site context model object;
     *  
     *  this function returns a R# error message object if the given location 
     *  data can not be cast to a nucleotide location object.
   */
   function context(loci: any, distance: object, note?: string, env?: object): object;
   /**
    * make the chromosome mapping of the blastn hits result data
    * 
    * 
     * @param blastn a collection of the blastn tabular format hit record data 
     *  ([HitRecord](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast.HitRecord)) for make the chromosome mapping.
     * @param eval_thres the maximum e-value threshold of the accepted blastn hits.
     * 
     * + default value Is ``1.7976931348623157E+308``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the chromosome mapping of the accepted blastn hits;
     *  
     *  this function returns a R# error message object if the input blastn 
     *  hits data can not be cast to a collection of the [HitRecord](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast.HitRecord) 
     *  object.
   */
   function context_location(blastn: any, eval_thres?: number, env?: object): object;
   /**
    * filter genes by given strand direction
    * 
    * 
     * @param genes a collection of the gene model object which is subclass of [IGeneBrief](cref:T:SMRUCC.genomics.ComponentModel.Annotation.IGeneBrief)
     * @param strand the nucleotide sequence strand direction, value could be +, -, forward, reverse.
     * 
     * + default value Is ``'+'``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the gene model object that its location strand direction 
     *  matches the given strand value.
   */
   function filter_strand(genes: any, strand?: any, env?: object): any;
   /**
    * build the genomics context model object from a given gff3 annotation 
    *  table
    * 
    * 
     * @param gff a [GFFTable](cref:T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable) genomics feature annotation table object for 
     *  build the genomics context model.
     * @param chr_name the chromosome name for build the context model of the specific 
     *  chromosome. If this parameter is not specified, then context models of 
     *  all chromosomes will be returned.
     * 
     * + default value Is ``null``.
     * @param strict throw an R# error message when the given chromosome name can not be 
     *  found in the gff table? if this parameter is FALSE, then just a 
     *  warning message will be pushed and a NULL value will be returned.
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a [GenomeContext](cref:T:SMRUCC.genomics.ContextModel.GenomeContext%601) genomics context object when there is 
     *  only one chromosome in the given gff table or the ``chr_name`` 
     *  parameter is specified, or a named list of the context object of each 
     *  chromosome.
   */
   function genomics_context(gff: object, chr_name?: string, strict?: boolean, env?: object): object;
   module is {
      /**
       * assert that does the given nucleotide location is in forward direction?
       * 
       * 
        * @param loci a target nucleotide location
        * @return a boolean value: TRUE means the given nucleotide location is in the 
        *  forward strand direction, FALSE otherwise.
      */
      function forward(loci: object): boolean;
   }
   /**
    * create a new nucleotide location object
    * 
    * 
     * @param left the left/start position of the location region.
     * @param right the right/end position of the location region.
     * @param strand the strand direction of the location region, value could be ``+``, 
     *  ``-``, ``forward`` or ``reverse``.
     * 
     * + default value Is ``null``.
     * @return a [NucleotideLocation](cref:T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation) nucleotide location object.
   */
   function location(left: object, right: object, strand?: any): object;
   /**
    * do offset of the given location
    * 
    * 
     * @param loci the target nucleotide location for make the offset.
     * @param offset the offset value in nucleotide for move the given location region.
     * @return a new [NucleotideLocation](cref:T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation) location object that has been 
     *  moved by the given offset value.
   */
   function offset(loci: object, offset: object): object;
   /**
    * evaluate the primer hit coverage of the candidate primer regions on the 
    *  given chromosome context
    * 
    * 
     * @param targetHits a vector of the [NucleotideLocation](cref:T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation) primer hit location 
     *  data on the target chromosome.
     * @param chr a [GenomeContext](cref:T:SMRUCC.genomics.ContextModel.GenomeContext%601) genomics context object of the target 
     *  chromosome, which provides the gene context information of the 
     *  candidate primer regions.
     * @param chr_seq the sequence data slicer object of the target chromosome for extract 
     *  the sequence region data, which can be created by the ``slicer`` api.
     * @return a vector of the [PrimerCoverage](cref:T:SMRUCC.genomics.Analysis.PrimerDesigner.PrimerCoverage) primer hit coverage 
     *  analysis result object.
   */
   function primer_coverage(targetHits: object, chr: object, chr_seq: object): object;
   /**
    * get the segment relationship of two location
    * 
    * 
     * @param a the first location for evaluate the segment relationship, which can be 
     *  a gene model object([IGeneBrief](cref:T:SMRUCC.genomics.ComponentModel.Annotation.IGeneBrief)), a contig 
     *  object([Contig](cref:T:SMRUCC.genomics.SequenceModel.NucleotideModels.Contig)), or a [NucleotideLocation](cref:T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation) 
     *  nucleotide location object.
     * @param b the second location for evaluate the segment relationship, which 
     *  accepts the same data models as the ``a`` parameter.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a [SegmentRelationships](cref:T:SMRUCC.genomics.ComponentModel.Loci.SegmentRelationships) segment relationship enum value 
     *  of the given two location regions;
     *  
     *  this function returns a R# error message object if the given location 
     *  data can not be cast to a nucleotide location object.
   */
   function relationship(a: any, b: any, env?: object): object;
   /**
    * set genomics context of the matched motif site
    * 
    * 
     * @param sites a collection of the motif sites
     * @param genomics the genomics feature information as the context for make location assignment.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [VirtualFootprint](cref:T:SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.VirtualFootprint) motif site object that 
     *  its location region data has been assigned with the genomics context 
     *  information;
     *  
     *  this function returns a R# error message object if the input motif 
     *  sites data can not be cast to a collection of the 
     *  [VirtualFootprint](cref:T:SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.VirtualFootprint) object.
   */
   function set_context(sites: any, genomics: object, env?: object): any;
   /**
    * get TSS upstream site sequence data
    * 
    * 
     * @param genome the genome reference sequence data source, which can be a 
     *  [File](cref:T:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File) ncbi genbank assembly object or a 
     *  [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) nucleotide sequence object.
     * @param genes gene list could be omit if the input genome data is a ncbi genbank model object. 
     *  then all gene features inside the input genbank assembly will be used for export of 
     *  the TSS upstream site.
     * 
     * + default value Is ``null``.
     * @param upstream_len the length in nucleotide of the TSS upstream region.
     * 
     * + default value Is ``150``.
     * @param simple_title generate the fasta headers title of the output sequence in a simple 
     *  format?
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) TSS upstream region nucleotide 
     *  sequence data, one sequence element for each of the target gene;
     *  
     *  this function returns NULL if the input genome data is nothing, or a R# 
     *  error message object if the given genome data type is not supported.
   */
   function TSS_upstream(genome: any, genes?: any, upstream_len?: object, simple_title?: boolean, env?: object): any;
}
