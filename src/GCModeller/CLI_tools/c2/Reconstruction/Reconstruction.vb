Namespace Reconstruction

    Public Class Reconstruction : Implements System.IDisposable

        Dim Reconstructed As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.PGDB

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>返回给Main函数的状态值</returns>
        ''' <remarks></remarks>
        Public Function Invoke(Reconstructed As String, Subject As String, WorkDir As String, ReconstructedExport As String) As Integer
            Dim blast As New LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.InitializeParameter(
                GCModeller.FileSystem.GetLocalBlast,
                LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.InitializeMethods.Program.BlastPlus)
            Dim Session As c2.Reconstruction.Operation.OperationSession =
                New Operation.OperationSession(Reconstructed, Subject, blast, WorkDir, Settings.SettingsFile.C2.MotifSampler)
            Call Session.Intialize()

            Session.MYSQL = MySQLExtensions.MySQL

            Dim ProteinCPLXrct As c2.Reconstruction.ProteinCPLX = New ProteinCPLX(Session)
            Dim Promotersrct As c2.Reconstruction.Promoters = New Promoters(Session)

            Dim ObjectEqualsSession As c2.Reconstruction.ObjectEquals.Session = New ObjectEquals.Session(Session)
            Dim Metabolismrct As c2.Reconstruction.MetabolismPathways = New MetabolismPathways(Session, ObjectEqualsSession.ProteinEquals, ObjectEqualsSession.ReactionEquals)

            Call ProteinCPLXrct.Performance()
            Call Promotersrct.Performance()
            Call ObjectEqualsSession.Initialize()
            ObjectEqualsSession.PromoterEquals = ObjectEquals.Promoters.Create(Promotersrct)

            Call Metabolismrct.Performance()

            Dim TranscriptUnitrct As c2.Reconstruction.TranscriptUnit = New TranscriptUnit(Session, Promotersrct.ReconstructList)
            Call TranscriptUnitrct.Performance()
            Dim Regulationrct As c2.Reconstruction.RegulationNetwork = New RegulationNetwork(Session, ObjectEqualsSession)
            Call Regulationrct.Performance()

            Call Session.ReconstructedMetaCyc.Database.Save(ReconstructedExport)
            Me.Reconstructed = Session.ReconstructedMetaCyc.Database

            Return 0
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Export">经过编译后的模型文件的输出文件名</param>
        ''' <param name="Log">输出的日志文件</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Compile(Export As String, Log As String) As Integer
            Dim LogFile As Microsoft.VisualBasic.Logging.LogFile = New Microsoft.VisualBasic.Logging.LogFile(Log)

            If String.IsNullOrEmpty(Export) Then
                Export = My.Application.Info.DirectoryPath & "/" & Me.Reconstructed.DataDir.Replace("\", "/").Split.Last & ".xml"
                LogFile.WriteLine(String.Format("User not specific the output file for the compiled model, the compiled model file will be save to location:{0}  ""{1}""", vbCrLf, Export),
                                  "gcc_main() -> compile_metacyc", Microsoft.VisualBasic.Logging.MSG_TYPES.WRN)
            End If

            LogFile.WriteLine(String.Format("Start to compile metacyc database:{0}  ""{1}""", vbCrLf, Me.Reconstructed.DataDir), "gcc_main() -> compile_metacyc", Microsoft.VisualBasic.Logging.MSG_TYPES.INF)

            Dim TagFilters = (From Filter In Settings.SettingsFile.Gcc.Filters Select New LANS.SystemsBiology.Assembly.SBML.Replacement With {
                                                                                   .NewReplaced = Filter.NewReplaced, .Old = Filter.Old}).ToArray
            Dim Compiler As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.Builder.Compiler =
                New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.Builder.Compiler With
                {
                    .StringReplacements = TagFilters,
                    .LogFile = LogFile
            }
            Call Compiler.PreCompile(Me.Reconstructed.DataDir)
            Call Compiler.Return.Save(Export)
            LogFile.SaveLog(appendToLogFile:=False)

            Return 0
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace