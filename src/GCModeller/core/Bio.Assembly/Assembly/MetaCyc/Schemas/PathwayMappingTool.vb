Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem

Namespace Assembly.MetaCyc.Schema.Metabolism

    ''' <summary>
    ''' 使用一个汇总的MetaCyc数据库，根据目标物种的基因组以及蛋白质信息进行MetaCyc数据库的重建工作
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class PathwayMappingTool

        Dim MetaCyc As DatabaseLoadder

        ''' <summary>
        ''' 用于进行参考的MetaCyc数据库
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <remarks></remarks>
        Sub New(MetaCyc As DatabaseLoadder)
            Me.MetaCyc = MetaCyc
        End Sub

        Public Sub Initlaize()
            If Me.MetaCyc.Database.FASTAFiles.protseq.IsNullOrEmpty Then
                Dim LQuery = (From Protein As Slots.Protein
                              In Me.MetaCyc.GetProteins.Values
                              Let Id As String = DBLinkManager.DBLink.GetUniprotId(Protein.DBLinksMgr)
                              Where Not String.IsNullOrEmpty(Id)
                              Select Uniprot.Web.DownloadProtein(Id)).ToArray
                Call CType(LQuery, SequenceModel.FASTA.FastaFile).Save(Me.MetaCyc.Database.FASTAFiles.ProteinSourceFile)
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Proteins"></param>
        ''' <param name="SavedFile"></param>
        ''' <returns>返回序列下载结果，当所有的序列结果都下载完成的时候，返回0，当出现没有被下载的序列的情况时，返回未被下载的序列数</returns>
        ''' <remarks></remarks>
        Public Shared Function DownloadFromUniprot(Proteins As Proteins, SavedFile As String) As Integer
            Dim NotDownloads As Integer = 0

            For Each Protein As Slots.Protein In Proteins
                Dim Id As String = DBLinkManager.DBLink.GetUniprotId(Protein.DBLinksMgr)

                If String.IsNullOrEmpty(Id) Then '未被下载的序列对象
                    Dim Err As String = String.Format("[FASTA_OBJECT_NOT_DOWNLOAD] {0}", Protein.Identifier)
                    NotDownloads += 1
                    Call Console.WriteLine(Err)
                    Call FileIO.FileSystem.WriteAllText(App.CurrentDirectory & "/Err.log", Err & vbCrLf, append:=True)
                Else
                    Dim fasta As SequenceModel.FASTA.FastaToken = Uniprot.Web.DownloadProtein(UniprotId:=Id)
                    If Len(fasta.SequenceData) = 0 Then
                        Dim Err As String = String.Format("[FASTA_OBJECT_NOT_DOWNLOAD] {0}", Protein.Identifier)
                        NotDownloads += 1
                        Call Console.WriteLine(Err)
                        Call FileIO.FileSystem.WriteAllText(App.CurrentDirectory & "/Err.log", Err & vbCrLf, append:=True)
                    Else
                        fasta.Attributes = {"gnl", Id, String.Format("{0} {1} 0..0 Unknown", Protein.Identifier, Regex.Match(fasta.Attributes.Last, "GN=\S+").Value.Split(CChar("=")).Last)}
                        Call FileIO.FileSystem.WriteAllText(SavedFile, fasta.GenerateDocument(LineBreak:=60), append:=True, encoding:=System.Text.Encoding.ASCII)
                    End If
                End If
            Next

            Return NotDownloads
        End Function

        Public Overrides Function ToString() As String
            Return MetaCyc.ToString
        End Function
    End Class
End Namespace