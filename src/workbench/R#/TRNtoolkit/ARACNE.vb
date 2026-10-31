Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Model.Network.Regulons.ARACNE

<Package("ARACNE")>
Public Module ARACNETools

    <ExportAPI("aracne_options")>
    Public Function AracneOptions() As AracneOptions
        Return New AracneOptions
    End Function

    <ExportAPI("infer")>
    Public Function Infer(x As Matrix, Optional opts As AracneOptions = Nothing) As AracneNetwork
        Return x.InferAracneNetwork(If(opts, New AracneOptions))
    End Function

    <ExportAPI("write_edges")>
    Public Sub write_edges(net As AracneNetwork, file As String)
        Call net.WriteEdges(file)
    End Sub

    <ExportAPI("coef")>
    Public Function coef(net As AracneNetwork) As Matrix
        Return net.CoefMatrix
    End Function
End Module
