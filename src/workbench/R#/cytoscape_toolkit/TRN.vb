Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Model.Network.Regulons

''' <summary>
''' Transcription Regulation Network Builder Tools
''' </summary>
''' 
<Package("network.TRN")>
Module TRN

    <ExportAPI("fpkm.connections")>
    Public Function ExpressionConnections(fpkm As DataSet(), Optional cutoff# = 0.65) As Connection()
        Return fpkm.CorrelationNetwork(cutoff).ToArray
    End Function
End Module
