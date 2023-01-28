Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Public Module ArabidopsisThalianaTest

    Dim base As String = "E:\GCModeller\src\workbench\R#\demo\HTS\GSVA"

    Sub Main()
        Dim expr As Matrix = Matrix.LoadData($"{base}/ath_norm.csv")
        Dim kegg As New Background With {
            .clusters = LoadKEGG.ToArray
        }
    End Sub

    Private Iterator Function LoadKEGG() As IEnumerable(Of Cluster)
        Using file = $"{base}/ath.db".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Using pack As New StreamPack(file, [readonly]:=True)



            End Using
        End Using
    End Function
End Module
