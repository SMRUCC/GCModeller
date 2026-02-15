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
     * @param besthits is a collection of the blastp/blastn parsed result: @``T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit``
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
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function besthit_filter(besthits: object, evalue?: object, identities?: object, delNohits?: boolean, pickTop?: boolean, env?: object): object;
   module blasthit {
      /**
        * @param algorithm default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function bbh(forward: object, reverse: object, algorithm?: object, env?: object): object;
      /**
       * Export single side besthit
       * 
       * 
        * @param query the blast reader result from the ``read.blast`` iterator function.
        * @param idetities -
        * 
        * + default value Is ``0.3``.
        * @param coverage -
        * 
        * + default value Is ``0.5``.
        * @param topBest -
        * 
        * + default value Is ``false``.
        * @param keepsRawName -
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function sbh(query: object, idetities?: number, coverage?: number, topBest?: boolean, keepsRawName?: boolean, env?: object): object;
   }
   module blastn {
      /**
       * export results of fastq reads mapping to genome sequence.
       * 
       * 
        * @param query -
        * @param top_best 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function maphit(query: object, top_best?: boolean, env?: object): object;
   }
   module grep {
      /**
        * @param applyOnHits default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function names(query: object, operators: any, applyOnHits?: boolean, env?: object): object;
   }
   module open {
      /**
       * Open result table stream writer
       * 
       * 
        * @param file -
        * @param type -
        * 
        * + default value Is ``null``.
        * @param encoding -
        * 
        * + default value Is ``null``.
        * @param ioRead 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function stream(file: string, type?: object, encoding?: object, ioRead?: boolean, env?: object): any;
   }
   module read {
      /**
       * read the hits data in pipeline stream style
       * 
       * 
        * @param file -
        * @param encoding -
        * 
        * + default value Is ``null``.
      */
      function besthits(file: string, encoding?: object): object;
      /**
       * Open the blast output text file for parse data result.
       * 
       * 
        * @param file -
        * @param type ``nucl`` or ``prot``
        * 
        * + default value Is ``'nucl'``.
        * @param fastMode 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a collection of the query hits result details
      */
      function blast(file: string, type?: string, fastMode?: boolean, env?: object): object;
   }
   /**
    * read the diamond m8 annotation table file output
    * 
    * 
     * @param file -
     * @param stream -
     * 
     * + default value Is ``false``.
   */
   function read_m8(file: string, stream?: boolean): object;
   module stream {
      /**
       * Save the annotation rawdata into the given stream file.
       * 
       * 
        * @param data -
        * @param stream a stream data handler that generated via the ``open.stream`` function.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function flush(data: object, stream: any, env?: object): any;
   }
}
