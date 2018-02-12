#Region "Microsoft.VisualBasic::9903a051842a442b5037358859b4ab9d, CLI_tools\c2\Reconstruction\Genome\ExpressionProfile.vb"

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

    '     Class ExpressionProfile
    ' 
    '         Function: Extract, Proceeding, Report, Sigma
    ' 
    '         Sub: Export, PrepareData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace Reconstruction

    Public Class ExpressionProfile

        Public Shared Function Extract(
                                      Data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                                      IdCol As Integer,
                                      ExprValueCol As Integer,
                                      Optional FirstLineTitle As Boolean = True) As KeyValuePair(Of String, Integer)()
            Dim rows As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject() =
                If(FirstLineTitle, Data.Skip(1).ToArray, Data.ToArray)
            Dim pairItems As KeyValuePair(Of String, Integer)() = New KeyValuePair(Of String, Integer)(rows.Count - 1) {}
            For i As Integer = 0 To rows.Count - 1
                Dim row = rows(i)
                Dim ExprValue As String = row(ExprValueCol)
                Dim Id As String = row(IdCol)

                If String.IsNullOrEmpty(ExprValue) Then
                    ExprValue = 0
                End If

                pairItems(i) = New KeyValuePair(Of String, Integer)(Id, Convert.ToInt32(ExprValue))
            Next

            Return pairItems
        End Function

        Dim _Max, _Min As Integer
        Dim _Data As KeyValuePair(Of String, Integer)()

        Public Sub PrepareData(Data As KeyValuePair(Of String, Integer)())
            Dim Collection = (From item In Data Select item.Value).ToArray
            Me._Max = Collection.Max
            Me._Min = Collection.Min
            Me._Data = (From item In Data Select item Order By item.Value Ascending).ToArray
        End Sub

        Public Function Proceeding(InitSectorCounts As Integer, SigmaValue As Double) As KeyValuePair(Of String, Integer)()()
            Dim SectionList As List(Of KeyValuePair(Of String, Integer)()) = New List(Of KeyValuePair(Of String, Integer)())
            Dim Sector As List(Of KeyValuePair(Of String, Integer)) = New List(Of KeyValuePair(Of String, Integer)) From {_Data.First}
            Dim d As Double = (_Max - _Min) / (InitSectorCounts * 1024)

            For i As Integer = 1 To _Data.Count - 1
                If _Data(i).Value - _Data(i - 1).Value < d Then
                    Call Sector.Add(_Data(i))
                Else
                    Call SectionList.Add(Sector.ToArray)
                    Sector = New List(Of KeyValuePair(Of String, Integer)) From {_Data(i)}
                End If
            Next

            Call SectionList.Add(Sector.ToArray)

            If InitSectorCounts = _Data.Count - 1 Then
                Return SectionList.ToArray
            Else
                For Each Section In SectionList
                    Dim _sigma As Double = Sigma(Section)
                    If Not _sigma < SigmaValue Then
                        Return Proceeding(InitSectorCounts + 1, SigmaValue)
                    End If
                Next

                Return SectionList.ToArray
            End If
        End Function

        Public Shared Sub Export(Data As KeyValuePair(Of String, Integer)()(), ExportedDir As String, SeqHandler As System.Func(Of String(), LANS.SystemsBiology.SequenceModel.FASTA.FastaFile))
            Call FileIO.FileSystem.CreateDirectory(ExportedDir)
            For Each item In Data
                Dim idList As String() = (From node In item Select node.Key).ToArray
                Dim SavePath As String = String.Format("{0}/{1}..fsa", ExportedDir, idList.First)
                Call SeqHandler(idList).Save(SavePath)
            Next
        End Sub

        Public Shared Function Report(Data As KeyValuePair(Of String, Integer)()()) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim rows As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject() =
                New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject(Data.Count - 1) {}
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            For i As Integer = 0 To rows.Count - 1
                Dim row = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
                Dim Avg As Double = Data(i).Average(Function(item As KeyValuePair(Of String, Integer)) item.Value)
                For Each item In Data(i)
                    Call sBuilder.Append(item.Key & ",")
                Next
                Call sBuilder.Remove(sBuilder.Length - 1, 1)

                Call row.Add(i)
                Call row.Add(Avg)
                Call row.Add(Data(i).Count)
                Call row.Add(sBuilder.ToString)
                Call sBuilder.Clear()

                rows(i) = row
            Next

            Dim File As New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Call File.AppendLine(New String() {"Id", "avgExprValue", "listCounts", "Idlist"})
            Call File.AppendRange(rows)
            Return File
        End Function

        ''' <summary>
        ''' 计算数组的标准差
        ''' </summary>
        ''' <param name="Data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function Sigma(Data As KeyValuePair(Of String, Integer)()) As Double
            Dim Avg As Double = Data.Average(Function(item As KeyValuePair(Of String, Integer)) item.Value)
            Dim Sum = (From item In Data Select (item.Value - Avg) ^ 2).Sum
            Return System.Math.Sqrt(Sum / Data.Count)
        End Function
    End Class
End Namespace
