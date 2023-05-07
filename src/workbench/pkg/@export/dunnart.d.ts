// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.dunnart

/**
*/
declare namespace dunnart {
   module as {
      /**
        * @param colorSet default value is ``'Paired:c12'``.
        * @param group_key default value is ``'map'``.
        * @param fillOpacity default value is ``0.5``.
        * @param lighten default value is ``0.1``.
      */
      function graphObj(network:object, colorSet?:string, group_key?:string, fillOpacity?:number, lighten?:number): any;
   }
   /**
     * @param desc default value is ``false``.
     * @param colorSet default value is ``'Paired:c12'``.
     * @param fillOpacity default value is ``0.5``.
     * @param lighten default value is ``0.1``.
     * @param isConnected default value is ``true``.
   */
   function network_map(template:object, maps:object, desc?:boolean, colorSet?:string, fillOpacity?:number, lighten?:number, isConnected?:boolean): any;
   /**
     * @param optmize_iterations default value is ``100``.
     * @param lower_degrees default value is ``3``.
     * @param lower_adjcents default value is ``2``.
   */
   function optmize(template:object, optmize_iterations?:object, lower_degrees?:object, lower_adjcents?:object): any;
}
