Namespace LocalBLAST.InteropService

    ''' <summary>
    ''' 通用化操作
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Operation : Inherits LocalBLAST.InteropService.InteropService

        Protected Friend ProgramProfile As NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles

        Protected Sub New(BlastBin As String)
            Call MyBase.New(BlastBin)
        End Sub

        Public Overrides Function Blastp(InputQuery As String, TargetSubjectDb As String, Output As String, Optional e As String = "10") As CommandLine.IORedirectFile
            Dim Cmdl = DirectCast(ProgramProfile.GetCommand("blastp"), Executable.Executable_BLAST).CreateCommand(InputQuery, TargetSubjectDb, e, Output)
            MyBase._InternalLastBLASTOutputFile = Output
            Return Cmdl
        End Function

        Public Overrides Function GetLastLogFile() As BLASTOutput.IBlastOutput
            Select Case ProgramProfile.Name.ToLower
                Case "localblast"
                    Return SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.TryParse(_InternalLastBLASTOutputFile)
                Case "blast+"
                    Return SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParse(_InternalLastBLASTOutputFile)
                Case "rpsblast"
                    Return SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParse(_InternalLastBLASTOutputFile)
                Case Else
                    Return SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile.BlastOutput.LoadFromFile(_InternalLastBLASTOutputFile)
            End Select
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="BlastBin"></param>
        ''' <param name="TypeId">if the given key is not exists in the default profile collection then the function will return the standard LOCALBLAST profile as default.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject(BlastBin As String, TypeId As String) As Operation
            Dim LQuery = (From profileItem As SMRUCC.genomics.NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles In SMRUCC.genomics.NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles.DefaultProfiles
                          Where String.Equals(profileItem.Name, TypeId, StringComparison.OrdinalIgnoreCase)
                          Select profileItem).ToArray
            If LQuery.IsNullOrEmpty Then
                Return New Operation(BlastBin) With {
                    .ProgramProfile = NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles.LocalBLAST
                }
            Else
                Return New Operation(BlastBin) With {
                    .ProgramProfile = LQuery.First
                }
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="BlastBin"></param>
        ''' <param name="FilePath">Profile file path of <see cref="NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles"></see></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObjectFromFile(BlastBin As String, FilePath As String) As Operation
            Dim ProgramProfile As NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles =
                FilePath.LoadXml(Of NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles)()
            Return New NCBI.Extensions.LocalBLAST.InteropService.Operation(BlastBin) With {
                .ProgramProfile = ProgramProfile
            }
        End Function

        Public Overloads Overrides Function Blastn(Input As String, TargetDb As String, Output As String, Optional e As String = "10") As CommandLine.IORedirectFile
            Dim Cmdl = DirectCast(ProgramProfile.GetCommand("blastn"), Executable.Executable_BLAST).CreateCommand(Input, TargetDb, e, Output)
            MyBase._InternalLastBLASTOutputFile = Output
            Return Cmdl
        End Function

        Public Overloads Overrides Function FormatDb(Db As String, dbType As String) As CommandLine.IORedirectFile
            Dim Cmdl = DirectCast(ProgramProfile.GetCommand("builddb"), Executable.Executable_BuildDB).CreateCommand(Db, dbType)
            Return Cmdl
        End Function

        Public Overrides ReadOnly Property MolTypeNucleotide As String
            Get
                Return ProgramProfile.MoltypeNucleotide
            End Get
        End Property

        Public Overrides ReadOnly Property MolTypeProtein As String
            Get
                Return ProgramProfile.MolTypeProtein
            End Get
        End Property
    End Class
End Namespace