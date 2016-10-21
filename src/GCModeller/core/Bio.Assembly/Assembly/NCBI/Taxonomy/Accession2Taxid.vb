Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI.Taxonomy

    Public Module Accession2Taxid

        Public Function LoadAll(DIR$) As BucketDictionary(Of String, Integer)
            Return DIR.__loadData _
                .CreateBuckets(Function(x) x.Name,
                               Function(x) x.x)
        End Function

        <Extension>
        Public Iterator Function ReadFile(file$) As IEnumerable(Of NamedValue(Of Integer))
            Dim line$
            Dim tokens$()

            Using reader As StreamReader = file.OpenReader
                Call reader.ReadLine() ' skip first line, headers

                Do While Not reader.EndOfStream
                    line = reader.ReadLine
                    tokens = line.Split(ASCII.TAB)

                    ' accession       accession.version       taxid   gi
                    Yield New NamedValue(Of Integer) With {
                        .Name = tokens(Scan0),
                        .x = CInt(Val(tokens(2))),
                        .Description = line
                    }
                Loop
            End Using
        End Function

        <Extension>
        Private Iterator Function __loadData(DIR$) As IEnumerable(Of NamedValue(Of Integer))
            For Each file$ In ls - l - r - "*.*" <= DIR
                For Each x In file.ReadFile
                    Yield x
                Next
            Next
        End Function

        Const null$ = Nothing

        ''' <summary>
        ''' 这个函数所返回来的数据之中是包含有表头的
        ''' </summary>
        ''' <param name="acc_list"></param>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function Matchs(acc_list As IEnumerable(Of String), DIR$) As IEnumerable(Of String)
            Dim list As Dictionary(Of String, String) = acc_list _
                .Distinct _
                .ToDictionary(Function(id) id,
                              Function(s) null)
            Yield {
                "accession", "accession.version", "taxid", "gi"
            }.JoinBy(vbTab)

            For Each x As NamedValue(Of Integer) In __loadData(DIR)
                If list.ContainsKey(x.Name) Then
                    Yield x.Description

                    If list.Count = 0 Then
                        Exit For
                    Else
                        list.Remove(x.Name)
                    End If
                End If
            Next
        End Function
    End Module
End Namespace