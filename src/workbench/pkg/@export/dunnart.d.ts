// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.dunnart@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace dunnart {
   module as {
      /**
        * @param colorSet default value Is ``'Paired:c12'``.
        * @param group_key default value Is ``'map'``.
        * @param fillOpacity default value Is ``0.5``.
        * @param lighten default value Is ``0.1``.
      */
      function graphObj(network:object, colorSet?:string, group_key?:string, fillOpacity?:number, lighten?:number): object;
   }
   /**
     * @param desc default value Is ``false``.
     * @param colorSet default value Is ``'Paired:c12'``.
     * @param fillOpacity default value Is ``0.5``.
     * @param lighten default value Is ``0.1``.
     * @param isConnected default value Is ``true``.
   */
   function network_map(template:object, maps:object, desc?:boolean, colorSet?:string, fillOpacity?:number, lighten?:number, isConnected?:boolean): object;
   /**
     * @param optmize_iterations default value Is ``100``.
     * @param lower_degrees default value Is ``3``.
     * @param lower_adjcents default value Is ``2``.
   */
   function optmize(template:object, optmize_iterations?:object, lower_degrees?:object, lower_adjcents?:object): object;
}
