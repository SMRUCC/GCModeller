// export R# package module type define for javascript/typescript language
//
//    imports "sampleInfo" from "phenotype_kit";
//
// ref=phenotype_kit.DEGSample@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * GCModeller DEG experiment analysis designer toolkit
 * 
*/
declare namespace sampleInfo {
   /**
    * Create new analysis design sample info via formula
    * 
    * 
     * @param sampleinfo -
     * @param designs -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function design(sampleinfo: any, designs?: object, env?: object): object;
   module group {
      /**
       * get/set the group colors
       * 
       * 
        * @param sampleinfo -
        * @param colorSet -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function colors(sampleinfo: object, colorSet?: any, env?: object): any;
   }
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
       * Read the sampleinfo data table from a given csv file
       * 
       * 
        * @param file -
        * @param tsv -
        * 
        * + default value Is ``false``.
        * @param exclude_groups -
        * 
        * + default value Is ``null``.
        * @param id_makenames -
        * 
        * + default value Is ``false``.
      */
      function sampleinfo(file: string, tsv?: boolean, exclude_groups?: string, id_makenames?: boolean): object;
   }
   /**
    * Get sample id collection from a speicifc sample data groups
    * 
    * 
     * @param sampleinfo -
     * @param groups -
     * @param env -
     * 
     * + default value Is ``null``.
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
   module write {
      /**
       * save sampleinfo data as csv file
       * 
       * > You also can save the sampleinfo data directly via the ``write.csv`` function.
       * 
        * @param sampleinfo -
        * @param file -
      */
      function sampleinfo(sampleinfo: object, file: string): boolean;
   }
}
