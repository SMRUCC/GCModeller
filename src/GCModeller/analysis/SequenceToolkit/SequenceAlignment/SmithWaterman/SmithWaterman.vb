#Region "Microsoft.VisualBasic::6c96a78edb01b6f7c59bcee52015835c, analysis\SequenceToolkit\SequenceAlignment\SmithWaterman\SmithWaterman.vb"

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

    '   Total Lines: 94
    '    Code Lines: 41 (43.62%)
    ' Comment Lines: 43 (45.74%)
    '    - Xml Docs: 90.70%
    ' 
    '   Blank Lines: 10 (10.64%)
    '     File Size: 4.36 KB


    '     Class SmithWaterman
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Align, GetBestHSP, GetOutput, SymbolProvider
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Language.Default
Imports SMRUCC.genomics.SequenceModel

Namespace BestLocalAlignment

    ''' <summary>
    ''' Smith-Waterman local alignment algorithm.
    '''
    ''' Design Note: this class implements AminoAcids interface: a simple fix customized to amino acids, since that is all we deal with in this class
    ''' Supporting both DNA and Aminoacids, will require a more general design.
    ''' </summary>
    Public Class SmithWaterman : Inherits GSW(Of Char)

        ''' <summary>
        ''' 蛋白比对的矩阵
        ''' </summary>
        Shared ReadOnly blosum62 As [Default](Of Blosum) = Blosum.FromInnerBlosum62

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="blosum">
        ''' If the matrix parameter is null, then the default build in blosum62 matrix will be used.
        ''' </param>
        Sub New(query$, subject$, Optional blosum As Blosum = Nothing)
            Call MyBase.New(query.ToArray, subject.ToArray, SymbolProvider(blosum))
        End Sub

        Sub New(query As ISequenceModel, subject As ISequenceModel, Optional blosum As Blosum = Nothing)
            Call MyBase.New(query.SequenceData.ToArray, subject.SequenceData.ToArray, SymbolProvider(blosum))
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function SymbolProvider(blosum As Blosum) As GenericSymbol(Of Char)
            Return New GenericSymbol(Of Char)(
                equals:=Function(x, y) x = y,
                similarity:=AddressOf (blosum Or blosum62).GetDistance,
                toChar:=Function(x) x,
                empty:=Function() "-"c
            )
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="cutoff">0%-100%</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetOutput(cutoff As Double, minW As Integer) As Output
            Return Output.CreateObject(Me, cutoff, minW)
        End Function

        ''' <summary>
        ''' 轻量级路径:只取得分最高的一条 <see cref="HSP"/>。
        ''' </summary>
        ''' <param name="cutoff">0%-100%,收集 HSP 的得分阈值(占最高分的比例)。</param>
        ''' <param name="minW">最短片段长度过滤;0 表示不限制。</param>
        ''' <returns>得分最高的 HSP;若无满足条件的比对则返回 Nothing。</returns>
        ''' <remarks>
        ''' 相比 <see cref="GetOutput"/>,本方法不会构建 <see cref="Output"/> 对象,
        ''' 从而避免复制整个动态规划得分矩阵、构建方向矩阵视图与回溯路径。
        ''' 在需要对大量序列做两两比对的聚类流程中,应优先使用本方法:
        ''' 完整的 <see cref="Output"/> 会产生与序列长度乘积成正比的大对象(进入 LOH),
        ''' 在循环中反复分配会导致进程常驻内存不断攀升。
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function GetBestHSP(cutoff As Double, minW As Integer) As HSP
            Dim best As LocalHSPMatch(Of Char) = MyBase.GetBestHSP(cutoff * AlignmentScore, minW)

            If best Is Nothing Then
                Return Nothing
            Else
                Return HSP.CreateFrom(best, AddressOf symbol.ToChar)
            End If
        End Function

        ''' <summary>
        ''' Default using ``Blosum62`` matrix.
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="blosum"></param>
        ''' <returns></returns>
        Public Shared Function Align(query As IPolymerSequenceModel, subject As IPolymerSequenceModel, Optional blosum As Blosum = Nothing) As SmithWaterman
            Return New SmithWaterman(query.SequenceData, subject.SequenceData, blosum).BuildMatrix
        End Function
    End Class
End Namespace
