Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.ContextModel.Promoter

Module Module1

    Sub Main()

        Call scanerTest()

        Call loadTest()
    End Sub

    Sub scanerTest()

        Dim orgs = "P:\XCC\models".LoadKEGGModels
        Dim models = ModelLoader.LoadGenomic("P:\XCC\assembly", "P:\XCC\models").ToArray

        Dim scaner As New ConsensusScanner(models)

    End Sub


    Sub loadTest()
        Dim orgs = "P:\XCC\models".LoadKEGGModels
        Dim models = ModelLoader.LoadGenomic("P:\XCC\assembly", "P:\XCC\models").ToArray
        Dim upstreams = models(0).GetUpstreams(PrefixLength.L150)

        Pause()
    End Sub
End Module
