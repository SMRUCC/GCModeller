Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif

Module Module1

    Sub Main()

        Call loadTest()
    End Sub

    Sub loadTest()
        Dim orgs = "P:\XCC\models".LoadKEGGModels
        Dim models = ModelLoader.LoadGenomic("P:\XCC\assembly", "P:\XCC\models").ToArray

        Pause()
    End Sub
End Module
