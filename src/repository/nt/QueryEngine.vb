Imports Microsoft.VisualBasic.Language.UnixBash
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports mysqlClient = Oracle.LinuxCompatibility.MySQL.MySQL

Public Class QueryEngine

    ReadOnly __nt As New Dictionary(Of Index)
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
                Call __nt.Add(index)
            Next
        Next

        Return __nt.Values.Sum(Function(i) i.Size)
    End Function

    Public Iterator Function Search(query$) As IEnumerable(Of FastaToken)

    End Function
End Class
