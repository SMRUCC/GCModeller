Imports Microsoft.VisualBasic.Data.NLP.LDA
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SingleCell.STdeconvolve

Module Program
    Sub Main(args As String())
        Dim STdataset As Matrix = Matrix.LoadData("E:\GCModeller\src\GCModeller\analysis\PhenoGraph\demo\HR2MSI mouse urinary bladder S096_top3.csv")
        Dim corpus As Corpus = STdataset.CreateSpatialDocuments
        Dim result = corpus.LDAModelling(13).Deconvolve(corpus)


        Pause()
    End Sub
End Module
