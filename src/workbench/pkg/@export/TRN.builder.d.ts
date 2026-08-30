// export R# package module type define for javascript/typescript language
//
//    imports "TRN.builder" from "TRNtoolkit";
//
// ref=TRNtoolkit.TRNBuilder@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for create a transcription regulation network
 * 
 * > This R# package module provides the toolkit for build the transcription 
 * >  regulation network(TRN) from the regulon database and the motif site scan 
 * >  result:
 * >  
 * >  + ``open_motifdb``: open the position weight matrix(PWM) motif database;
 * >  + ``motif_search``: scan the TF binding site(TFBS) motif site on the given 
 * >    promoter/upstream sequence regions;
 * >  + ``regulation.footprint``: create the regulation footprint(regulation network 
 * >    edges) from the regulator mapping data(bbh), the motif site data and the 
 * >    regprecise regulon database;
 * >  + ``read.regulations``/``write.regulations``: read and save the regulation 
 * >    footprint data table;
 * >  + ``read.footprints``: read the motif site(footprint site) table data.
*/
declare namespace TRN.builder {
   /**
    * scan the TF binding site motif on the given sequence regions
    * 
    * 
     * @param db the position weight matrix(PWM) motif database object, which contains the 
     *  motif model of each transcription factor family.
     * @param search_regions the sequence regions for run the motif site scan, which can be a fasta 
     *  sequence collection, a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object or a character vector 
     *  of the raw sequence data, each sequence is a candidate promoter/upstream 
     *  sequence region of one gene.
     * @param family an optional character vector of the transcription factor family name for 
     *  restrict the motif scan: only the motif model of the given families will be 
     *  used for the scan, all of the motif models in the database will be used if 
     *  this parameter is not specified.
     * 
     * + default value Is ``null``.
     * @param pval_cutoff the p-value cutoff of the motif site match: the candidate site that its 
     *  match p-value is greater than this cutoff will be ignored, by default is 
     *  0.05.
     * 
     * + default value Is ``0.05``.
     * @param minW the minimum score ratio cutoff of the motif site match, by default is 0.85.
     *  
     *  NOTE: this parameter is not applied by the current implementation, the motif 
     *  site match result is filtered by the ``pval_cutoff`` and the ``top`` 
     *  parameter only.
     * 
     * + default value Is ``0.85``.
     * @param top the top n best matched site of each motif model on each sequence region, by 
     *  default is 3.
     * 
     * + default value Is ``3``.
     * @param bg the background model of the motif site scan, the uniform background model 
     *  will be used if this parameter is not specified.
     * 
     * + default value Is ``null``.
     * @param scan_reverse scan the motif site on the reverse complement strand of the sequence region 
     *  or not? by default is TRUE.
     * 
     * + default value Is ``true``.
     * @param tqdm_bar display the progress bar of the motif site scan task on the console? by 
     *  default is TRUE.
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifMatch`` motif site match result: the 
     *  ``title`` property is the sequence title of the corresponding sequence 
     *  region, the ``motif`` property is the matched motif model, the ``start``, 
     *  ``ends``, ``strand`` and ``segment`` property is the location and the 
     *  sequence data of the matched site, and the ``score1``, ``score2`` and 
     *  ``pvalue`` property is the match score data;
     *  
     *  this function returns a R# error message object if the given sequence source 
     *  can not be cast to a fasta sequence collection.
   */
   function motif_search(db: object, search_regions: any, family?: any, pval_cutoff?: number, minW?: number, top?: object, bg?: object, scan_reverse?: boolean, tqdm_bar?: boolean, env?: object): object;
   /**
    * open the motif database
    * 
    * > NOTE: the ``db`` parameter of the ``motif_search`` api requires a 
    * >  ``SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.PWMDatabase`` 
    * >  object, but this function returns a ``MEMEMotifRepository`` or a 
    * >  ``SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.PWMDatabase`` 
    * >  object, so the database object that is created by this api can not be 
    * >  consumed by the ``motif_search`` api directly at this moment.
    * 
     * @param file the motif database source:
     *  
     *  1. a directory path that contains a set of the MEME format motif files 
     *     (*.meme), then a @``T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.MEMEMotifRepository`` object will be created 
     *     from this directory;
     *  2. a file path or a file stream object of the binary motif database file, 
     *     then the database will be opened from the given data stream in read only 
     *     mode.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a motif database object for get the position weight matrix data of each 
     *  transcription factor family, which could be used by the ``motif_search`` api 
     *  for run the TFBS motif site scan;
     *  
     *  this function returns a R# error message object if the given motif database 
     *  file can not be opened for read.
   */
   function open_motifdb(file: any, env?: object): object;
   module read {
      /**
       * read a footprint site model data file
       * 
       * 
        * @param file the file path of the footprint site csv table file, which contains the 
        *  motif site location data and the downstream gene information of each site.
        * @return a vector of the @``T:SMRUCC.genomics.Data.Regprecise.FootprintSite`` object that is loaded from the 
        *  given csv table file.
      */
      function footprints(file: string): object;
      /**
       * read a regulation prediction result file
       * 
       * 
        * @param file the file path of the regulation footprint csv table file, which could be 
        *  created by the ``write.regulations`` api.
        * @return a vector of the @``T:SMRUCC.genomics.Data.Regprecise.RegulationFootprint`` 
        *  object that is loaded from the given csv table file, each object is a 
        *  regulation network edge of the regulator to its regulated target gene.
      */
      function regulations(file: string): object;
   }
   module regulation {
      /**
       * create the regulation footprint(regulation network edges) from the regulator 
       *  mapping data, the motif site data and the regprecise regulon database
       * 
       * > only the regulator of the ``TF`` type in the regprecise database will be used 
       * >  for create the regulation network, and the transcription factor family name 
       * >  of the regulator is the first token of the family data which is splitted by 
       * >  the ``/`` or the ``\`` character.
       * >  
       * >  the regulator mapping is created by the bbh best hit: the ``HitName`` of the 
       * >  @``T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit`` data is mapped to the regprecise regulator via its 
       * >  locus id(the text after the last ``:`` character), and the ``QueryName`` is 
       * >  used as the regulator gene id in the target genome.
       * 
        * @param regulators the regulator mapping data, which can be a vector of the 
        *  @``T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit`` object(the bbh best hit mapping result of the 
        *  regulator protein to the target genome), or a pipeline object that produces a 
        *  set of the @``T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit`` data.
        * @param motifLocis a vector of the @``T:SMRUCC.genomics.Data.Regprecise.FootprintSite`` motif site data, which could be 
        *  loaded from a csv table file via the ``read.footprints`` api: the ``src`` 
        *  property of the site data is the transcription factor family name set of the 
        *  corresponding motif site and the ``gene`` property is the regulated target 
        *  gene of the site.
        * @param regprecise the regprecise regulon database object(@``T:SMRUCC.genomics.Data.Regprecise.TranscriptionFactors``), 
        *  which provides the regulator information(the effector, the regulation mode, 
        *  the regulog, the biological process, etc) of each transcription factor 
        *  family.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a pipeline object of the 
        *  @``T:SMRUCC.genomics.Data.Regprecise.RegulationFootprint`` regulation 
        *  network edge data: each edge is a regulation of one regulator to one target 
        *  gene, which is created by mapping the motif site to the regulator of the 
        *  corresponding transcription factor family in the regprecise database, the 
        *  duplicated edge(``{regulator}->{regulated}``) will be removed 
        *  automatically;
        *  
        *  this function returns NULL if the given regulator mapping data is nothing, or 
        *  a R# error message object if the given regulator data is not a collection of 
        *  the @``T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit`` data.
      */
      function footprint(regulators: any, motifLocis: object, regprecise: object, env?: object): object;
   }
   module write {
      /**
       * save the regulation network data file.
       * 
       * 
        * @param regulationFootprints the regulation network edge data for save, which can be a vector of the 
        *  @``T:SMRUCC.genomics.Data.Regprecise.RegulationFootprint`` object, or 
        *  a pipeline object that produces a set of the regulation footprint data.
        * @param file the file path of the generated regulation footprint csv 
        *  table file.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a boolean value for indicates that the regulation network data has been 
        *  saved into the target file successfully or not;
        *  
        *  this function returns a R# error message object if the given data is nothing, 
        *  the output file path is empty, or the given data is not a collection of the 
        *  regulation footprint data.
      */
      function regulations(regulationFootprints: any, file: string, env?: object): any;
   }
}
