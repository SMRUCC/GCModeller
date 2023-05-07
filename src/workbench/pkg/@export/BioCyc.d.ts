declare namespace BioCyc {
   module open {
      /**
      */
      function biocyc(repo:string): any;
   }
   /**
   */
   function getCompounds(repo:object): any;
   /**
   */
   function formula(meta:object): any;
   /**
   */
   function createBackground(biocyc:object): any;
}
