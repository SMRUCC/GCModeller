#Region "Microsoft.VisualBasic::fcb69d72cdcaca1a88b9ecebedbf4744, ..\GCModeller\core\Bio.InteractionModel\DataServicesExtension.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv

<[Namespace]("Plot.Data.Services")> Public Module DataServicesExtension

    ''' <summary>
    ''' 带标签的实验样品数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure SerialsData : Implements sIdEnumerable
        Implements IEnumerable(Of Double)
        Implements IKeyValuePairObject(Of String, Double())

        ''' <summary>
        ''' 样品标签
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Tag As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, Double()).Identifier
        ''' <summary>
        ''' 该样品的实验数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ChunkBuffer As Double() Implements IKeyValuePairObject(Of String, Double()).Value

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1} sample datas", Tag, ChunkBuffer.Length)
        End Function

        Default Public ReadOnly Property DataValue(Index As Integer) As Double
            Get
                Return ChunkBuffer(Index)
            End Get
        End Property

        Public ReadOnly Property Length As Integer
            Get
                If ChunkBuffer.IsNullOrEmpty Then
                    Return 0
                Else
                    Return ChunkBuffer.Length
                End If
            End Get
        End Property

        Public Iterator Function GetEnumerator() As IEnumerator(Of Double) Implements IEnumerable(Of Double).GetEnumerator
            For Each n As Double In ChunkBuffer
                Yield n
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Structure

    <ExportAPI("Read.Csv.Serials")>
    Public Function LoadData(path As String) As SerialsData()
        Dim Csv As DocumentStream.File = DocumentStream.File.FastLoad(path)
        Return LoadCsv(Csv)
    End Function

    <ExportAPI("Load.Csv.Serials")>
    Public Function LoadCsv(Csv As DocumentStream.File) As SerialsData()
        Return LoadCsv(CsvDatas:=Csv.ToArray)
    End Function

    Public Function LoadCsv(CsvDatas As IEnumerable(Of DocumentStream.RowObject)) As SerialsData()
        Dim LQuery = (From Line As DocumentStream.RowObject
                      In CsvDatas.AsParallel
                      Let TagValue As String = Line.First
                      Let Value As Double() = (From col As String In Line.Skip(1) Select Val(col)).ToArray
                      Select New SerialsData With {
                          .Tag = TagValue,
                          .ChunkBuffer = Value}).ToArray
        Return LQuery
    End Function

    <ExportAPI("Export.Csv")>
    Public Function SaveCsv(data As Generic.IEnumerable(Of SerialsData)) As DocumentStream.File
        Dim RowGenerateLQuery = (From row As SerialsData In data.AsParallel
                                 Let IDCol As String() = New String() {row.Tag}
                                 Let DataCol As String() = (From n In row Select s = n.ToString).ToArray
                                 Let RowData As String()() = New String()() {IDCol, DataCol}
                                 Select CType(RowData.MatrixToList, DocumentStream.RowObject)).ToArray
        Dim Csv = CType(RowGenerateLQuery, DocumentStream.File)
        Return Csv
    End Function

    <ExportAPI("Write.Csv")>
    Public Function SaveCsv(data As IEnumerable(Of SerialsData), saveToCsv As String) As Boolean
        Return SaveCsv(data).Save(saveToCsv, False)
    End Function
End Module

