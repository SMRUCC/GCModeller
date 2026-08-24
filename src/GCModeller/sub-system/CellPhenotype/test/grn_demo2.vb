Imports Microsoft.VisualBasic.Data.Framework.StorageProvider
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.CellPhenotype
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module grn_demo2

    Sub Main()
        Dim data = "K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv"
        Dim wgcna = NetworkFileIO.Load("K:\hsa_grn").CreateGraph
        Dim TF = DataFrameResolver.Load("K:\hsa_grn\Homo_sapiens_TF.txt", tsv:=True)
        Dim TFlist As String() = TF("Ensembl")
        Dim grn = GeneRegulatoryNetwork.BuildBNNetwork(wgcna, TFlist)


        Dim exprData = BnIO.ReadGeneExpressionMatrix(Matrix.LoadData(data, tqdm_wrap:=True))

        Pause()
    End Sub
End Module
