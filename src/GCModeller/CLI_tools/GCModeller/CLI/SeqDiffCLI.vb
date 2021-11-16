#Region "Microsoft.VisualBasic::f9cca34a484673fd30db32b4d06239c9, CLI_tools\GCModeller\CLI\SeqDiffCLI.vb"

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

    ' Module CLI
    ' 
    '     Function: SeqDiffCLI
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/seqdiff", Usage:="/seqdiff /in <mla.fasta> [/toplog <file-list.txt> /winsize 250 /steps 50 /slides 5 /out <out.csv>]")>
    <ArgumentAttribute("/toplog", True, AcceptTypes:={GetType(String())},
                   Description:="Put these directory path in the item order of:
+ hairpinks
+ perfects palindrome
+ repeats view
+ rev-repeats view")>
    Public Function SeqDiffCLI(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim winsize As Integer = args.GetValue("/winsize", 250)
        Dim steps As Integer = args.GetValue("/steps", 50)
        Dim slides As Integer = args.GetValue("/slides", 5)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".winsize={winsize},steps={steps},slides={slides}_seqdiff.csv")
        Dim mla As New FastaFile([in])
        Dim result = mla.Select(AddressOf SeqDiff.Parser)
        Dim t As String = args("/toplog")

        Call SeqDiff.GCOutlier(mla, result, {0.95, 0.99, 1}, winsize, steps, slides)

        If t.FileExists Then
            Dim list As String() = t.ReadAllLines

            Call SeqDiff.ApplyPalindrom(mla, result, list(0), "hairpink")
            Call SeqDiff.ApplyPalindrom(mla, result, list(1), "palindrome")
            Call SeqDiff.ApplyRepeats(mla, result, list(2), False)
            Call SeqDiff.ApplyRepeats(mla, result, list(3), True)
        End If

        Return result.SaveTo(out).CLICode
    End Function
End Module
