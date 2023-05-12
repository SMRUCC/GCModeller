// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.SigmaDifference@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace sigma_difference {
   module genome {
      /**
       * 并行版本的计算函数
       * 
       * 
        * @param genome -
        * @param windowsSize 默认为1kb的长度
        * 
        * + default value Is ``1000``.
      */
      function sigma_diff(genome: object, compare: object, windowsSize?: object): object;
   }
}
