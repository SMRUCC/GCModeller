// export R# package module type define for javascript/typescript language
//
//    imports "SBML" from "biosystem";
//
// ref=biosystem.SBMLTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Systems Biology Markup Language (SBML)
 *  
 *  a free and open data format for computational systems biology 
 *  that’s used by thousands of people worldwide.
 * 
*/
declare namespace SBML {
   module extract {
      /**
        * @param json default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function compartments(sbml: any, json?: boolean, env?: object): any;
      /**
        * @param env default value Is ``null``.
      */
      function pathway_model(sbml: any, env?: object): any;
   }
   /**
     * @param json default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function extract_compounds(sbml: any, json?: boolean, env?: object): any;
   /**
     * @param json default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function extract_reactions(sbml: any, json?: boolean, env?: object): any;
   module read {
      /**
      */
      function sbgn(file: string): object;
      /**
       * Read a sbml model file from a given file path
       * 
       * 
        * @param file -
      */
      function sbml(file: string): object;
   }
}
