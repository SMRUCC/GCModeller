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
        For Each db$ In ls - l - lsDIR <= DATA
            Dim name$ = db$.BaseName

            If name.TextEquals("headers") OrElse name.TextEquals("index") Then
                Continue For
            End If

            Call $"Loading {name}...".__DEBUG_ECHO

            For Each nt$ In ls - l - r - wildcards("*.nt") <= db$
                Dim index As New Index(DATA, name, nt$.BaseName)
                Dim title As New TitleIndex(DATA, name, nt$.BaseName)

                Call __nt.Add(index)
                Call __headers.Add(title)
            Next
        Next

        Return __nt.Values.Sum(Function(i) i.Size)
    End Function

    ''' <summary>
    ''' 请参考搜索引擎的语法，假若查询里面含有符号的话，会被当作分隔符来看待，所以假若符号也要被匹配出来的话，需要添加双引号
    ''' </summary>
    ''' <param name="query$"></param>
    ''' <returns></returns>
    Public Iterator Function Search(query$) As IEnumerable(Of FastaToken)
        Dim LQuery = From db As TitleIndex
                     In __headers.Values.AsParallel
                     Let expression As Expression = Build(query$)
                     Let def As IObject = db.GetDef
                     Select db.EnumerateTitles _
                         .AsParallel _
                         .Where(Function(x) expression.Evaluate(def, x))

        For Each x As NamedValue(Of String) In LQuery.MatrixAsIterator
            Dim seq$ = __nt(x.Description) _
                .ReadNT_by_gi(gi:=x.Name)

            Yield New FastaToken With {
                .Attributes = {"gi", x.Name, x.x},
                .SequenceData = seq
            }
        Next
    End Function
End Class
