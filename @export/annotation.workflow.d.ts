// export R# package module type define for javascript/typescript language
//
//    imports "annotation.workflow" from "seqtoolkit";
//
// ref=seqtoolkit.workflows@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * A pipeline collection for proteins' biological function 
 *  annotation based on the sequence alignment.
 * 
*/
declare namespace annotation.workflow {
   /**
    * make filter of the blast best hits via the given parameter combinations
    * 
    * 
     * @param besthits is a collection of the blastp/blastn parsed result: [BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)
     * @param evalue new cutoff value of the evalue for make filter of the given hits collection
     * 
     * + default value Is ``null``.
     * @param identities 
     * + default value Is ``null``.
     * @param delNohits removes ``HITS_NOT_FOUND``? default is yes.
     * 
     * + default value Is ``true``.
     * @param pickTop pick the top one hit for each query group?
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a filtered lazy pipeline collection of the [BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit) best 
     *  hit data.
   */
   function besthit_filter(besthits: object, evalue?: object, identities?: object, delNohits?: boolean, pickTop?: boolean, env?: object): object;
   /**
    * cast the blastn tabular format hits result as a data frame object
    * 
    * 
     * @param hits a vector of the blastn tabular format hit record 
     *  ([HitRecord](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast.HitRecord)) for cast as the data frame.
     * @param args the additional arguments for the data frame cast, this parameter is 
     *  not used in this function.
     * @param env the R# runtime environment object.
     * @return a data frame object of the blastn hits result: each row is one hit, 
     *  and the columns are the hit details: ``query_id``, ``subject_id``, 
     *  ``identities``, ``alignment_length``, ``mis_matches``, ``gap_opens``, 
     *  ``query_start``, ``query_end``, ``subject_start``, ``subject_end``, 
     *  ``e_value`` and ``bit_score``.
   */
   function blast_tabular(hits: object, args: object, env: object): any;
   module blasthit {
      /**
       * export the bi-directional best hit(BBH) result from the given forward 
       *  and reverse best hit data streams
       * 
       * 
        * @param forward a lazy pipeline collection of the forward direction hits data, which 
        *  could be the raw query([Query](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query)) stream, the single side 
        *  best hit([BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)) stream, or the diamond m8 annotation 
        *  ([DiamondAnnotation](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.DiamondAnnotation)) stream.
        * @param reverse a lazy pipeline collection of the reverse direction hits data, which 
        *  accepts the same data models as the ``forward`` parameter.
        * @param algorithm the BBH match algorithm of the bi-directional best hit search.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a lazy pipeline collection of the [BiDirectionalBesthit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit) 
        *  bi-directional best hit result;
        *  
        *  this function returns a R# error message object if the input data 
        *  streams are nothing or their element types are not supported.
      */
      function bbh(forward: object, reverse: object, algorithm?: object, env?: object): object;
      /**
       * Export single side besthit
       * 
       * 
        * @param query the blast reader result from the ``read.blast`` iterator function.
        * @param idetities the minimum identity threshold value of the accepted hits.
        * 
        * + default value Is ``0.3``.
        * @param coverage the minimum coverage threshold value of the accepted hits.
        * 
        * + default value Is ``0.5``.
        * @param topBest only export the top best hit for each query?
        * 
        * + default value Is ``false``.
        * @param keepsRawName keep the raw query name text in the exported best hit data?
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a lazy pipeline collection of the [BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit) single side 
        *  best hit result;
        *  
        *  this function returns a R# error message object if the input query 
        *  pipeline data type is not the [Query](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query) object.
      */
      function sbh(query: object, idetities?: number, coverage?: number, topBest?: boolean, keepsRawName?: boolean, env?: object): object;
   }
   module blastn {
      /**
       * export results of fastq reads mapping to genome sequence.
       * 
       * 
        * @param query a lazy pipeline collection of the blast query hits result 
        *  ([Query](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query)), which is created by the ``read.blast`` api.
        * @param top_best only export the top best mapping for each query?
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a lazy pipeline collection of the [BlastnMapping](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping.BlastnMapping) reads 
        *  mapping result;
        *  
        *  this function returns a R# error message object if the input query 
        *  pipeline data type is not the [Query](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query) object.
      */
      function maphit(query: object, top_best?: boolean, env?: object): object;
   }
   /**
    * Make query group and convert to alignment hit collection
    * 
    * 
     * @param x a collection of the [DiamondAnnotation](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.DiamondAnnotation) diamond annotation 
     *  hits data for make the hit group.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [HitCollection](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models.HitCollection) hit group object, one group 
     *  element for each query id.
   */
   function diamond_hitgroups(x: any, env?: object): object;
   /**
    * filter the bi-directional best hit data by removing the low level 
    *  hits(SBH and NA level)
    * 
    * 
     * @param bbh a collection of the [BiDirectionalBesthit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit) bi-directional 
     *  best hit data for make the level filter.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a lazy pipeline collection of the [BiDirectionalBesthit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit) 
     *  object which its level value is neither ``SBH`` nor ``NA``;
     *  
     *  this function returns a R# error message object if the input bbh data 
     *  can not be cast to a collection of the 
     *  [BiDirectionalBesthit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit) object.
   */
   function filter_low_level(bbh: any, env?: object): any;
   module grep {
      /**
       * apply a text grep script on the query name or hit name of the blast 
       *  query result data
       * 
       * 
        * @param query a lazy pipeline collection of the blast query hits result 
        *  ([Query](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query)), which is created by the ``read.blast`` api.
        * @param operators the text grep script for do the name string replacement: a text grep 
        *  script string, or a compiled text grep engine object.
        * @param applyOnHits apply the text grep script on the hit name instead of the query name?
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a lazy pipeline collection of the [Query](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query) blast query 
        *  result which its name data has been processed by the given text grep 
        *  script;
        *  
        *  this function returns a R# error message object if the given text grep 
        *  program is invalid.
      */
      function names(query: object, operators: any, applyOnHits?: boolean, env?: object): object;
   }
   module open {
      /**
       * Open result table stream writer
       * 
       * 
        * @param file the file path of the target result table stream file.
        * @param type the table format type of the target stream data: ``SBH``, ``BBH``, 
        *  ``Mapping`` or ``Terms``.
        * 
        * + default value Is ``null``.
        * @param encoding the text encoding value of the target stream file.
        * 
        * + default value Is ``null``.
        * @param ioRead open the target file in read mode? if this parameter is TRUE, then a 
        *  lazy pipeline collection of the table data will be returned instead of 
        *  the stream writer object.
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a stream writer object for save the annotation data in a stream manner 
        *  via the ``stream.flush`` api, or a lazy pipeline collection of the 
        *  table data when the ``ioRead`` parameter is TRUE.
      */
      function stream(file: string, type?: object, encoding?: object, ioRead?: boolean, env?: object): any;
   }
   module read {
      /**
       * read the bi-directional best hit data in pipeline stream style
       * 
       * 
        * @param file the file path of the bbh bi-directional best hit data table file.
        * @param encoding the text encoding value of the target table file.
        * 
        * + default value Is ``null``.
        * @return a lazy pipeline collection of the [BiDirectionalBesthit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit) 
        *  bi-directional best hit object that loaded from the given table file.
      */
      function bbh_hits(file: string, encoding?: object): object;
      /**
       * read the hits data in pipeline stream style
       * 
       * 
        * @param file the file path of the sbh best hit data table file.
        * @param encoding the text encoding value of the target table file.
        * 
        * + default value Is ``null``.
        * @return a lazy pipeline collection of the [BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit) best hit 
        *  object that loaded from the given table file.
      */
      function besthits(file: string, encoding?: object): object;
      /**
       * Open the blast output text file for parse data result.
       * 
       * 
        * @param file the file path of the blast output text file.
        * @param type ``nucl`` or ``prot``
        * 
        * + default value Is ``'nucl'``.
        * @param fastMode run the blastp output parser in the fast mode?
        * 
        * + default value Is ``true``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a collection of the query hits result details
      */
      function blast(file: string, type?: string, fastMode?: boolean, env?: object): object;
      /**
       * read ncbi blast output format 6 (tabular) file for blastn result mapping to genome sequence
       * 
       * 
        * @param file the input source: a file path of the blast output format 6 tabular 
        *  table file, or a file stream object of the target table file.
        * @param make_query_group group the hits result by the query id? if this parameter is TRUE, then 
        *  a named list of the hits result group will be returned.
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a vector of the [HitRecord](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast.HitRecord) blast hits result object, or a 
        *  named list of the hits result group when the ``make_query_group`` 
        *  parameter is TRUE;
        *  
        *  this function returns a R# error message object if the given file can 
        *  not be opened for read.
      */
      function outfmt6(file: any, make_query_group?: boolean, env?: object): object;
   }
   /**
    * read the diamond m8 annotation table file output
    * 
    * 
     * @param file the file path of the diamond m8 format annotation table file.
     * @param stream read the table data in a lazy stream manner?
     * 
     * + default value Is ``false``.
     * @param filter the keyword for filter the annotation result.
     * 
     * + default value Is ``'unknown'``.
     * @param parseHitId the index number of the token in the hit id text for make the hit name 
     *  parse, a negative value means no parse.
     * 
     * + default value Is ``-1``.
     * @param hitIdDeli the delimiter character for split the hit id text.
     * 
     * + default value Is ``'|'``.
     * @return a vector of the [DiamondAnnotation](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.DiamondAnnotation) diamond annotation 
     *  object, or a lazy pipeline collection of this data when the ``stream`` 
     *  parameter is TRUE.
   */
   function read_m8(file: string, stream?: boolean, filter?: string, parseHitId?: object, hitIdDeli?: string): object;
   /**
    * removes protein suffix id
    * 
    * 
     * @param hits a collection of the blast hits result or a character vector of the protein id.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the cleaned protein id or the blast hits result object 
     *  which its query name and hit name have been trimmed of the protein id 
     *  suffix.
   */
   function remove_protein_suffix(hits: any, env?: object): string|object;
   module stream {
      /**
       * Save the annotation rawdata into the given stream file.
       * 
       * 
        * @param data a lazy pipeline collection of the annotation data for write into the 
        *  target stream file, the supported data element types are: 
        *  [BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit), [BiDirectionalBesthit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit), 
        *  [BlastnMapping](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping.BlastnMapping) and [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm).
        * @param stream a stream data handler that generated via the ``open.stream`` function.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return this function returns a R# error message object if the output stream 
        *  device is nothing, the input data is nothing, or the stream device 
        *  type is not matched with the incoming data type.
      */
      function flush(data: object, stream: any, env?: object): any;
   }
}
