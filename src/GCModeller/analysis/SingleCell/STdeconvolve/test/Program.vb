Imports Microsoft.VisualBasic.Data.NLP.LDA
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SingleCell.STdeconvolve

Module Program
    Sub Main(args As String())
        Dim STdataset As Matrix = Matrix.LoadData("E:\GCModeller\src\GCModeller\analysis\PhenoGraph\demo\HR2MSI mouse urinary bladder S096_top3.csv")
        Dim corpus As STCorpus = STdataset.CreateSpatialDocuments
        Dim result = corpus.LDAModelling(13).Deconvolve(corpus)

        Call result.GetJson.SaveTo("E:\GCModeller\src\GCModeller\analysis\STdeconvolve\demo\HR2MSI mouse urinary bladder S096_top3.json")

        Pause()
    End Sub
End Module
