Imports RouterAdapter
Imports SMRUCC.genomics.Data.BioCyc

Module PathwayFinderDemo

    Sub Run()
        Dim biocyc As Workspace = Workspace.Open("F:\ecoli\29.0")
        Dim router As New BioCycAdapter(biocyc)

    End Sub
End Module
