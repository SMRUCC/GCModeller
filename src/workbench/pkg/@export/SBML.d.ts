// export R# package module type define for javascript/typescript language
//
//    imports "SBML" from "biosystem";
//
// ref=biosystem.SBMLTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace SBML {
   module extract {
      /**
        * @param json default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function compartments(sbml: any, json?: boolean, env?: object): any;
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
      function sbml(file: string): any;
   }
}
