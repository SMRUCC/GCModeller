#Region "Microsoft.VisualBasic::80d3d1ec831f7c740c873b088723b5c7, cuda\ILCuda\Runtime\CudaEngine\KernelNames.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 39
    '    Code Lines: 25 (64.10%)
    ' Comment Lines: 10 (25.64%)
    '    - Xml Docs: 50.00%
    ' 
    '   Blank Lines: 4 (10.26%)
    '     File Size: 1.79 KB


    '     Module KernelNames
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Runtime

    ''' <summary>
    ''' 框架自带内核的函数名（与 Kernels\*.cu 中的 extern "C" 名称一一对应）。
    '''
    ''' demo 或业务方自行注册的内核名不在本表中，应在使用方自己的代码里定义，
    ''' 并通过 <see cref="KernelCatalog"/> 注册元数据。
    ''' </summary>
    Public Module KernelNames
        ' ---- Kernels\basic.cu ----
        Public Const VecAdd As String = "vecAddKernel"
        Public Const Saxpy As String = "saxpyKernel"

        ' ---- Kernels\reduce.cu ----
        Public Const ReduceSum As String = "reduceSumKernel"
        Public Const ReduceMax As String = "reduceMaxKernel"
        Public Const ReduceMin As String = "reduceMinKernel"
        Public Const ReduceFinalSum As String = "reduceFinalSumKernel"
        Public Const ReduceFinalMax As String = "reduceFinalMaxKernel"
        Public Const ReduceFinalMin As String = "reduceFinalMinKernel"

        ' ---- Kernels\elementwise.cu ----
        Public Const EwAdd As String = "ewAddKernel"
        Public Const EwSub As String = "ewSubKernel"
        Public Const EwMul As String = "ewMulKernel"
        Public Const EwDiv As String = "ewDivKernel"
        Public Const EwScale As String = "ewScaleKernel"
        Public Const EwAxpy As String = "ewAxpyKernel"
        Public Const EwRelu As String = "ewReluKernel"
        Public Const EwExp As String = "ewExpKernel"
        Public Const EwLog As String = "ewLogKernel"
        Public Const EwSqrt As String = "ewSqrtKernel"
        Public Const EwAbs As String = "ewAbsKernel"

        ' ---- Kernels\blas.cu ----
        Public Const Gemm As String = "gemmKernel"
        Public Const Gemv As String = "gemvKernel"

        ' ---- Kernels\sort.cu ----
        ' 分段基数排序（值）：每段一个 block，段内升序，NaN 沉到段尾。
        ''' <summary>分段基数排序（单精度）</summary>
        Public Const SortSegmentsF32 As String = "ilcudaSortSegmentsF32Kernel"
        ''' <summary>分段基数排序（双精度）</summary>
        Public Const SortSegmentsF64 As String = "ilcudaSortSegmentsF64Kernel"
        ' 批量二分查找：对每个查询值给出其在排序段内的 lower / upper bound。
        ''' <summary>批量 lower/upper bound（单精度排序段）</summary>
        Public Const SegBoundsF32 As String = "ilcudaSegBoundsF32Kernel"
        ''' <summary>批量 lower/upper bound（双精度排序段）</summary>
        Public Const SegBoundsF64 As String = "ilcudaSegBoundsF64Kernel"
        ' 单段第 k 名：先选阈值（基数选择），再按下标稳定收集。
        ''' <summary>单段第 k 名阈值（单精度，k 为降序名次）</summary>
        Public Const SegSelectF32 As String = "ilcudaSegSelectF32Kernel"
        ''' <summary>按阈值收集下标（单精度，按下标升序）</summary>
        Public Const SegCollectF32 As String = "ilcudaSegCollectF32Kernel"
        ' 并列和：排序后单趟游程扫描，产出 Σ(t³−t)。
        ''' <summary>逐段并列和 Σ(t³−t)（单精度排序段）</summary>
        Public Const SegTieSumF32 As String = "ilcudaSegTieSumF32Kernel"
        ''' <summary>逐段并列和 Σ(t³−t)（双精度排序段）</summary>
        Public Const SegTieSumF64 As String = "ilcudaSegTieSumF64Kernel"

        ' ---- Kernels\rng.cu ----
        ''' <summary>均匀分布填充（双精度，53 位精度）</summary>
        Public Const RngUniformF64 As String = "ilcudaRngUniformF64Kernel"
        ''' <summary>均匀分布填充（单精度）</summary>
        Public Const RngUniformF32 As String = "ilcudaRngUniformF32Kernel"
        ''' <summary>标准正态填充（双精度，Box-Muller）</summary>
        Public Const RngNormalF64 As String = "ilcudaRngNormalF64Kernel"
        ''' <summary>标准正态填充（单精度，Box-Muller）</summary>
        Public Const RngNormalF32 As String = "ilcudaRngNormalF32Kernel"
        ''' <summary>置换（Fisher-Yates，单线程顺序执行）</summary>
        Public Const RngPermutation As String = "ilcudaRngPermutationKernel"
        ''' <summary>按累积分布加权抽样（双精度）</summary>
        Public Const RngChoiceF64 As String = "ilcudaRngChoiceF64Kernel"

        ' ---- Kernels\linalg.cu ----
        ''' <summary>双精度分块矩阵乘（transA / transB 用 int 标志）</summary>
        Public Const GemmF64 As String = "ilcudaGemmF64Kernel"
        ''' <summary>economy QR：m x n 且 m >= n，输出显式 Q(m x n) 与 R(n x n)</summary>
        Public Const QrHouseholderF64 As String = "ilcudaQrHouseholderF64Kernel"

        ' ---- Kernels\solve.cu ----
        ''' <summary>批量 Cholesky 求解，小档 n &lt;= 32（共享内存）</summary>
        Public Const CholSolveSmallF64 As String = "ilcudaCholSolveSmallF64Kernel"
        ''' <summary>批量 Cholesky 求解，中档 n &lt;= 256（全局暂存）</summary>
        Public Const CholSolveMediumF64 As String = "ilcudaCholSolveMediumF64Kernel"
        ''' <summary>带行掩码的 Gram 矩阵 + 岭项（XᵀX + λI 与 XᵀY）</summary>
        Public Const GramMaskedF64 As String = "ilcudaGramMaskedF64Kernel"
    End Module
End Namespace
