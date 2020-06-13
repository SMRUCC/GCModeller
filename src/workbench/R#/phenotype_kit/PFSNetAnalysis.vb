Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.PFSNet
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure

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
End Module
