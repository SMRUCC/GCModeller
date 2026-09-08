Imports Microsoft.VisualBasic.Data.GraphTheory.Network

Namespace Chem

    ''' <summary>
    ''' 化学键：分子图中的一条边，由两个原子索引与键级构成。
    ''' </summary>
    ''' <remarks>
    ''' 实现 <see cref="IndexEdge"/>，因此分子图可直接接入 sciBASIC 的图论算法（连通分量、
    ''' 最短路等）。本结构不含芳香键标记——芳香性一律以 Kekulé 式的单/双键表达（键级 1/2/3）。
    ''' </remarks>
    Public Class Bond : Implements IndexEdge

        ''' <summary>
        ''' 键一端的原子索引（分子内的 0 基下标）。
        ''' </summary>
        ''' <returns>原子索引，取值区间 [0, <see cref="Molecule.NumAtoms"/>)。</returns>
        Public Property a As Integer Implements IndexEdge.U

        ''' <summary>
        ''' 键另一端的原子索引（分子内的 0 基下标）。
        ''' </summary>
        ''' <returns>原子索引，取值区间 [0, <see cref="Molecule.NumAtoms"/>)。</returns>
        Public Property b As Integer Implements IndexEdge.V

        ''' <summary>
        ''' 键级：1 = 单键，2 = 双键，3 = 三键。
        ''' </summary>
        ''' <returns>键级数值，本库中恒为 1、2 或 3。</returns>
        Public Property order As Integer

        ''' <summary>
        ''' 由三元组 <c>(a, b, order)</c> 隐式转换为 <see cref="Bond"/>，
        ''' 便于以 <c>bonds.Add((a, b, order))</c> 的形式直接添加化学键。
        ''' </summary>
        ''' <param name="tri">三元组：第一项为原子 a、第二项为原子 b、第三项为键级。</param>
        ''' <returns>与三元组等价的 <see cref="Bond"/> 实例。</returns>
        Public Shared Widening Operator CType(tri As (Integer, Integer, Integer)) As Bond
            Return New Bond With {.a = tri.Item1, .b = tri.Item2, .order = tri.Item3}
        End Operator

    End Class
End Namespace
