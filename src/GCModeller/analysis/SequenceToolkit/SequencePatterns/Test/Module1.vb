#Region "Microsoft.VisualBasic::d521f9a0205f76edbcc2da5e074c2a66, GCModeller\analysis\SequenceToolkit\SequencePatterns\Test\Module1.vb"

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

    '   Total Lines: 45
    '    Code Lines: 27
    ' Comment Lines: 0
    '   Blank Lines: 18
    '     File Size: 1.42 KB


    ' Module Module1
    ' 
    '     Sub: Main, plotTest
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Motif.Patterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports scanerMotif = SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.Motif

Module Module1

    Sub Main()

        Call plotTest()


        Dim s = "[AG]CGTT[AC]G[ATC]"
        Dim st = PatternParser.SimpleTokens(s)




        Dim scan As New Scanner(New FastaSeq("F:\Xanthomonas_campestris_8004_uid15\CP000050.fna"))
        Dim result = scan.Scan(s)

        Dim motif As String = "[AG]{2,7}at*g+G4A{3,5}G{29}N?N{x}-(aa{x}TGA{b}){3,7}~x={2,5};b={x+2}"
        Dim tokens = PatternParser.ExpressionParser(motif)
    End Sub

    Sub plotTest()

        Dim motifs = "E:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\K03406_small.json".ReadAllText.LoadObject(Of scanerMotif())

        For i As Integer = 0 To motifs.Length - 1
            Dim test = motifs(i).CreateDrawingModel

            Call test.InvokeDrawing(True).SaveAs($"./test__{i}.png")
        Next




        Pause()
    End Sub
End Module
