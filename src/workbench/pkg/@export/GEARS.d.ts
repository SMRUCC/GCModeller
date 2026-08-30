// export R# package module type define for javascript/typescript language
//
//    imports "GEARS" from "biosystem";
//
// ref=biosystem.gearsTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace GEARS {
   /**
   */
   function new(x: object, prior: object, config: object): object;
   /**
   */
   function train(gears: object): object;
   /**
    * Set the training sample set
    * 
    * 
     * @param gears -
     * @param x -
     * @param controls -
     * @param perturbed -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function training_set(gears: object, x: object, controls: any, perturbed: any, env?: object): any;
}
