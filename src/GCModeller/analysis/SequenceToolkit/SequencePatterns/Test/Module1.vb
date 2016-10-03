#Region "Microsoft.VisualBasic::c7cfe5201ac6d2f9ced5a008799ea14f, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Test\Module1.vb"

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

Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif.Patterns
Imports LANS.SystemsBiology.SequenceModel.FASTA

Module Module1

    Sub Main()

        Dim s = "[AG]CGTT[AC]G[ATC]"
        Dim st = PatternParser.SimpleTokens(s)




        Dim scan As New Scanner(New FastaToken("F:\Xanthomonas_campestris_8004_uid15\CP000050.fna"))
        Dim result = scan.Scan(s)

        Dim motif As String = "[AG]{2,7}at*g+G4A{3,5}G{29}N?N{x}-(aa{x}TGA{b}){3,7}~x={2,5};b={x+2}"
        Dim tokens = PatternParser.ExpressionParser(motif)
    End Sub

End Module
