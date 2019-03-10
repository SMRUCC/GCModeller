#Region "Microsoft.VisualBasic::958d0ba619acbe7e77c760a58d0bece8, LocalBLAST\LocalBLAST\LocalBLAST\Program\RpsBLAST.vb"

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

    '     Class RpsBLAST
    ' 
    '         Properties: LastBLASTOutputFilePath
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: MakeProfileDb, Performance
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace LocalBLAST.Programs

    Public NotInheritable Class RpsBLAST : Inherits NCBI.Extensions.LocalBLAST.InteropService.LocalBlastProgramGroup

        Protected Friend ReadOnly _MakeProfileDb As String
        Protected Friend ReadOnly _RpsBLAST As String

        Public ReadOnly Property LastBLASTOutputFilePath As String
            Get
                Return _InternalLastBLASTOutputFile
            End Get
        End Property

        Sub New(BLASTBin As String)
            Me._innerBLASTBinDIR = BLASTBin
            Me._MakeProfileDb = BLASTBin & "/makeprofiledb.exe"
            Me._RpsBLAST = BLASTBin & "/rpsblast.exe"
        End Sub

        Public Function MakeProfileDb(pnList As String) As CommandLine.IORedirect
            Dim TargetAssembly As String = FileIO.FileSystem.GetParentPath(pnList) & "/makeprofiledb.exe"
            Dim Cmdl As CommandLine.IORedirect = New CommandLine.IORedirect(TargetAssembly, String.Format("-in ""{0}"" -dbtype rps", pnList))
            Call FileIO.FileSystem.CopyFile(Me._MakeProfileDb, TargetAssembly)
            Call Console.WriteLine("[RPSBLAST_MAKEPROFILEDB]{0}  ---> ""{1}""", vbCrLf, Cmdl.ToString)

            Return Cmdl
        End Function

        Public Function Performance(FsaDb As String, rpsDb As String, Evalue As String, Output As String) As CommandLine.IORedirect
            Dim Argv As String = String.Format("-query ""{0}"" -evalue {1} -out ""{2}"" -db ""{3}""", FsaDb, Evalue, Output, rpsDb)
            Dim Cmdl As CommandLine.IORedirect = New CommandLine.IORedirect(Me._RpsBLAST, Argv)
            _InternalLastBLASTOutputFile = Output
            Call Console.WriteLine("[RPSBLAST]{0}  ---> ""{1}""", vbCrLf, Cmdl.ToString)

            Return Cmdl
        End Function
    End Class
End Namespace
