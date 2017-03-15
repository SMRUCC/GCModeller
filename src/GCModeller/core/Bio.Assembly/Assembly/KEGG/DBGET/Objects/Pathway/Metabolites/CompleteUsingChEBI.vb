Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables

Namespace Assembly.KEGG.DBGET.bGetObject

    Partial Module MetabolitesDBGet

        Const ChEBI$ = NameOf(ChEBI) & "_unclassified"

        ''' <summary>
        ''' 所下载的数据都会被放在一个名字叫做<see cref="ChEBI"/>的文件夹之中
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <param name="accessionTsv$"></param>
        ''' <param name="forceUpdate"></param>
        ''' <returns></returns>
        Public Function CompleteUsingChEBI(DIR$, accessionTsv$, Optional forceUpdate As Boolean = False) As String()
            Dim accs As Accession() = Accession _
                .Load(accessionTsv, type:="KEGG COMPOUND accession") _
                .Values _
                .Select(Function(x) x.Value) _
                .IteratesALL _
                .ToArray
            ' 这里是已经下载的文件列表
            Dim downloads As New IndexOf(Of String)((ls - l - r - "*.xml" <= DIR).Select(AddressOf BaseName))
            Dim path$
            Dim failures As New List(Of String)
            Dim ETA$

            DIR = DIR & "/" & ChEBI

            Using progress As New ProgressBar("Download missing ChEBI compounds data...", cls:=True)
                Dim tick As New ProgressProvider(accs.Length)

                Call $"Have {downloads.Count} compounds have been downloaded...".__DEBUG_ECHO

                For Each acc As Accession In accs

                    If downloads(acc.ACCESSION_NUMBER) > -1 Then
                        ' 已经有数据了，直接跳过
                        Call $"Skip download compounds model: {acc.GetJson}".__DEBUG_ECHO
                        GoTo EXIT_LOOP
                    Else
                        path = DIR & "/" & acc.ACCESSION_NUMBER & ".xml"
                    End If

                    ' 检查是否存在
                    If Not forceUpdate AndAlso path.FileExists(ZERO_Nonexists:=True) Then
                        Call $"Skip download compounds model: {acc.GetJson}".__DEBUG_ECHO
                        GoTo EXIT_LOOP
                    Else
                        Thread.Sleep(1000)
                    End If

                    Dim cpd As Compound = MetabolitesDBGet.DownloadCompound(acc.ACCESSION_NUMBER)

                    If cpd Is Nothing Then
                        ' 没有下载成功
                        failures += acc.ACCESSION_NUMBER
                    Else
                        Call cpd.SaveAsXml(path)
                    End If
EXIT_LOOP:
                    ETA = tick.ETA(progress.ElapsedMilliseconds).FormatTime
                    Call progress.SetProgress(
                        tick.StepProgress,
                        detail:=ETA)
                Next
            End Using

            Return failures
        End Function
    End Module
End Namespace