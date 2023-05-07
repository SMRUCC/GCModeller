// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.network

/**
 * 
*/
declare namespace network {
   /**
     * @param compounds default value is ``null``.
     * @param enzymeBridged default value is ``true``.
   */
   function fromCompounds(compoundsId:string, graph:object, compounds?:object, enzymeBridged?:boolean): any;
   /**
   */
   function assignKeggClass(g:object): any;
}
