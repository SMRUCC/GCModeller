// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.PlasmidComparative@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 根据BBH结果所计算出来的保守片段之间进行delta值的相互比较
 * 
*/
declare namespace Comparative.Plasmid {
   module Plasmid {
      /**
      */
      function DeltaMatrix(partitions: object): object;
      /**
      */
      function Partitioning(Besthits: object, CdsInfo: object, Fasta: object): object;
   }
}
