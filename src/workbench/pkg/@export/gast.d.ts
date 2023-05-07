declare namespace gast {
   module OTU {
      /**
        * @param min_pct default value is ``0.97``.
        * @param gast_consensus default value is ``false``.
        * @param env default value is ``null``.
      */
      function taxonomy(blastn:any, OTUs:object, taxonomy:object, min_pct?:number, gast_consensus?:boolean, env?:object): any;
   }
   module parse {
      /**
      */
      function greengenes_tax(tax:string): any;
      /**
        * @param removes_lt default value is ``0.0001``.
      */
      function mothur_OTUs(OTU_rep_fasta:string, removes_lt?:number): any;
   }
}
