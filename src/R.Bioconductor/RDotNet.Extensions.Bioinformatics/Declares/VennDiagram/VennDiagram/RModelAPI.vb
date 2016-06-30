Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports RDotNET.Extensions.VisualBasic

Namespace VennDiagram.ModelAPI

    Public Module RModelAPI

        ''' <summary>
        ''' 从一个Excel逗号分割符文件之中生成一个文氏图的数据模型
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Generate(source As DocumentStream.File) As VennDiagram
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
                              lst = New List(Of String)).ToArray

            For row As Integer = 1 To source.RowNumbers - 1
                Dim Line As RowObject = source(row)
                For colums As Integer = 0 To Width - 1
                    If Not String.IsNullOrEmpty(Line.Column(colums).Trim) Then
                        Call Vector(colums).lst.Add(CStr(row))
                    End If
                Next
            Next

            Return Vector.ToDictionary(Function(x) x.k, Function(x) x.lst.ToArray)
        End Function

        ''' <summary>
        ''' 从实际的对象映射到venn图里面的实体标记
        ''' </summary>
        ''' <returns>为了保证一一对应的映射关系，这个函数里面不再使用并行化拓展</returns>
        <Extension>
        Public Function VectorMapper(entities As IEnumerable(Of String), lTokens As Func(Of String, String())) As String()
            Dim Tokens As String()() = entities.ToArray(Function(entity) lTokens(entity))
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
            entities.MatrixAsIterator.Distinct.ToArray(
              Function(name, idx) New With {.name = name, .idx = idx}) _
                  .ToDictionary(Function(entity) entity.name,
                                Function(entity) entity.idx)
            Dim Mappings = entities.ToArray(Function(entity) entity.ToArray(Function(name) dictTokens(name)))
            Dim resultSet As String() = Mappings.ToArray(Function(entity) entity.JoinBy(","))
            Return resultSet
        End Function

        <Extension>
        Public Function VectorMapper(Of T, Tvalue)(source As Dictionary(Of T, Tvalue), lTokens As Func(Of Tvalue, String())) As Dictionary(Of T, String)
            Dim lst As String()() = source.ToArray(Function(x) lTokens(x.Value))
            Dim mapps = VectorMapper(lst)
            Dim result As New Dictionary(Of T, String)
            Dim i As Integer = 0

            For Each x In source.ToArray
                Call result.Add(x.Key, mapps(i.MoveNext))
            Next

            Return result
        End Function
    End Module
End Namespace