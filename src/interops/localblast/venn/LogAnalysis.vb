#Region "Microsoft.VisualBasic::4ee5d17c1022afb190e03fa71e73bcc6, localblast\venn\LogAnalysis.vb"

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

'     Module LogAnalysis
' 
'         Function: __getBestHitPaired, GetMainidList, Merge, TakeBestHits
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace BlastAPI

    ''' <summary>
    ''' BLAST日志分析模块
    ''' </summary>
    ''' <remarks>This module its code is too old, obsolete!</remarks>
    Public Module LogAnalysis

        ''' <summary>
        ''' 将多个分析出来的最佳匹配的文件合并成一个文件，这个所得到的文件将会用于文氏图的绘制
        ''' </summary>
        ''' <param name="dataset"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Merge(dataset As IEnumerable(Of IO.File)) As IO.File
            Dim result As IO.File = New IO.File
            Dim MainIdList As String() = GetMainidList(dataset)
            Dim LQuery = From Csv As IO.File
                         In dataset
                         Select (From row In Csv.AsParallel
                                 Let Pair = New KeyValuePair(Of String, String)(row(0), row(1))
                                 Select Pair
                                 Distinct
                                 Order By Pair.Key Ascending).ToArray

            For i As Integer = 0 To MainIdList.Count - 1
                Dim Id As String = MainIdList(i)
                Dim row As New IO.RowObject(Id)

                For idx As Integer = 0 To dataset.Count - 1
                    Dim Query = (From k In LQuery(idx) Where String.Equals(k.Key, Id) Select k).ToArray
                    row += If(Query.Length > 0, Query.First.Value, "")
                Next

                result += row
            Next

            Return result
        End Function

        Private Function GetMainidList(datasets As IEnumerable(Of IO.File)) As String()
            Dim List As New List(Of String)(
                datasets _
                    .Select(Function(doc) doc.Column(Scan0)) _
                    .IteratesALL)
            List = (From Id As String
                    In List
                    Select Id
                    Distinct
                    Order By Id Ascending).AsList - "Unknown"
            Return List.ToArray
        End Function

        ''' <summary>
        ''' 从已经分析好的日志文件之中生成最佳匹配表
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TakeBestHits(query As IBlastOutput, subject As IBlastOutput) As IO.File
            Dim tblQuery As IO.File = query.ExportBestHit.ToCsvDoc
            Dim tblHits As IO.File = subject.ExportBestHit.ToCsvDoc

            Call tblQuery.RemoveAt(index:=0)
            Call tblHits.RemoveAt(index:=0)

            Dim LQuery As IEnumerable(Of IO.RowObject) =
                From row As IO.RowObject
                In tblQuery.AsParallel
                Let QueryHitPair As IO.RowObject = __getBestHitPaired(Query:=row, Table:=tblHits)
                Select QueryHitPair
                Order By QueryHitPair.First Ascending '
            Return New IO.File(LQuery)
        End Function

        Private Function __getBestHitPaired(Query As IO.RowObject, Table As IEnumerable(Of IO.RowObject)) As IO.RowObject
            Dim LQuery As IO.RowObject() =
                (From row As IO.RowObject
                 In Table.AsParallel
                 Where String.Equals(row.Column(1), Query.First)
                 Select row).ToArray  '在表二中查找出目标匹配项

            If LQuery.Length > 0 Then  '找到了对应的项
                If String.Equals(Query.Column(1), LQuery(Scan0).First) Then '假若两个ID编号相等，则认为是最佳匹配项
                    Return {Query.First, LQuery(Scan0).First}
                Else
                    Return {Query.First}
                End If
            Else
                Return {Query.First} '不是最佳匹配
            End If
        End Function
    End Module
End Namespace
