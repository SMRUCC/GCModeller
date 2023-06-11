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
        * @param env default value Is ``null``.
      */
      function compartments(sbml: any, env?: object): any;
   }
   module read {
      /**
      */
      function sbml(file: string): any;
   }
}
