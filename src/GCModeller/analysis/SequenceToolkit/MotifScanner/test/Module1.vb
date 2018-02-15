#Region "Microsoft.VisualBasic::421a11d4b38638e3e118ad51961eafb4, analysis\SequenceToolkit\MotifScanner\test\Module1.vb"

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

' Module Module1
' 
'     Sub: loadTest, Main, scanerTest, seeding
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()

        '  Call msaTest2()

        Call seeding()

        Call scanerTest()

        Call loadTest()
    End Sub

    Sub msaTest2()
        Dim seqs$ = "ATGAAT-
ATGAAT-
ATGAAT-
ATGAAT-
ATGAAT-
ATGAAT-
ATGAAT-
ATGAAT-
ATGAAT-"

        Dim c As New CenterStar(seqs.lTokens)
        Dim r = c.Compute
    End Sub

    Sub seeding()
        Dim test As FastaFile = FastaFile.LoadNucleotideData("E:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\K03406_small.fasta")
        Dim result = test.PopulateMotifs.ToArray

        Call result.GetJson(indent:=True).SaveTo("./motifs.json")

        Pause()
    End Sub

    Sub scanerTest()

        Dim orgs = "P:\XCC\models".LoadKEGGModels
        Dim models = ModelLoader.LoadGenomic("P:\XCC\assembly", "P:\XCC\models").ToArray

        Dim scaner As New ConsensusScanner(models)
        '  Dim result = scaner.PopulateMotifs("K03406").ToArray

        Call scaner.DumpSequence("K03406", "./K03406.fasta")

        Pause()
    End Sub


    Sub loadTest()
        Dim orgs = "P:\XCC\models".LoadKEGGModels
        Dim models = ModelLoader.LoadGenomic("P:\XCC\assembly", "P:\XCC\models").ToArray
        Dim upstreams = models(0).GetUpstreams(PrefixLength.L150)

        Pause()
    End Sub
End Module

