Imports LANS.SystemsBiology.Assembly.KEGG.DBGET
Imports LANS.SystemsBiology.Assembly.SBML
Imports LANS.SystemsBiology.Assembly.SBML.ExportServices.KEGG
Imports LANS.SystemsBiology.ComponentModel.EquaionModel.DefaultTypes
Imports LANS.SystemsBiology.GCModeller.AnalysisTools.ModelSolvers.FBA.Models.rFBA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace("FBA.CLI", Category:=APICategories.CLI_MAN)>
Public Module CLI

    Sub New()
        Call Settings.Session.Initialize(GetType(CLI))

        Try
            Dim template As String = App.HOME & "/Templates/rFBA.ModifierTemplates.Csv"
            If Not template.FileExists Then
                Dim modifyTemplates As Modifier() = {
                    New Modifier With {.locus = "Gene LocusId 1", .modify = 0, .Comments = "Deletion mutation"},
                    New Modifier With {.locus = "Gene LocusId 2", .modify = 0.3, .Comments = "Low expression (0, 1)"},
                    New Modifier With {.locus = "Gene LocusId 3", .modify = 1, .Comments = "No modify"},
                    New Modifier With {.locus = "Gene LocusId 4", .modify = 2, .Comments = "OverExpression"}
                }
                Call modifyTemplates.SaveTo(template)
            End If

            Call New rFBA_ARGVS().SaveAsXml(Settings.Session.Templates & "/rFBA.Parameters.Xml")
        Catch ex As Exception
            ' 不影响使用，直接无视这个错误
        End Try
    End Sub

    <ExportAPI("/Export", Info:="", Usage:="export -i <fba_model> -o <r_script>", Example:="export -i /home/xieguigang/ecoli.xml -o /home/xieguigang/ecoli.r")>
    Public Function Export(args As CommandLine.CommandLine) As Integer
        'Dim FBA As LANS.SystemsBiology.ModelSolvers.FBA.FBA_RScript_Builder = CommandLine("-i").LoadXml(Of LANS.SystemsBiology.ModelSolvers.FBA.FBA_RScript_Builder)()
        'Call FileIO.FileSystem.WriteAllText(CommandLine("-o"), FBA.RScript, append:=False, encoding:=System.Text.Encoding.ASCII)
        'Return 0
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 向数据库之中导入没有的反应过程的数据记录
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Imports", Usage:="/Imports /in <sbml.xml>")>
    Public Function ImportsRxns(args As CommandLine.CommandLine) As Integer
        Dim inXml As String = args("/in")
        Dim readSource As String = GCModeller.FileSystem.KEGG.GetReactions
        Dim outDIR As String = readSource & "/Imports/"
        Dim files = FileIO.FileSystem.GetFiles(readSource, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
        Dim exists = (From file As String
                      In files.AsParallel
                      Select file.LoadXml(Of bGetObject.Reaction).ReactionModel).ToArray
        Dim sbml As Level2.XmlFile = Level2.XmlFile.Load(inXml)
        Dim pendings = sbml.GetReactions.ToDictionary(Function(x) x, Function(x) x.ReactionModel)
        Dim LQuery = (From x As KeyValuePair(Of bGetObject.Reaction, Equation)
                      In pendings.AsParallel
                      Let isExists As Boolean = Not (From xx As Equation
                                                     In exists
                                                     Where xx.Equals(x.Value, True)
                                                     Select xx).FirstOrDefault Is Nothing
                      Where Not isExists
                      Select x).ToArray
        For Each query In LQuery
            Dim out As String = outDIR & "/" & query.Key.Entry.NormalizePathString(False) & ".xml"
            Call query.Key.SaveAsXml(out)
        Next

        Return True
    End Function
End Module
