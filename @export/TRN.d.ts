// export R# package module type define for javascript/typescript language
//
//    imports "TRN" from "cytoscape";
//
// ref=cytoscape_toolkit.TRN@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace TRN {
   module fpkm {
      /**
        * @param cutoff default value Is ``0.65``.
      */
      function connections(fpkm: object, cutoff?: number): object;
   }
   /**
     * @param cache default value Is ``2048``.
     * @param env default value Is ``null``.
   */
   function open_bicor(repo: string, cache?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function open_modules(repo: string, env?: object): object;
   /**
     * @param type default value Is ``null``.
     * @param wgcnaOpts default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function write_bicor(x: object, repo: string, type?: object, wgcnaOpts?: object, env?: object): any;
}
