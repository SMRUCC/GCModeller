// export R# package module type define for javascript/typescript language
//
//    imports "multi_omics" from "phenotype_kit"
//
// ref=phenotype_kit.multiOmics@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace multi_omics {
   module omics {
      /**
        * @param xlab default value Is ``'X'``.
        * @param ylab default value Is ``'Y'``.
        * @param size default value Is ``'3000,3000'``.
        * @param padding default value Is ``'padding: 200px 250px 200px 100px;'``.
        * @param ptSize default value Is ``10``.
        * @param env default value Is ``null``.
      */
      function 2D_scatter(x: any, y: any, xlab?: string, ylab?: string, size?: any, padding?: any, ptSize?: number, env?: object): any;
   }
}
