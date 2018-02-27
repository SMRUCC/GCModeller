#Region "Microsoft.VisualBasic::ddd97c1fa3a0f787ec78bcf0650b6938, VennDiagram\VennDiagram\Program.vb"

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

    ' Module Program
    ' 
    '     Function: Main
    ' 
    '     Sub: New
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text

Module Program

    Public Const PlotTools As String = "R plot API"

    Sub New()
        Dim template As String = App.HOME & "/Templates/venn.csv"

        If Not template.FileExists Then
            Dim example As New IO.File

            example += {"Xcc8004", "ecoli", "pa14", "ftn", "aciad"}
            example += {"1", "1", "1", "1", "1"}
            example += {"1", "", "", "", "1"}
            example += {"", "", "1", "", "1"}
            example += {"", "1", "", "", "1"}
            example += {"1", "", "", "1", ""}

            Call example.Save(template, Encodings.ASCII)
        End If

        template = App.HOME & "/Templates/venn.partitions.csv"

        If Not template.FileExists Then
            Dim example As New IO.File

            example += {"serial", "color", "Title"}
            example += {"Xcc8004", "blue", "Xanthomonas campestris pv. campestris str. 8004"}
            example += {"ecoli", "green", "Ecoli. K12"}
            example += {"pa14", "yellow", "PA14"}
            example += {"ftn", "black", "FTN"}
            example += {"aciad", "red", "ACIAD"}

            Call example.Save(template, Encodings.ASCII)
        End If
    End Sub

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine, AddressOf CLI.DrawFile)
    End Function
End Module
