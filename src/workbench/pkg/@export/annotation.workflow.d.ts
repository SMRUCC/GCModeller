declare namespace annotation.workflow {
   module read {
      /**
        * @param type default value is ``'nucl'``.
        * @param fastMode default value is ``true``.
        * @param env default value is ``null``.
      */
      function blast(file:string, type?:string, fastMode?:boolean, env?:object): any;
   }
   module blastn {
      /**
        * @param top_best default value is ``false``.
        * @param env default value is ``null``.
      */
      function maphit(query:object, top_best?:boolean, env?:object): any;
   }
   module blasthit {
      /**
        * @param idetities default value is ``0.3``.
        * @param coverage default value is ``0.5``.
        * @param topBest default value is ``false``.
        * @param keepsRawName default value is ``false``.
        * @param env default value is ``null``.
      */
      function sbh(query:object, idetities?:number, coverage?:number, topBest?:boolean, keepsRawName?:boolean, env?:object): any;
      /**
        * @param algorithm default value is ``null``.
        * @param env default value is ``null``.
      */
      function bbh(forward:object, reverse:object, algorithm?:object, env?:object): any;
   }
   module grep {
      /**
        * @param applyOnHits default value is ``false``.
        * @param env default value is ``null``.
      */
      function names(query:object, operators:any, applyOnHits?:boolean, env?:object): any;
   }
   module stream {
      /**
        * @param env default value is ``null``.
      */
      function flush(data:object, stream:any, env?:object): any;
   }
   module besthit {
      /**
        * @param evalue default value is ``1E-05``.
        * @param delNohits default value is ``true``.
        * @param pickTop default value is ``false``.
        * @param env default value is ``null``.
      */
      function filter(besthits:object, evalue?:number, delNohits?:boolean, pickTop?:boolean, env?:object): any;
   }
   module open {
      /**
        * @param type default value is ``null``.
        * @param encoding default value is ``null``.
        * @param ioRead default value is ``false``.
        * @param env default value is ``null``.
      */
      function stream(file:string, type?:object, encoding?:object, ioRead?:boolean, env?:object): any;
   }
}
