declare namespace S.system {
   module S {
      /**
        * @param title default value is ``'unnamed model'``.
        * @param description default value is ``''``.
      */
      function script(title?:string, description?:string): any;
   }
   /**
     * @param model default value is ``null``.
     * @param strict default value is ``true``.
   */
   function kernel(snapshot:object, model?:object, strict?:boolean): any;
   /**
     * @param env default value is ``null``.
   */
   function environment(kernel:object, symbols:any, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function bounds(kernel:object, bounds:object, env?:object): any;
   module s {
      /**
        * @param env default value is ``null``.
      */
      function system(kernel:object, ssystem:any, env?:object): any;
   }
   /**
     * @param ticks default value is ``100``.
     * @param resolution default value is ``0.1``.
   */
   function run(kernel:object, ticks?:object, resolution?:number): any;
   /**
     * @param file default value is ``null``.
     * @param symbols default value is ``null``.
   */
   function snapshot(file?:string, symbols?:string): any;
}
