Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module Module1

    Sub Main()
        Dim raw As Matrix = Matrix.LoadData("D:\GCModeller\src\GCModeller\annotations\WGCNA\metabolome.csv")
        Dim out = WGCNA.Analysis.Run(raw)

        Pause()
    End Sub

End Module
