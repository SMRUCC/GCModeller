
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

<Package("geneExpression")>
Module geneExpression

    ''' <summary>
    ''' load an expressin matrix data
    ''' </summary>
    ''' <param name="file$"></param>
    ''' <param name="exclude_samples"></param>
    ''' <returns></returns>
    <ExportAPI("load.expr")>
    Public Function loadExpression(file$, Optional exclude_samples As String() = Nothing) As DataFrameRow()
        Return Matrix.LoadData(file, If(exclude_samples Is Nothing, Nothing, New Index(Of String)(exclude_samples))).expression
    End Function
End Module
