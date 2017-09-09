#Region "Microsoft.VisualBasic::c2ecea9a6986e39735d0a2a3870055d9, ..\CLI_tools\Solver.FBA\CLI\CLI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.ExportServices.KEGG

<Package("FBA.CLI", Category:=APICategories.CLI_MAN)>
<CLI> Public Module CLI

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
    Public Function Export(args As CommandLine) As Integer
        'Dim FBA As SMRUCC.genomics.ModelSolvers.FBA.FBA_RScript_Builder = CommandLine("-i").LoadXml(Of SMRUCC.genomics.ModelSolvers.FBA.FBA_RScript_Builder)()
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
    Public Function ImportsRxns(args As CommandLine) As Integer
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
