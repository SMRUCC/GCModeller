Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.EBI.ChEBI.XML

    <XmlRoot("ChEBI-DataSet", [Namespace]:="http://gcmodeller.org/core/chebi/dataset.XML")>
    Public Class EntityList

        <XmlElement("chebi-entity")>
        Public Property DataSet As ChEBIEntity()

        Public Function ToSearchModel() As Dictionary(Of Long, ChEBIEntity)
            Dim table As New Dictionary(Of Long, ChEBIEntity)

            For Each chemical As ChEBIEntity In DataSet
                Dim id& = chemical.Address

                If Not table.ContainsKey(id) Then
                    table.Add(id, chemical)
                End If
            Next

            Return table
        End Function

        Public Function AsList() As HashList(Of ChEBIEntity)
            Dim list As New HashList(Of ChEBIEntity)

            For Each chemical As ChEBIEntity In DataSet
                Call list.Add(chemical)
            Next

            Return list
        End Function

        Public Overrides Function ToString() As String
            If DataSet.IsNullOrEmpty Then
                Return "No items"
            Else
                Return $"list of {DataSet.Length} chebi entity: ({DataSet.Take(10).Keys.GetJson}...)"
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadDirectory(folder$) As EntityList
            Return Extensions.Compile(folder)
        End Function
    End Class

    Public Module Extensions

        ''' <summary>
        ''' 将单个的chebi分子数据文件合并在一个大文件之中，方便进行数据的加载
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        Public Function Compile(DIR$) As EntityList
            Dim list As New Dictionary(Of ChEBIEntity)

            For Each path As String In ls - l - r - "*.XML" <= DIR
                For Each chemical As ChEBIEntity In path.LoadXml(Of ChEBIEntity())
                    If Not list & chemical Then
                        list += chemical
                    End If
                Next
            Next

            Return New EntityList With {
                .DataSet = list _
                    .Values _
                    .ToArray
            }
        End Function
    End Module
End Namespace