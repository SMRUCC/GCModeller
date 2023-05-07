// export R# package module type define for javascript/typescript language
//
// ref=phenotype_kit.multiOmics

/**
*/
declare namespace multi_omics {
   module omics {
      /**
        * @param xlab default value is ``'X'``.
        * @param ylab default value is ``'Y'``.
        * @param size default value is ``'3000,3000'``.
        * @param padding default value is ``'padding: 200px 250px 200px 100px;'``.
        * @param ptSize default value is ``10``.
        * @param env default value is ``null``.
      */
      function 2D_scatter(x:any, y:any, xlab?:string, ylab?:string, size?:any, padding?:any, ptSize?:number, env?:object): any;
   }
}
