// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.API@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace GCModeller.Property {
   module Write {
      /**
      */
      function Name(model:object, value:string): object;
      /**
      */
      function Comment(model:object, value:string): object;
      /**
      */
      function Species(model:object, value:string): object;
      /**
      */
      function Title(model:object, value:string): object;
      /**
      */
      function DBLinks(model:object, value:object): object;
   }
   module Author {
      /**
      */
      function Add(model:object, value:string): object;
   }
   module EMail {
      /**
      */
      function Add(model:object, value:string): object;
   }
   module Reversion {
      /**
      */
      function Plus(model:object): object;
   }
   module Publication {
      /**
      */
      function Add(model:object, value:string): object;
   }
   module Url {
      /**
      */
      function Add(model:object, value:string): object;
   }
}
