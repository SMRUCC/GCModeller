#Region "Microsoft.VisualBasic::ebbeb9829a2e2d1b430cc4c050783431, GCModeller\engine\Compiler\MarkupCompiler\KEGG\v2KEGGCompiler.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 131
    '    Code Lines: 103
    ' Comment Lines: 13
    '   Blank Lines: 15
    '     File Size: 5.43 KB


    '     Class v2KEGGCompiler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CompileImpl, PreCompile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports XmlReaction = SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2.Reaction

Namespace MarkupCompiler

    Public Class v2KEGGCompiler : Inherits Compiler(Of VirtualCell)

        ReadOnly genomes As Dictionary(Of String, GBFF.File)

        Friend ReadOnly KEGG As RepositoryArguments
        Friend ReadOnly regulations As RegulationFootprint()
        Friend ReadOnly model As CellularModule
        Friend ReadOnly locationAsLocus_tag As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="model">中间模型数据</param>
        ''' <param name="genomes"></param>
        ''' <param name="KEGG"></param>
        ''' <param name="regulations">
        ''' 所有的复制子的调控网络应该都是合并在一起通过这个参数传递进来了
        ''' </param>
        ''' <param name="locationAsLocus_tag"></param>
        Sub New(model As CellularModule,
                genomes As Dictionary(Of String, GBFF.File),
                KEGG As RepositoryArguments,
                regulations As RegulationFootprint(),
                locationAsLocus_tag As Boolean)

            Me.model = model
            Me.genomes = genomes
            Me.KEGG = KEGG
            Me.regulations = regulations
            Me.locationAsLocus_tag = locationAsLocus_tag
        End Sub

        Protected Overrides Function PreCompile(args As CommandLine) As Integer
            Dim info As New StringBuilder

            Using writer As New StringWriter(info)
                Call CLITools.AppSummary(GetType(v2KEGGCompiler).Assembly.FromAssembly, "", "", writer)
            End Using

            m_compiledModel = New VirtualCell With {
                .taxonomy = model.Taxonomy
            }
            m_logging.WriteLine(info.ToString)

            Return 0
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
            Dim KOgenes As Dictionary(Of String, CentralDogma) = model _
                .Genotype _
                .centralDogmas _
                .Where(Function(process)
                           Return Not process.IsRNAGene AndAlso Not process.orthology.StringEmpty
                       End Function) _
                .ToDictionary(Function(term)
                                  Return term.geneID
                              End Function)
            ' Dim metabolism As New CompileMetabolismWorkflow(Me)
            Dim metabolism As CompileMetabolismWorkflow
            Dim enzymes As Enzyme() = model.Regulations _
                .Where(Function(process)
                           Return process.type = Processes.MetabolicProcess
                       End Function) _
                .DoCall(Function(cat) metabolism.createEnzymes(cat, KOgenes)) _
                .ToArray
            Dim KOfunc As Dictionary(Of String, CentralDogma()) = KOgenes.Values _
                .GroupBy(Function(proc) proc.orthology) _
                .ToDictionary(Function(KO) KO.Key,
                              Function(g)
                                  Return g.ToArray
                              End Function)
            'Dim genomeCompiler As New CompileGenomeWorkflow(Me)
            'Dim TRNCompiler As New CompileTRNWorkflow(Me)
            Dim genomeCompiler As CompileGenomeWorkflow
            Dim TRNCompiler As CompileTRNWorkflow

            m_compiledModel.genome = New Genome With {
                .replicons = genomeCompiler _
                    .populateReplicons(genomes) _
                    .ToArray,
                .regulations = TRNCompiler _
                    .getTFregulations() _
                    .ToArray
            }

            Call CompileLogging.WriteLine("create metabolism network")

            m_compiledModel.metabolismStructure = New MetabolismStructure With {
                .reactions = model _
                    .Phenotype _
                    .fluxes _
                    .Select(Function(r)
                                Return New XmlReaction With {
                                    .ID = r.ID,
                                    .name = r.name,
                                    .Equation = r.GetEquationString,
                                    .is_enzymatic = r.is_enzymatic
                                }
                            End Function) _
                    .ToArray,
                .enzymes = enzymes,
                .compounds = .reactions _
                    .AsEnumerable _
                    .DoCall(AddressOf metabolism.getCompounds) _
                    .ToArray,
                .maps = KEGG.GetPathways _
                    .PathwayMaps _
                    .DoCall(Function(maps) metabolism.createMaps(maps, KOfunc)) _
                    .ToArray
            }

            Return 0
        End Function
    End Class
End Namespace
