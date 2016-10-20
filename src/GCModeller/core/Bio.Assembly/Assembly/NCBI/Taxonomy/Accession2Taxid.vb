Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI

    Public Module Accession2Taxid

        Public Function LoadAll(DIR$) As BucketDictionary(Of String, Integer)
            Return DIR.__loadData _
                .CreateBuckets(Function(x) x.Name,
                               Function(x) x.x)
        End Function

        <Extension>
        Private Iterator Function __loadData(DIR$) As IEnumerable(Of NamedValue(Of Integer))
            Dim line$
            Dim tokens$()

            For Each file$ In ls - l - r - "*.dmp" <= DIR
                Using reader = file.OpenReader
                    Do While Not reader.EndOfStream
                        line = reader.ReadLine
                        tokens = line.Split(ASCII.TAB)

                        Yield New NamedValue(Of Integer) With {
                            .Name = tokens(Scan0),
                            .x = CInt(Val(tokens(1)))
                        }
                    Loop
                End Using
            Next
        End Function
    End Module
End Namespace