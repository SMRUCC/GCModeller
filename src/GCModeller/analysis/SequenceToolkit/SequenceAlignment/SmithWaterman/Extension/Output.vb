#Region "Microsoft.VisualBasic::f3be1f091781bb79263bb55eb43c8f36, analysis\SequenceToolkit\SequenceAlignment\SmithWaterman\Extension\Output.vb"

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

    '   Total Lines: 116
    '    Code Lines: 75 (64.66%)
    ' Comment Lines: 26 (22.41%)
    '    - Xml Docs: 96.15%
    ' 
    '   Blank Lines: 15 (12.93%)
    '     File Size: 4.81 KB


    '     Class Output
    ' 
    '         Properties: Best, Directions, DP, HSP, Query
    '                     Subject, Traceback
    ' 
    '         Function: ContainsPoint, CreateObject, GenericEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace BestLocalAlignment

    ''' <summary>
    ''' Smith-Waterman 局部比对的完整输出结果。
    ''' </summary>
    ''' <remarks>
    ''' 该对象是一个**重量级**结果容器:<see cref="DP"/> 与 <see cref="Directions"/> 
    ''' 均为按行包装的动态规划矩阵视图,其内存占用与 query*subject 成正比
    ''' (千级长度的蛋白序列比对即可达到数十 MB),并且这些行数组会进入大对象堆(LOH)。
    ''' 
    ''' 特别注意 <see cref="Directions"/> 的构建代价:它由方向矩阵(Integer)逐行
    ''' 转换成 Double 数组,元素被放大到 8 字节;而 <see cref="DP"/> 又是得分矩阵的
    ''' 一份完整副本。也就是说单次比对会额外产生**两份**与 query*subject 等大的
    ''' Double 矩阵,与算法自身持有的矩阵叠加后内存放大到约三倍。
    ''' 
    ''' 因此本类实现了 <see cref="IDisposable"/>:在批量/循环比对场景中,请使用
    ''' <c>Using</c> 语句或显式调用 <see cref="Dispose"/> 及时切断对矩阵的引用。
    ''' 若只需要得分最高的一条比对结果,应改用轻量级接口
    ''' <see cref="GSW(Of T).GetBestHSP"/>,它完全不会构建本对象。
    ''' </remarks>
    <XmlType("GSW", [Namespace]:=SMRUCC.genomics.LICENSE.GCModeller)>
    Public Class Output : Implements Enumeration(Of HSP), IDisposable

        Private disposedValue As Boolean

        ''' <summary>
        ''' best chain, 但是不明白这个有什么用途
        ''' </summary>
        ''' <returns></returns>
        Public Property Best As HSP

        ''' <summary>
        ''' 最佳的比对结果
        ''' </summary>
        ''' <returns></returns>
        Public Property HSP As HSP()
        ''' <summary>
        ''' Dynmaic programming matrix.(也可以看作为得分矩阵)
        ''' </summary>
        ''' <returns></returns>
        Public Property DP As ArrayRow()
        ''' <summary>
        ''' The directions pointing to the cells that
        ''' give the maximum score at the current cell.
        ''' The first index is the column index.
        ''' The second index is the row index.
        ''' </summary>
        ''' <returns></returns>
        Public Property Directions As ArrayRow()

        Public Property Query As String
        Public Property Subject As String
        Public Property Traceback As Coordinate()

        Public Function ContainsPoint(i As Integer, j As Integer) As Boolean
            Dim LQuery = (From x In Traceback Where x.X = i AndAlso x.Y = j Select 100).FirstOrDefault
            Return LQuery > 50
        End Function

        Public Overrides Function ToString() As String
            Dim edits As String = ""
            Dim pre = Traceback.First

            For Each cd As Coordinate In Traceback.Skip(1)
                If cd.X - pre.X = -1 AndAlso cd.Y - pre.Y = -1 Then
                    edits &= "m" '  match 和 substitute应该如何进行判断？？？
                End If
                If cd.X - pre.X = 0 AndAlso cd.Y - pre.Y = -1 Then
                    edits &= "i"
                End If
                If cd.X - pre.X = -1 AndAlso cd.Y - pre.Y = 0 Then
                    edits &= "d"
                End If

                pre = cd
            Next

            Return edits
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="sw"></param>
        ''' <param name="threshold">0% - 100%</param>
        ''' <returns></returns>
        Public Shared Function CreateObject(Of T)(sw As GSW(Of T), threshold As Double, minW As Integer) As Output
            Dim hspList = sw.CreateHSP(cutoff:=threshold * sw.AlignmentScore).ToArray
            Dim direction = sw.prevCells.Select(Function(x) New ArrayRow(x)).ToArray
            Dim toChar As Func(Of T, Char) = AddressOf sw.symbol.ToChar
            Dim dp = sw.GetDPMAT.Select(Function(x) New ArrayRow(x)).ToArray
            Dim query = sw.query.Select(Function(x) toChar(x)).CharString
            Dim subject = sw.subject.Select(Function(x) toChar(x)).CharString
            Dim m2Len As Integer = Math.Min(query.Length, subject.Length)
            Dim best As HSP = If(hspList Is Nothing OrElse hspList.Length = 0,
                                  Nothing,
                                  BestLocalAlignment.HSP.CreateFrom(hspList.GetBestAlignment, toChar))

            If m2Len < minW Then
                Call $"Min width {minW} is too large than query/subject, using min(query,subject):={m2Len} instead....".debug
                minW = m2Len
            End If

            hspList = (From x In hspList Where x.LengthHit >= minW AndAlso x.LengthQuery >= minW Select x).ToArray

            Return New Output With {
                .Traceback = sw.GetTraceback,
                .Directions = direction,
                .DP = dp,
                .HSP = hspList _
                    .Select(Function(h)
                                Dim hsp As HSP = BestLocalAlignment.HSP.CreateFrom(h, AddressOf sw.symbol.ToChar)
                                hsp.QueryLength = query.Length
                                hsp.SubjectLength = subject.Length
                                Return hsp
                            End Function) _
                    .ToArray,
                .Query = query,
                .Subject = subject,
                .Best = best
            }
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of HSP) Implements Enumeration(Of HSP).GenericEnumerator
            For Each region As HSP In HSP.SafeQuery
                Yield region
            Next
        End Function

        ''' <summary>
        ''' 释放本对象所持有的动态规划矩阵引用。
        ''' </summary>
        ''' <remarks>
        ''' <see cref="DP"/> / <see cref="Directions"/> 的每一个元素都是一行矩阵数据的包装,
        ''' 每行都是一个独立的 Double 数组,长度与序列长度同阶,足以进入大对象堆(LOH)。
        ''' 这里先逐行置空,再释放外层数组,确保这些大数组在本对象被丢弃后可以立即被 GC 回收,
        ''' 而不是滞留在大对象堆中造成常驻内存持续增长。
        ''' 
        ''' <see cref="Best"/> / <see cref="Query"/> / <see cref="Subject"/> 属于小对象,
        ''' 通常需要在 Dispose 之后继续使用(例如返回给调用方),因此不在此处清理。
        ''' </remarks>
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    Call eraseMatrix(_DP)
                    Call eraseMatrix(_Directions)

                    Erase _DP
                    Erase _Directions
                    Erase _Traceback
                    Erase _HSP
                End If

                disposedValue = True
            End If
        End Sub

        ''' <summary>
        ''' 逐行切断矩阵视图对底层行数组的引用。
        ''' </summary>
        Private Shared Sub eraseMatrix(matrix As ArrayRow())
            If matrix Is Nothing Then
                Return
            End If

            For i As Integer = 0 To matrix.Length - 1
                matrix(i) = Nothing
            Next
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
