// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.workflows@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * A pipeline collection for proteins' biological function 
 *  annotation based on the sequence alignment.
 * 
*/
declare namespace annotation.workflow {
   module read {
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
      */
      function blast(file: string, type?: string, fastMode?: boolean, env?: object): object;
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
   module blasthit {
      /**
       * Export single side besthit
       * 
       * 
        * @param query -
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
      /**
        * @param algorithm default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function bbh(forward: object, reverse: object, algorithm?: object, env?: object): object;
   }
   module grep {
      /**
        * @param applyOnHits default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function names(query: object, operators: any, applyOnHits?: boolean, env?: object): object;
   }
   module stream {
      /**
        * @param env default value Is ``null``.
      */
      function flush(data: object, stream: any, env?: object): any;
   }
   module besthit {
      /**
        * @param evalue default value Is ``1E-05``.
        * @param delNohits default value Is ``true``.
        * @param pickTop default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function filter(besthits: object, evalue?: number, delNohits?: boolean, pickTop?: boolean, env?: object): object;
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
}
