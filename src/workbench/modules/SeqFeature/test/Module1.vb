#Region "Microsoft.VisualBasic::9977360ec56af771260d4b2c6e5a749f, SeqFeature\test\Module1.vb"

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
    '     Sub: Main, ruleTest, viewerTest
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.SeqFeature
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Protease

Module Module1

    Sub Main()

        ' Call ruleTest()

        '       Call {New CleavageRule}.SaveTo("./CleavageRule.csv")


        ' Pause()

        Call viewerTest()
    End Sub

    Sub ruleTest()

        Dim rules = "D:\GCModeller\src\GCModeller\analysis\ProteinTools\CleavageRule.csv".LoadCsv(Of CleavageRule)
        Dim seq = New FastaSeq With {.SequenceData = "SERVELAT"}

        Dim sites = seq.RunTest(proteases:=rules).ToArray

        Call "output result".__DEBUG_ECHO

        Call VBDebugger.WaitOutput()

        Call Console.WriteLine()
        Call Console.WriteLine()
        Call Console.WriteLine()

        For Each x As Site In sites
            x.Left += 2
        Next

        Call Console.WriteLine(sites.GetJson(indent:=True))

        Call Console.WriteLine()
        Call Console.WriteLine()
        Call Console.WriteLine()

        Call sites.DisplayOn(seq.SequenceData)
        Call Console.WriteLine()
        Call Console.WriteLine()
        Pause()
    End Sub

    Sub viewerTest()
        Dim sites As New List(Of Site)

        For Each line In "D:\GCModeller\src\workbench\SeqFeature\test.csv".ReadAllLines.Skip(1)
            Dim t = line.Split(","c)
            Dim name = t(0)
            Dim lefts = t(1).Split(" "c)

            For Each x In lefts
                sites.Add(New Site With {.Name = name, .Left = x})
            Next
        Next

        Dim seq = "SERVELAT"

        Call sites.DisplayOn(seq)

        Pause()
    End Sub
End Module
