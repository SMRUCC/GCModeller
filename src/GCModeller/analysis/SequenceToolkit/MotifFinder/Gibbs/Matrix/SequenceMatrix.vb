#Region "Microsoft.VisualBasic::8f3ad52b65ec6b4296873518dca63597, analysis\SequenceToolkit\MotifFinder\Gibbs\Matrix\SequenceMatrix.vb"

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

    '   Total Lines: 51
    '    Code Lines: 32 (62.75%)
    ' Comment Lines: 9 (17.65%)
    '    - Xml Docs: 77.78%
    ' 
    '   Blank Lines: 10 (19.61%)
    '     File Size: 1.83 KB


    '     Class SequenceMatrix
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: probability
    ' 
    '         Sub: initSequenceMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Matrix

    Public Class SequenceMatrix : Inherits WeightMatrix

        ''' <summary>
        ''' 加一平滑所引入的伪计数
        ''' </summary>
        Const PSEUDOCOUNT As Double = 1.0R

        Private ReadOnly sequences As IList(Of String)
        Private ReadOnly sequenceCount As Integer
        Private ReadOnly sequenceLength As Integer
        ''' <summary>
        ''' 每一列之上实际观测到的碱基数量，N 等非标准字符不计入
        ''' </summary>
        Private ReadOnly columnObserved As Integer()

        Public Sub New(sequences As IList(Of String))
            If sequences Is Nothing OrElse sequences.Count = 0 Then
                Throw New ArgumentException("the motif sequence collection should not be empty!", NameOf(sequences))
            End If

            Me.sequences = sequences
            Me.sequenceCount = sequences.Count
            Me.rowSum = sequenceCount
            Me.sequenceLength = sequences(0).Length

            Call initMatrix(sequenceLength)

            ReDim Me.columnObserved(sequenceLength - 1)

            Call initSequenceMatrix()
        End Sub

        ''' <summary>
        ''' Counts the occurrences of each base along each position of each sequence
        ''' </summary>
        Private Sub initSequenceMatrix()
            Dim b As Integer
            Dim sequence As String

            For i As Integer = 0 To sequenceCount - 1
                sequence = sequences(i)

                For j As Integer = 0 To sequenceLength - 1
                    b = Utils.indexOfBase(sequence(j))

                    ' b = -1 means N or - these non-standard sequence chars
                    If b > -1 Then
                        countsMatrix(j)(b) += 1
                        ' 记录该列上实际观测到的碱基数，供概率归一化使用
                        columnObserved(j) += 1
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 第 index 列之上实际观测到的碱基数量（被屏蔽为 N 的位置不计入）
        ''' </summary>
        ''' <param name="index">, index of base </param>
        ''' <returns></returns>
        Public Function observedCount(index As Integer) As Integer
            Return columnObserved(index)
        End Function

        ''' <summary>
        ''' Returns the probability of seeing the base in the index </summary>
        ''' <param name="index">, index of base </param>
        ''' <param name="base">, base in the index </param>
        Public Overridable Function probability(index As Integer, base As Integer) As Double
            Dim observed As Integer = columnObserved(index)

            ' 整列都被屏蔽（全部为 N）时没有任何观测数据，退化为均匀分布，
            ' 此时该列对信息含量的贡献恰好为 0
            If observed = 0 Then
                Return 0.25
            End If

            ' 必须按照「该列的实际观测数」归一化，而不是按照序列条数 rowSum：
            ' 位点被屏蔽之后列内会出现 N，若继续沿用固定的 rowSum 分母，
            ' 概率会被稀释、信息含量被压低甚至变为负数，并且跨屏蔽轮次之间不可比。
            Return (countsMatrix(index)(base) + PSEUDOCOUNT) / (observed + PSEUDOCOUNT * 4)
        End Function
    End Class

End Namespace
