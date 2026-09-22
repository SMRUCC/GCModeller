// export R# package module type define for javascript/typescript language
//
//    imports "bifrost" from "seqtoolkit";
//
// ref=seqtoolkit.bifrost@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace bifrost {
   module as {
      /**
        * @param env default value Is ``null``.
      */
      function genes(x: any, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function gff3(x: any, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function proteins(x: any, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function metaeuk(x: any, env?: object): object;
   /**
     * @param min_ORF_len default value Is ``90``.
     * @param model default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function prodigal(x: any, min_ORF_len?: object, model?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function prodigal_training(x: any, env?: object): object;
}
