#Region "Microsoft.VisualBasic::51bf2aa7ed4c6ca8648e7020b140271d, RDotNet.Extensions.Bioinformatics\Declares\CRAN\VennDiagram\VennDiagram\RModelAPI.vb"

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

    '     Module RModelAPI
    ' 
    '         Function: __vector, Generate, (+3 Overloads) VectorMapper
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace VennDiagram.ModelAPI

    Public Module RModelAPI

        ''' <summary>
        ''' 从一个Excel逗号分割符文件之中生成一个文氏图的数据模型
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Generate(source As IO.File) As VennDiagram
            Dim LQuery = From vec
                         In __vector(source:=source)
                         Select New Partition With {
                             .Vector = String.Join(", ", vec.Value),
                             .Name = vec.Key
                         } '
            Return New VennDiagram With {
                .partitions = LQuery.ToArray
            }
        End Function

        Private Function __vector(source As File) As Dictionary(Of String, String())
            Dim Width As Integer = source.First.Count
            Dim Vector = (From name As String
                          In source.First
                          Select k = name,
                              list = New List(Of String)).ToArray

            For row As Integer = 1 To source.RowNumbers - 1
                Dim Line As RowObject = source(row)
                For colums As Integer = 0 To Width - 1
                    If Not String.IsNullOrEmpty(Line.Column(colums).Trim) Then
                        Call Vector(colums).list.Add(CStr(row))
                    End If
                Next
            Next

            Return Vector.ToDictionary(Function(x) x.k, 
                                       Function(x) x.list.ToArray)
        End Function

        ''' <summary>
        ''' 从实际的对象映射到venn图里面的实体标记
        ''' </summary>
        ''' <returns>为了保证一一对应的映射关系，这个函数里面不再使用并行化拓展</returns>
        <Extension>
        Public Function VectorMapper(entities As IEnumerable(Of String), lTokens As Func(Of String, String())) As String()
            Dim Tokens As String()() = entities.Select(Function(entity) lTokens(entity)).ToArray
            Return VectorMapper(entities:=Tokens)
        End Function

        ''' <summary>
        ''' 从实际的对象映射到venn图里面的实体标记
        ''' </summary>
        ''' <param name="entities">字符串矩阵</param>
        ''' <returns>为了保证一一对应的映射关系，这个函数里面不再使用并行化拓展</returns>
        <Extension>
        Public Function VectorMapper(Of T As IEnumerable(Of IEnumerable(Of String)))(entities As T) As String()
            Dim dictTokens As Dictionary(Of String, Integer) =
                entities _
                .IteratesALL _
                .Distinct _
                .Select(Function(name, idx) (name:=name, idx:=idx)) _
                .ToDictionary(Function(entity) entity.name,
                              Function(entity) entity.idx)
            Dim Mappings = entities _
                .Select(Function(entity) entity.Select(Function(name) dictTokens(name))) _
                .ToArray
            Dim resultSet As String() = Mappings _
                .Select(Function(entity) entity.JoinBy(",")) _
                .ToArray
            Return resultSet
        End Function

        <Extension>
        Public Function VectorMapper(Of T, Tvalue)(source As Dictionary(Of T, Tvalue), lTokens As Func(Of Tvalue, String())) As Dictionary(Of T, String)
            Dim lst As String()() = source.Select(Function(x) lTokens(x.Value)).ToArray
            Dim mapps = VectorMapper(lst)
            Dim result As New Dictionary(Of T, String)
            Dim i As i32 = 0

            For Each x In source.ToArray
                Call result.Add(x.Key, mapps(++i))
            Next

            Return result
        End Function
    End Module
End Namespace
