#Region "Microsoft.VisualBasic::9be9d5da9a1a03bcc34b57a74c212ecb, analysis\RNA-Seq\Toolkits.RNA-Seq\Matrix\Matrix.vb"

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

    '     Class MatrixFrame
    ' 
    '         Properties: Average, LstExperiments, LstLocusId, Name
    ' 
    '         Function: [Get], __caculation, __createRow, CalculatePccMatrix, GetCurrentRPKMsVector
    '                   GetOriginalMatrix, GetValue, Load, Log2, SetColumnAuto
    '                   ToDictionary, ToString
    ' 
    '         Sub: (+2 Overloads) SetColumn
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Namespace dataExprMAT

    ''' <summary>
    ''' 用来表述一个RNA-Seq结果数据的集合
    ''' </summary>
    Public Class MatrixFrame

        ''' <summary>
        ''' 原始数据
        ''' </summary>
        Dim __reader As IO.DataFrame
        Protected Friend __pCol As Integer = -1

        Public Function CalculatePccMatrix() As PccMatrix
            Return CreatePccMAT(__reader, False)
        End Function

        Public ReadOnly Property LstExperiments As String()
            Get
                Return __reader.HeadTitles.Skip(1).ToArray
            End Get
        End Property

        Public ReadOnly Property LstLocusId As String()
        Public ReadOnly Property Average As Double

        Private Shared Function __caculation(source As MatrixFrame, sample As Experiment) As Double()
            Dim Numerator As Double(), Denominator As Double()

            Call source.SetColumnAuto(sample.Experiment) : Numerator = source.GetCurrentRPKMsVector()
            Call source.SetColumnAuto(sample.Reference) : Denominator = source.GetCurrentRPKMsVector()

            Return (From ni As SeqValue(Of Double)
                    In Numerator.SeqIterator
                    Select stdNum.Log(ni.value / Denominator(ni.i), 2)).ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="samples">{分子, 分母}()</param>
        ''' <returns></returns>
        ''' <remarks>请勿再随意修改本函数之中的并行定义，以免照成混乱</remarks>
        ''' 
        Public Function Log2(samples As IEnumerable(Of Experiment)) As IO.File
            Dim LQuery = (From sample As Experiment
                          In samples
                          Let vector As Double() = __caculation(Me, sample)
                          Select New ExprSamples(sample.Sample, vector)).ToArray
            Dim RowsQuery = (From item In (From i As SeqValue(Of String)
                                           In LstLocusId.SeqIterator
                                           Select i).AsParallel
                             Let GeneId As String = item.value
                             Let i = item.i
                             Let createdRow As IO.RowObject = __createRow(LQuery, GeneId, i)
                             Select createdRow
                             Order By createdRow.First Ascending).AsList
            Dim Head As New IO.RowObject("LocusId" + (From item In LQuery Select item.locusId).AsList)
            Dim data As IO.File = New IO.File(Head + RowsQuery)

            Return data
        End Function

        Private Shared Function __createRow(LQuery As ExprSamples(), GeneId As String, i As Integer) As IO.RowObject
            Dim row As IO.RowObject =
                New IO.RowObject(GeneId + LQuery.ToList(Function(x) x(i).ToString))
            Return row
        End Function

        Public Function GetOriginalMatrix() As IO.File
            Return __reader
        End Function

        ''' <summary>
        ''' 获取当前实验标号设置之下的所有的实验数据
        ''' 在使用本函数获取返回值之前，请先试用<see cref="SetColumn"></see>或者<see cref="SetColumnAuto"></see>设置实验编号
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCurrentRPKMsVector() As Double()
            Dim LQuery = (From item In __reader.ToArray Select Val(item.Column(__pCol))).ToArray
            Return LQuery
        End Function

        Const ExperimentNotExists As String =
            "[WARNING] Target experiment ""{0}"" is not exists in the expression data collection, an empty value will be return!"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ExperimentId"></param>
        ''' <param name="calAvg">
        ''' 在切换列的时候是否进行该实验条件之下的表达的平均值的计算，假若需要频繁的进行列切换但是并不涉及到平均值的计算话，可以将这个值设置为False
        ''' </param>
        Public Sub SetColumn(ExperimentId As String, Optional calAvg As Boolean = True)
            __pCol = __reader.GetOrdinal(ExperimentId)
            If __pCol = -1 Then _
                Call String.Format(ExperimentNotExists, ExperimentId).__DEBUG_ECHO
            If calAvg Then
                Me._Average = (From row In __reader.ToArray Select Val(row(__pCol))).ToArray.Average
            End If
        End Sub

        Public Function SetColumnAuto(ExperimentIdOrColIndex As String) As Boolean
            Dim col_p As Integer = Val(ExperimentIdOrColIndex)

            If col_p = 0 AndAlso String.Equals(ExperimentIdOrColIndex, "0") Then
                Call SetColumn(0)
            ElseIf col_p > 0 AndAlso String.Equals(ExperimentIdOrColIndex, col_p.ToString) Then
                Call SetColumn(CInt(col_p))
            Else
                Call SetColumn(ExperimentIdOrColIndex)
            End If

            Return col_p = -1
        End Function

        Const ExperimentIndexNotExists As String =
            "[WARNING] Target experiment of Id index ""{0}"" is not exists in the chipdata collection, an empty value will be return!"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ColIndex">大于零的编号</param>
        ''' <remarks></remarks>
        Public Sub SetColumn(ColIndex As Integer)
            __pCol = ColIndex
            If __pCol - 1 >= __reader.Width Then Call String.Format(ExperimentIndexNotExists, ColIndex).__DEBUG_ECHO

            Me._Average = (From row In __reader.ToArray Select Val(row(__pCol))).ToArray.Average
        End Sub

        ''' <summary>
        ''' 当目标基因编号不存在的时候返回0
        ''' </summary>
        ''' <param name="locusTag"></param>
        ''' <param name="DEBUGInfo">当发生错误的时候是否显示出调试信息</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValue(locusTag As String, Optional DEBUGInfo As Boolean = False) As Double
            Dim rowIdx As Integer = Array.IndexOf(Me.LstLocusId, locusTag.ToUpper)

            If rowIdx = -1 Then
                If DEBUGInfo Then _
                Call $"[ERROR] locus_tag=""{locusTag}"" is not found in the dataset!".__DEBUG_ECHO
                Return 0
            Else
                If __pCol = -1 Then
                    Return 0
                End If

                Dim row As RowObject = __reader.Item(rowIdx)
                Return Val(row.Column(__pCol))
            End If
        End Function

        Public Function [Get](index As Integer) As Double
            If __pCol = -1 Then
                Return 0
            End If

            Dim row = __reader.Item(index)
            Return Val(row.Column(__pCol))
        End Function

        Public Overrides Function ToString() As String
            Return Me.Name
        End Function

        Public ReadOnly Property Name As String

        ''' <summary>
        ''' Load expression data from a csv docuemnt stream.
        ''' </summary>
        ''' <param name="chipDataCsv">首行是标题行，第一列是基因的编号</param>
        ''' <returns></returns>
        Public Shared Function Load(chipDataCsv As IO.File) As MatrixFrame
            Dim locusId As String() =
                LinqAPI.Exec(Of String) <= From row As IO.RowObject
                                           In chipDataCsv.Skip(1)  ' 首行是标题
                                           Select row.First
            Dim ChipData As MatrixFrame = New MatrixFrame With {
                .__reader = IO.DataFrame.CreateObject(chipDataCsv),
                ._LstLocusId = locusId
            }

            Call $"Chipdata set load job done! {locusId.Length} objects".__DEBUG_ECHO

            Return ChipData
        End Function

        ''' <summary>
        ''' {GeneID, Expressions}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToDictionary() As Dictionary(Of String, Double())
            Dim dat As ExprSamples() = MatrixAPI.ToSamples(Me.GetOriginalMatrix, True)
            Return dat.ToDictionary(Function(x) x.locusId, Function(x) x.data)
        End Function
    End Class
End Namespace
