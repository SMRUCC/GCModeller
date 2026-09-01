' ============================================================
' PropagationMethod.vb - 全局虚拟扰动的传播方法枚举
' ============================================================
' 依据 DBNBlocks.md 文档，全局扰动必须在"整合后的全局网络"上
' 模拟传播，而非把子模块结果简单堆叠。提供两种传播方法，
' 由 WGCNASubnetworkPipeline.Propagation 参数切换。
' ============================================================

Namespace ModularNetwork

    ''' <summary>
    ''' 全局虚拟扰动的传播方法
    ''' </summary>
    Public Enum PropagationMethod

        ''' <summary>
        ''' 雅可比矩阵传播（默认）：把全局系数矩阵 A 视作线性化雅可比，
        ''' 扰动向量沿 A^k 多步线性传播至收敛，得到稳态全局响应。
        ''' 适用于小幅扰动的线性近似。
        ''' </summary>
        Jacobian

        ''' <summary>
        ''' 级联采样传播：在整合后的全局网络上做 do-演算，并把上一步
        ''' 各基因均值作为相邻模块证据迭代采样多步，跨模块传播不确定性。
        ''' 更忠实于概率模型，计算更重。
        ''' </summary>
        CascadeSampling

    End Enum

End Namespace
