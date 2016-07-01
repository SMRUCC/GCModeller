Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.Application.RpsBLAST

    ''' <summary>
    ''' Cog分类操作
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ClassifyCOGs

        Dim _rpsCOG_Db As String
        Dim _RpsBLAST As LocalBLAST.Programs.RpsBLAST

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Blastbin"></param>
        ''' <param name="rpsCOG_db">Cog数据库</param>
        ''' <remarks></remarks>
        Sub New(Blastbin As String, rpsCOG_db As String)
            Me._RpsBLAST = New Programs.RpsBLAST(Blastbin)
            Me._rpsCOG_Db = rpsCOG_db
        End Sub

        Public Sub MakeCogRpsDb()
            Call _RpsBLAST.MakeProfileDb(_rpsCOG_Db).Start(WaitForExit:=True)
        End Sub

        Public Sub Performence(Query As String, EValue As String, Output As String)
            Call _RpsBLAST.Performance(Query, Me._rpsCOG_Db, EValue, Output).Start(WaitForExit:=True)
        End Sub

        Public Overloads Function Get_COGClassify(QueryGrepMethod As TextGrepMethod, Optional DescriptionGrepMethod As TextGrepMethod = Nothing) As DocumentStream.File

            Return ClassifyCOGs.Get_COGClassify(_RpsBLAST.LastBLASTOutputFilePath, QueryGrepMethod, DescriptionGrepMethod)
        End Function

        Public Overloads Shared Function Get_COGClassify(LogFile As String,
                                                         QueryGrepMethod As TextGrepMethod,
                                                         Optional DescriptionGrepMethod As TextGrepMethod = Nothing) As DocumentStream.File

            If Not String.IsNullOrEmpty(LogFile) AndAlso FileIO.FileSystem.FileExists(LogFile) Then
                Call Console.WriteLine("[COG_LOGRESULT]Load result file:{0}  ---> ""{1}""", vbCrLf, LogFile)

                Dim Log = NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParse(LogFile)
                Dim QueriesName As String() = (From query In Log.Queries Select query.QueryName).ToArray
                If Not QueryGrepMethod Is Nothing Then
                    Call Log.Grep(QueryGrepMethod, AddressOf Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("'tokens | 2';'tokens ' ' 1';'tokens , 0'").Grep)
                End If
                Call Console.WriteLine("Generating output document...")

                Dim File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = Log.ExportBestHit.ToCsvDoc
                If Not DescriptionGrepMethod Is Nothing Then
                    Call File.First().Add("Protein_description")
                    For i As Integer = 0 To QueriesName.Count - 1
                        Dim row = File(i + 1)
                        Call row.Add(DescriptionGrepMethod(QueriesName(i)))
                    Next
                End If
                Call Console.WriteLine("JobDone!!!")

                Return File
            Else
                Return New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            End If
        End Function

        Public Shared Function Get_MyvaCOG_Classify(path As String,
                                                    QueryGrepMethod As TextGrepMethod,
                                                    WhogXml As String,
                                                    Optional DescriptionGrepMethod As TextGrepMethod = Nothing) As MyvaCOG()
            If String.IsNullOrEmpty(path) OrElse Not FileIO.FileSystem.FileExists(path) Then
                Call Console.WriteLine("File ""{0}"" is not avaliable for cog analysis!", path)
                Return New MyvaCOG() {}
            End If

            Call Console.WriteLine("[COG_LOGRESULT]Load result file:{0}  ---> ""{1}""", vbCrLf, path)

            Dim Log = NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParse(path)
            Dim QueriesName As String() = (From query In Log.Queries Select query.QueryName).ToArray
            If Not QueryGrepMethod Is Nothing Then
                Call Log.Grep(QueryGrepMethod, Nothing)  'AddressOf NCBI.Extensions.LocalBLAST.GrepScript.Compile("'tokens | 2';'tokens ' ' 1';'tokens , 0'").Grep)
            End If
            Call Console.WriteLine("Generating output document...")

            Dim MyvaCOG = (From item In Log.ExportBestHit Select LocalBLAST.Application.RpsBLAST.MyvaCOG.CreateObject(item)).ToArray
            If Not DescriptionGrepMethod Is Nothing Then
                For i As Integer = 0 To QueriesName.Count - 1
                    Dim row = MyvaCOG(i)
                    row.Description = DescriptionGrepMethod(QueriesName(i))
                Next
            End If
            Call Console.WriteLine("JobDone!!!")

            Dim WhoCog = WhogXml.LoadXml(Of LocalBLAST.Application.RpsBLAST.Whog.Whog)()
            MyvaCOG = WhoCog.MatchCogCategory(MyvaCOG)

            Return MyvaCOG
        End Function
    End Class
End Namespace