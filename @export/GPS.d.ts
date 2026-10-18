// export R# package module type define for javascript/typescript language
//
//    imports "GPS" from "biosystem";
//
// ref=biosystem.gpsTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * GNN+Transformer as GRN model
 * 
*/
declare namespace GPS {
   /**
     * @param config default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function build_gps(x: object, prior: object, config?: object, env?: object): object;
   /**
   */
   function gps_config(): object;
   /**
     * @param env default value Is ``null``.
   */
   function train(gps: object, env?: object): object;
}
