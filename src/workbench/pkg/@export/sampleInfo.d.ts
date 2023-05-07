declare namespace sampleInfo {
   module guess {
      /**
        * @param maxDepth default value is ``false``.
        * @param raw_list default value is ``true``.
      */
      function sample_groups(sample_names:object, maxDepth?:boolean, raw_list?:boolean): any;
   }
   module read {
      /**
        * @param tsv default value is ``false``.
        * @param exclude_groups default value is ``null``.
        * @param id_makenames default value is ``false``.
      */
      function sampleinfo(file:string, tsv?:boolean, exclude_groups?:string, id_makenames?:boolean): any;
   }
   module write {
      /**
      */
      function sampleinfo(sampleinfo:object, file:string): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function sampleInfo(ID:string, sample_name:string, sample_info:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function sampleId(sampleinfo:any, groups:string, env?:object): any;
   module sampleinfo {
      module text {
         /**
         */
         function groups(dir:string): any;
      }
   }
}
