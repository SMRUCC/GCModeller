Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.GCModeller.Workbench.SeqFeature
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Protease

Module Module1

    Sub Main()

        Call ruleTest()

        Call {New CleavageRule}.SaveTo("./CleavageRule.csv")


        Pause()

        Call viewerTest()
    End Sub

    Sub ruleTest()

        Dim rules = "E:\GCModeller\src\GCModeller\analysis\ProteinTools\CleavageRule.csv".LoadCsv(Of CleavageRule)
        Dim seq = New FastaSeq With {.SequenceData = "SERVELAT"}

        Dim sites = seq.RunTest(proteases:=rules).ToArray

        Call "output result".__DEBUG_ECHO
        Call sites.DisplayOn(seq.SequenceData)

        Pause()
    End Sub

    Sub viewerTest()
        Dim sites As New List(Of Site)

        For Each line In "E:\GCModeller\src\workbench\SeqFeature\test.csv".ReadAllLines.Skip(1)
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
