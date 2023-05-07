// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.API

/**
*/
declare namespace GCModeller.Property {
   module Write {
      /**
      */
      function Name(model:object, value:string): any;
      /**
      */
      function Comment(model:object, value:string): any;
      /**
      */
      function Species(model:object, value:string): any;
      /**
      */
      function Title(model:object, value:string): any;
      /**
      */
      function DBLinks(model:object, value:object): any;
   }
   module Author {
      /**
      */
      function Add(model:object, value:string): any;
   }
   module EMail {
      /**
      */
      function Add(model:object, value:string): any;
   }
   module Reversion {
      /**
      */
      function Plus(model:object): any;
   }
   module Publication {
      /**
      */
      function Add(model:object, value:string): any;
   }
   module Url {
      /**
      */
      function Add(model:object, value:string): any;
   }
}
