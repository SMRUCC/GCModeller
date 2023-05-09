// export R# package module type define for javascript/typescript language
//
// ref=phenotype_kit.DEGSample@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * GCModeller DEG experiment analysis designer toolkit
 * 
*/
declare namespace sampleInfo {
   module guess {
      /**
       * try to parse the sampleInfo data from the
       *  sample labels
       * 
       * 
        * @param sample_names -
        * @param maxDepth 
        * + default value Is ``false``.
        * @param raw_list -
        * 
        * + default value Is ``true``.
      */
      function sample_groups(sample_names: object, maxDepth?: boolean, raw_list?: boolean): object|object;
   }
   module read {
      /**
        * @param tsv default value Is ``false``.
        * @param exclude_groups default value Is ``null``.
        * @param id_makenames default value Is ``false``.
      */
      function sampleinfo(file: string, tsv?: boolean, exclude_groups?: string, id_makenames?: boolean): object;
   }
   module write {
      /**
      */
      function sampleinfo(sampleinfo: object, file: string): boolean;
   }
   /**
    * create ``sample_info`` data table
    * 
    * 
     * @param ID -
     * @param sample_name -
     * @param sample_info -
     * @param env 
     * + default value Is ``null``.
   */
   function sampleInfo(ID: string, sample_name: string, sample_info: string, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function sampleId(sampleinfo: any, groups: string, env?: object): string;
   module sampleinfo {
      module text {
         /**
          * Create sampleInfo table from text files
          * 
          * 
           * @param dir -
         */
         function groups(dir: string): object;
      }
   }
}
