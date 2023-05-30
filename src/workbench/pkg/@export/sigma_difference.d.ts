// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.SigmaDifference@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace sigma_difference {
   module compile {
      /**
       * 
       * > SpeciesID, CAI, CUBIAS_LIST
       * >  src1
       * >  src2
       * >  src3
       * >  ......
       * 
        * @param genes -
        * @param workTEMP 
        * + default value Is ``'./CAI_Xml'``.
      */
      function cai(genes: string, workTEMP?: string): object;
      /**
      */
      function delta_query(source: string, saveCsv: string): boolean;
   }
   module Compile {
      /**
        * @param WorkTemp default value Is ``'./CAI_Xml'``.
      */
      function CAI(genes: object, WorkTemp?: string): object;
   }
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
   module Measure {
      /**
       * measuring the homogeneity property using a specific rule 
       *  sequence between the dnaA and gyrB gene in batch.
       *  
       *  (批量计算比较基因组序列之间的同质性)
       * 
       * 
        * @param PartitionData -
      */
      function Homogeneity(PartitionData: object, Rule: object, St: object, Sp: object): object;
   }
   module Partition {
      module Similarity {
         /**
          * 计算基因组之中的不同的功能分段之间的同质性
          * 
          * 
           * @param data -
         */
         function Calculates(data: object): object;
      }
   }
   module partition_data {
      /**
      */
      function create(besthit: object, partitions: object, allCDSInfo: object, faDIR: string): object;
   }
   module Partitions {
      /**
      */
      function Creates(PartitionRaw: object, TagCol: string, StartTag: string, StopTag: string, Nt: object): object;
   }
   module read {
      module csv {
         /**
         */
         function genome_partition_data(path: string): object;
         /**
         */
         function site_delta(path: string): object;
      }
   }
   module Read {
      module Csv {
         /**
         */
         function Chromsome_Partitioning(path: string): object;
      }
   }
   module rendering_merge {
      /**
       * 要求所有的文件都必须要为同一个基因组比对不同的基因组，不可以改动输出文件的文件名
       * 
       * 
        * @param source -
        * @param samples 
        * + default value Is ``1``.
      */
      function delta_source(source: string, query: object, render_source: string, saveto: string, samples?: object): boolean;
   }
   module sigma_diff {
      /**
       * 
       * 
        * @param query Query基因组的fasta序列的文件路径
        * @param sbjDIR 将要进行比较的基因组的fasta序列文件的存放文件夹
        * @param windowsSize 
        * + default value Is ``1000``.
      */
      function query(query: string, sbjDIR: string, EXPORT: string, windowsSize?: object): boolean;
   }
   module write {
      module csv {
         /**
         */
         function genome_partition_data(dat: object, saveto: string): boolean;
      }
   }
}
