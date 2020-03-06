Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Regprecise

<Package("TRN.builder")>
Module TRNBuilder

    <ExportAPI("read.regprecise")>
    Public Function readRegPrecise(file As String) As TranscriptionFactors
        Return file.LoadXml(Of TranscriptionFactors)
    End Function
End Module
