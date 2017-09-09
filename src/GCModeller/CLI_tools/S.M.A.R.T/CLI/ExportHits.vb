#Region "Microsoft.VisualBasic::e82dd5402ecfe47b51cb5058412d91bb, ..\CLI_tools\S.M.A.R.T\CLI\ExportHits.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    '<Command("export_hits", usage:="export_hits -i <input_dir> -o <output_file> -fsa <fasta_file>")>
    'Public Shared Function ExportHits(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
    '    Dim XmlLogDir As String = CommandLine("-i")
    '    Dim SavedFile As String = CommandLine("-o")
    '    Dim FASTA As SequenceModel.FASTA.File = SequenceModel.FASTA.File.Read(CommandLine("-fsa"))
    '    Dim Hits As List(Of String) = New List(Of String)
    '    Dim LQuery = (From Log In FileIO.FileSystem.GetFiles(XmlLogDir, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
    '                  Let XmlLog = NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.Load(Log)
    '                  Select Hits.Append(XmlLog.GetAllits())) '
    '    LQuery = LQuery.ToArray
    '    Hits = (From s As String In Hits Select s Distinct Order By s Ascending).AsList

    '    Dim FASTAList As List(Of SequenceModel.FASTA.FASTA) = New List(Of SequenceModel.FASTA.FASTA)
    '    LQuery = From Id In Hits Select FASTAList.Append(FASTA.Query(Id, CompareMethod.Binary)) '
    '    LQuery = LQuery.ToArray
    '    FASTA = FASTAList

    '    Call FASTA.Save(SavedFile)
    '    Return 0
    'End Function
End Module
