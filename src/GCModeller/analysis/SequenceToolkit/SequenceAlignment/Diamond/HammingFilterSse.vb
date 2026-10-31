' Hamming 距离过滤 —— 向量化实现入口
'
' 历史实现用 SSE2 (pcmpeqb / pmovmskb / popcnt) 做逐 16 字节比较,
' 但受限于 VB.NET 无法直接取数组内部引用做向量加载,每次比较都要先把
' 窗口内 16 个字节逐字节拷贝到临时缓冲,再用 16 个标量参数构造
' Vector128 —— 拷贝开销反而超过标量循环本身,且每个窗口还会分配两个临时数组。
'
' 现在统一为"预编码缓冲区 + 同类判定查表 + 早退"的标量实现:
'   - 每个残基只有 2 次数组读取 + 1 次查表,无分配、无拷贝、无字符串索引;
'   - Pass 路径一旦超过阈值立即退出,假阳性命中几乎不消耗时间。
' 该类保留原有类型名与接口签名,以便外部按 SSE 版本选择时保持兼容;
' 同时继承标量实现,行为与 <see cref="HammingFilter"/> 完全一致。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 向量化 Hamming 距离过滤器入口(内部为高度优化的标量表实现)。
    ''' </summary>
    Public Class HammingFilterSse : Inherits HammingFilter

        Sub New(Optional maxDistance As Integer = -1, Optional scorer As AminoAcidScorer = Nothing)
            MyBase.New(maxDistance, scorer)
        End Sub
    End Class
End Namespace
