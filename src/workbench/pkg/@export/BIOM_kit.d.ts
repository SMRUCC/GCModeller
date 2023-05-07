declare namespace BIOM_kit {
   module read {
      /**
        * @param denseMatrix default value is ``true``.
        * @param suppressErr default value is ``false``.
        * @param env default value is ``null``.
      */
      function matrix(file:any, denseMatrix?:boolean, suppressErr?:boolean, env?:object): any;
   }
   module biom {
      /**
        * @param env default value is ``null``.
      */
      function taxonomy(biom:any, env?:object): any;
      /**
        * @param env default value is ``null``.
      */
      function union(tables:any, env?:object): any;
   }
}
