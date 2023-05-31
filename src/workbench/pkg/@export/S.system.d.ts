// export R# package module type define for javascript/typescript language
//
//    imports "S.system" from "simulators";
//
// ref=simulators.SSystemKit@simulators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * S-system toolkit
 * 
*/
declare namespace S.system {
   /**
     * @param env default value Is ``null``.
   */
   function bounds(kernel: object, bounds: object, env?: object): object;
   /**
    * config the symbol environment for S-system kernel
    * 
    * 
     * @param kernel -
     * @param symbols -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function environment(kernel: object, symbols: any, env?: object): object;
   /**
    * create a new S-system dynamics kernel module
    * 
    * 
     * @param snapshot -
     * @param model -
     * 
     * + default value Is ``null``.
     * @param strict 
     * + default value Is ``true``.
   */
   function kernel(snapshot: object, model?: object, strict?: boolean): object;
   /**
    * run simulator
    * 
    * 
     * @param kernel -
     * @param ticks -
     * 
     * + default value Is ``100``.
     * @param resolution -
     * 
     * + default value Is ``0.1``.
   */
   function run(kernel: object, ticks?: object, resolution?: number): object;
   module s {
      /**
       * load S-system into the dynamics simulators kernel module
       * 
       * 
        * @param kernel -
        * @param ssystem -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function system(kernel: object, ssystem: any, env?: object): object;
   }
   module S {
      /**
       * create a new empty model for run S-system simulation
       * 
       * 
        * @param title 
        * + default value Is ``'unnamed model'``.
        * @param description 
        * + default value Is ``''``.
      */
      function script(title?: string, description?: string): object;
   }
   /**
    * create a symbol data snapshot device for write data into file
    * 
    * 
     * @param file -
     * 
     * + default value Is ``null``.
     * @param symbols -
     * 
     * + default value Is ``null``.
   */
   function snapshot(file?: string, symbols?: string): object;
}
