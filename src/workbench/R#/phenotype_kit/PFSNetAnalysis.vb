Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.PFSNet
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("PFSNet", Category:=APICategories.ResearchTools)>
Module PFSNetAnalysis

    <ExportAPI("load.expr")>
    Public Function loadExpression(file As String) As DataFrameRow()
        Return Matrix.LoadData(file).expression
    End Function

    <ExportAPI("load.pathway_network")>
    Public Function loadPathwayNetwork(file As String) As GraphEdge()
        Return GraphEdge.LoadData(file)
    End Function

    ''' <summary>
    ''' Finding consistent disease subnetworks using PFSNet
    ''' </summary>
    ''' <param name="expr1o"></param>
    ''' <param name="expr2o"></param>
    ''' <param name="ggi"></param>
    ''' <param name="b"></param>
    ''' <param name="t1"></param>
    ''' <param name="t2"></param>
    ''' <param name="n"></param>
    ''' <returns></returns>
    <ExportAPI("pfsnet")>
    Public Function pfsnet(expr1o As DataFrameRow(), expr2o As DataFrameRow(), ggi As GraphEdge(),
                           Optional b# = 0.5,
                           Optional t1# = 0.95,
                           Optional t2# = 0.85,
                           Optional n% = 1000) As PFSNetResultOut

        Return PFSNetAlgorithm.pfsnet(expr1o, expr2o, ggi, b, t1, t2, n)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="format">xml/json</param>
    ''' <returns></returns>
    <ExportAPI("read.pfsnet_result")>
    <RApiReturn(GetType(PFSNetResultOut))>
    Public Function readPFSNetOutput(file As String, Optional format As FileFormats = FileFormats.xml, Optional env As Environment = Nothing) As Object
        If Not file.FileExists Then
            Return Internal.debug.stop("the given file is not exists on your file system!", env)
        ElseIf format <> FileFormats.json AndAlso format <> FileFormats.xml Then
            Return Internal.debug.stop("the file format flag value is not supported at this api...", env)
        End If

        If format = FileFormats.xml Then
            Return file.LoadXml(Of PFSNetResultOut)
        Else
            Return file.LoadJsonFile(Of PFSNetResultOut)
        End If
    End Function
End Module
