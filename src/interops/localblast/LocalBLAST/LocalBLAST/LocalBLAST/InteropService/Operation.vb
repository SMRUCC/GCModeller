#Region "Microsoft.VisualBasic::45d20c1b69781fa9e082eef269ba355c, localblast\LocalBLAST\LocalBLAST\LocalBLAST\InteropService\Operation.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 95
    '    Code Lines: 65 (68.42%)
    ' Comment Lines: 18 (18.95%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 12 (12.63%)
    '     File Size: 4.45 KB


    '     Class Operation
    ' 
    '         Properties: MolTypeNucleotide, MolTypeProtein
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Blastn, Blastp, CreateObject, CreateObjectFromFile, FormatDb
    '                   GetLastLogFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine

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

        Public Overrides Function Blastp(InputQuery As String, TargetSubjectDb As String, Output As String, Optional e As String = "10") As IORedirect
            Dim Cmdl = DirectCast(ProgramProfile.GetCommand("blastp"), Executable.Executable_BLAST).CreateCommand(InputQuery, TargetSubjectDb, e, Output)
            MyBase._InternalLastBLASTOutputFile = Output
            Return Cmdl
        End Function

        Public Overrides Function GetLastLogFile() As BLASTOutput.IBlastOutput
            Select Case ProgramProfile.Name.ToLower
                Case "blast+"
                    Return BLASTOutput.BlastPlus.Parser.TryParse(_InternalLastBLASTOutputFile)
                Case "rpsblast"
                    Return BLASTOutput.BlastPlus.Parser.TryParse(_InternalLastBLASTOutputFile)
                Case Else
                    Return BLASTOutput.XmlFile.BlastOutput.LoadFromFile(_InternalLastBLASTOutputFile)
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
            Dim LQuery = (From profileItem As ProgramProfiles
                          In ProgramProfiles.DefaultProfiles
                          Where String.Equals(profileItem.Name, TypeId, StringComparison.OrdinalIgnoreCase)
                          Select profileItem).ToArray
            If LQuery.IsNullOrEmpty Then
                Return New Operation(BlastBin) With {
                    .ProgramProfile = ProgramProfiles.LocalBLAST
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

        Public Overloads Overrides Function Blastn(Input As String, TargetDb As String, Output As String, Optional e As String = "10") As IORedirect
            Dim Cmdl = DirectCast(ProgramProfile.GetCommand("blastn"), Executable.Executable_BLAST).CreateCommand(Input, TargetDb, e, Output)
            MyBase._InternalLastBLASTOutputFile = Output
            Return Cmdl
        End Function

        Public Overloads Overrides Function FormatDb(Db As String, dbType As String) As IORedirect
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
