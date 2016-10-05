Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports mysqlClient = Oracle.LinuxCompatibility.MySQL.MySQL

Public Class QueryEngine

    ReadOnly __nt As New Dictionary(Of Index)
    ReadOnly __headers As New Dictionary(Of TitleIndex)
    ReadOnly mysql As New mysqlClient

    ''' <summary>
    ''' 创建以及测试数据库连接
    ''' </summary>
    Sub New(uri As ConnectionUri)
        If (mysql <= uri) = -1.0R Then
            Throw New Exception("No mysql connection!")
        End If
    End Sub

    Sub New()
    End Sub

    Public Function ScanSeqDatabase(DATA$) As Long
        For Each db$ In ls - l - r - lsDIR <= DATA
            Dim name$ = db$.BaseName

            For Each nt$ In ls - l - r - wildcards("*.nt") <= db$
                Dim index As New Index(DATA, name, nt$.BaseName)
                Dim title As New TitleIndex(DATA, name, nt$.BaseName)

                Call __nt.Add(index)
                Call __headers.Add(title)
            Next
        Next

        Return __nt.Values.Sum(Function(i) i.Size)
    End Function

    Public Iterator Function Search(query$) As IEnumerable(Of FastaToken)
        Dim expression As Expression = Build(query$)

        For Each o As IObject In __headers.Values _
            .Select(Function(x) x.EnumerateTitles) _
            .MatrixAsIterator _
            .ForEach()

            If True = expression.Evaluate(x:=o) Then
                Dim x As NamedValue(Of String) = DirectCast(o.x, NamedValue(Of String))
                Dim seq$ = __nt(x.Description).ReadNT_by_gi(gi:=x.Name)

                Yield New FastaToken With {
                    .Attributes = {"gi", x.Name, x.x},
                    .SequenceData = seq
                }
            End If
        Next
    End Function
End Class
