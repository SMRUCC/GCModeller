'Imports System.Drawing
'Imports LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject
'Imports LANS.SystemsBiology.Assembly.SBML
'Imports LANS.SystemsBiology.Assembly.SBML.Level2
'Imports LANS.SystemsBiology.GCModeller.AnalysisTools.ModelSolvers.FBA.Models.rFBA
'Imports Microsoft.VisualBasic.DocumentFormat.Csv

'Public Module FluxVisual

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="coefficient"><see cref="RPKMStat"/> Csv文件的路径</param>
'    ''' <param name="model"></param>
'    ''' <returns></returns>
'    ''' <param name="mods">KEGG Modules DIR</param>
'    Public Function DrawingModule(coefficient As String, model As XmlFile, mods As String) As Image
'        Dim MAT As DocumentStream.File = DocumentStream.File.FastLoad(coefficient) ' 由于文件可能比较大，直接使用反射加载可能比较慢，由于id号之中没有逗号，所以直接使用fastLoad加载数据
'        Dim source As RPKMStat() = MAT.AsDataSource(Of RPKMStat)(False)
'        Dim modules = (From file As String
'                       In FileIO.FileSystem.GetFiles(mods, FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
'                       Select file.LoadXml(Of [Module])).ToArray
'        Dim kgRxns = ExportServices.LoadReactions(GCModeller.FileSystem.KEGG.GetReactions)

'    End Function
'End Module
