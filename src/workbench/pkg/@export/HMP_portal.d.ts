declare namespace HMP_portal {
   /**
     * @param env default value is ``null``.
   */
   function fetch(files:any, outputdir:string, env?:object): any;
   module read {
      /**
        * @param env default value is ``null``.
      */
      function manifest(file:string, env?:object): any;
   }
}
