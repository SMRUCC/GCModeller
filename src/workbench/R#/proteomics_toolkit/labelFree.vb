
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.Proteomics.FoldChangeMatrix

<Package("proteomics.labelfree")>
Module labelFree

    <ExportAPI("sample.normalize")>
    Public Function totalSumNormalize(data As DataSet(), Optional byMedianQuantile As Boolean = False, Optional samples As String() = Nothing) As DataSet()
        Return data.TotalSumNormalize(byMedianQuantile, samples).ToArray
    End Function
End Module
