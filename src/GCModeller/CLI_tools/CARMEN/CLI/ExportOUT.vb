#Region "Microsoft.VisualBasic::6fa60c6c690279b72986b8bd628676f1, ..\GCModeller\CLI_tools\CARMEN\CLI\ExportOUT.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports SMRUCC.genomics.Interops

Partial Module CLI

    <ExportAPI("/Export.Anno", Usage:="/Export.Anno /in <inDIR> [/out <out.Csv>]")>
    Public Function ExportAnno(args As CommandLine.CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".CARMEN.Csv")
        Dim rxnDefs As CARMEN.Reaction() = CARMEN.Merge(inDIR, False)
        Return rxnDefs.SaveTo(out).CLICode
    End Function
End Module

