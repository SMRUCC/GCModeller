#Region "Microsoft.VisualBasic::TensorBackend, annotations\WGCNA\WGCNA\Gpu\TensorBackend.vb"

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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Computing.ILCuda
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports tfCompute = Microsoft.VisualBasic.MachineLearning.TensorFlow.Compute

''' <summary>
''' Tensor 计算后端的显式开关（SIMD CPU ↔ CUDA GPU）
''' </summary>
''' <remarks>
''' <see cref="Tensor"/> 采用策略模式：所有算子都派发到
''' <c>Tensor.computeKernel</c>。把后端换成
''' <see cref="GPUTensor.CudaTensor"/> 之后，WGCNA 里的相关矩阵 GEMM 与
''' TOM 的 <c>A*A</c> 会透明地落到 GPU 上执行，业务代码无需任何改动。
''' 
''' <para>
''' <see cref="GPUTensor.CudaTensor"/> 继承自
''' <c>TensorComputeBase</c>，未实现 GPU 内核的算子会自动回退 CPU，
''' 因此注册成功与否都不会影响计算正确性，只影响速度。
''' </para>
''' 
''' <para>
''' 默认保持 SIMD CPU 后端，不改变既有调用方（R# 的 <c>cor_network</c>、CLI 工具）的行为；
''' 需要 GPU 加速时由调用方显式调用 <see cref="EnableGpu"/>。
''' </para>
''' </remarks>
Public Module TensorBackend

    ''' <summary>
    ''' 最近一次 GPU 后端注册的失败原因（成功时为 Nothing）
    ''' </summary>
    ''' <returns>失败原因描述文本；未尝试过或注册成功则为 Nothing</returns>
    Public ReadOnly Property LastGpuError As String
        Get
            Return GPUTensor.CudaTensor.LastError
        End Get
    End Property

    ''' <summary>
    ''' 当前是否已经工作在 CUDA GPU 后端上
    ''' </summary>
    ''' <returns>True 表示 <c>Tensor.computeKernel</c> 当前是 CUDA 后端</returns>
    Public ReadOnly Property IsGpuEnabled As Boolean
        Get
            Return TypeOf Tensor.computeKernel Is GPUTensor.CudaTensor
        End Get
    End Property

    ''' <summary>
    ''' 当前生效的计算后端名称（<c>SIMD</c> 或 <c>CUDA</c>）
    ''' </summary>
    ''' <returns>后端名称文本</returns>
    Public ReadOnly Property BackendName As String
        Get
            Return Tensor.computeKernel.Name
        End Get
    End Property

    ''' <summary>
    ''' 尝试把 <see cref="Tensor"/> 的计算后端切换到 CUDA GPU
    ''' </summary>
    ''' <param name="cacheBytes">
    ''' 显存 LRU 缓存容量（字节）。0 表示按可用显存自适应（取一半、上限 4 GiB）。
    ''' </param>
    ''' <param name="useFp32Gemm">
    ''' 矩阵乘是否走单精度内核。消费级显卡的 FP64 吞吐通常只有 FP32 的 1/32~1/64，
    ''' 这是 TOM 的 GEMM 阶段主要的加速来源。
    ''' </param>
    ''' <returns>注册成功返回 True；设备不可用 / NVRTC 版本不匹配等情况下返回 False 并保持 CPU 后端不变</returns>
    ''' <remarks>
    ''' 注册失败时可以通过 <see cref="LastGpuError"/> 获取原因。
    ''' 由于 <c>CudaTensor.Register()</c> 只在成功时才替换
    ''' <c>Tensor.computeKernel</c>，失败不会破坏已有的 CPU 后端。
    ''' </remarks>
    Public Function EnableGpu(Optional cacheBytes As Long = 0,
                              Optional useFp32Gemm As Boolean = True) As Boolean
        Return GPUTensor.CudaTensor.Register(cacheBytes:=cacheBytes, useFp32Gemm:=useFp32Gemm)
    End Function

    ''' <summary>
    ''' 把 <see cref="Tensor"/> 的计算后端切回默认的 SIMD CPU 实现
    ''' </summary>
    Public Sub DisableGpu()
        Call GPUTensor.CudaTensor.Unregister()
        Call tfCompute.SIMDTensor.Register()
    End Sub

    ''' <summary>
    ''' 矩阵乘走 GPU 的最小规模（m*k*n）。规模更小的 GEMM 会回退 CPU。
    ''' </summary>
    ''' <returns>阈值</returns>
    Public Property MinGemmElements As Integer
        Get
            Return GPUTensor.CudaTensor.MinGemmElements
        End Get
        Set(value As Integer)
            GPUTensor.CudaTensor.MinGemmElements = value
        End Set
    End Property

    ''' <summary>
    ''' 逐元素算子走 GPU 的最小元素数。规模更小时会回退 CPU。
    ''' </summary>
    ''' <returns>阈值</returns>
    Public Property MinGpuElements As Integer
        Get
            Return GPUTensor.CudaTensor.MinGpuElements
        End Get
        Set(value As Integer)
            GPUTensor.CudaTensor.MinGpuElements = value
        End Set
    End Property

    ''' <summary>
    ''' 按配置决定是否启用 GPU，并返回最终生效的后端名称
    ''' </summary>
    ''' <param name="useGpu">是否请求 GPU 加速</param>
    ''' <returns>最终生效的后端名称（<c>SIMD</c> 或 <c>CUDA</c>）</returns>
    ''' <remarks>
    ''' 请求 GPU 但注册失败时，本方法只写一条告警日志并继续使用 CPU，不会抛异常。
    ''' </remarks>
    Public Function ApplyBackend(useGpu As Boolean) As String
        If Not useGpu Then
            Return BackendName
        End If

        If EnableGpu() Then
            Return BackendName
        End If

        Call VBDebugger.EchoLine($"WGCNA: CUDA backend is not available ({LastGpuError}), fallback to SIMD CPU.")

        Return BackendName
    End Function
End Module
