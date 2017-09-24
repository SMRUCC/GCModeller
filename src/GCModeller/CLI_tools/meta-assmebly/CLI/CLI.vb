#Region "Microsoft.VisualBasic::793b11e27e8e1bbb48dc3242193a76a6, ..\CLI_tools\meta-assmebly\CLI\CLI.vb"

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

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.Metagenome.UPGMATree

Module CLI

    <ExportAPI("/UPGMA.Tree")>
    <Usage("/UPGMA.Tree /in <in.csv> [/out <>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(DataSet)},
              Description:="The input matrix in csv table format for build and visualize as a UPGMA Tree.")>
    Public Function UPGMATree(args As CommandLine) As Integer
        Dim data As IEnumerable(Of DataSet) = DataSet.LoadDataSet(args <= "/in")
        Dim tree As Taxa = data.BuildTree
        Dim out$ = (args <= "/out") Or ((args <= "/in").TrimSuffix & ".txt").AsDefault

        With tree.ToString
            Call .__INFO_ECHO
            Call .SaveTo(out)
        End With

        Return 0
    End Function
End Module

