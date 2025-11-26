#Region "Microsoft.VisualBasic::8b5b694d1cedeec045f40007d3aea205, engine\Compiler\MarkupCompiler\BioCyc\v2Compiler.vb"

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

    '   Total Lines: 211
    '    Code Lines: 180 (85.31%)
    ' Comment Lines: 3 (1.42%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 28 (13.27%)
    '     File Size: 8.62 KB


    '     Class v2Compiler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CompileImpl, createCompounds, createEnzyme, createKinetics, createReactions
    '                   enzymaticReaction, nonEnzymaticReaction, PreCompile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices
Imports SMRUCC.genomics.Metagenomics

Namespace MarkupCompiler.BioCyc

    Public Class v2Compiler : Inherits Compiler(Of VirtualCell)

        Friend ReadOnly biocyc As Workspace
        Friend ReadOnly cellular_id As String

        ReadOnly ccmap As New Dictionary(Of String, String)

        Sub New(biocyc As Workspace)
            Me.biocyc = biocyc
            Me.cellular_id = biocyc.species.uniqueId
        End Sub

        Public Function MapCompartId(id As String) As String
            If id Is Nothing OrElse id.StringEmpty(, True) Then
                Return cellular_id
            End If

            Return ccmap.ComputeIfAbsent(id, lazyValue:=Function() MapInternal(id))
        End Function

        Public Function MapInternal(id As String) As String
            Select Case id
                Case "CCO-OUT", "CCO-OUTER-MEM", "CCO-MEMBRANE", "CCO-EXTRACELLULAR-CCO-CYTOSOL", "CCO-EXTRACELLULAR"
                    Return "Extracellular"
                Case Else
                    Return cellular_id
            End Select
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function MapCompartId(id As String()) As String()
            Return id.SafeQuery.Select(AddressOf MapCompartId).ToArray
        End Function


        Protected Overrides Function PreCompile(args As CommandLine) As Integer
            Dim info As New StringBuilder
            Dim spec = biocyc.species

            Using writer As New StringWriter(info)
                Call CLITools.AppSummary(GetType(v2Compiler).Assembly.FromAssembly, "", "", writer)
            End Using

            m_compiledModel = New VirtualCell With {
                .taxonomy = New Taxonomy With {
                    .scientificName = If(spec.synonyms.FirstOrDefault, spec.commonName),
                    .ncbi_taxid = spec.NCBITaxonomyId,
                    .species = spec.commonName
                },
                .cellular_id = spec.uniqueId,
                .properties = New [Property] With {
                    .authors = spec.PGDBAuthors,
                    .comment = spec.comment,
                    .compiled = Now,
                    .name = spec.commonName,
                    .specieId = spec.NCBITaxonomyId,
                    .title = If(spec.synonyms.FirstOrDefault, spec.commonName)
                },
                .genome = New Genome
            }
            m_logging.WriteLine(info.ToString)

            Return 0
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
            Dim replicon As replicon = New GenomeCompiler(Me).CreateReplicon
            Dim reg As New RegulationCompiler(biocyc)

            m_compiledModel.metabolismStructure = New ReactionNetworkCompiler(Me).BuildModel
            m_compiledModel.genome = New Genome With {
                .replicons = {replicon},
                .proteins = New ProteinCompiler(biocyc) _
                    .CreateProteins _
                    .ToArray,
                .regulations = reg _
                    .CreateRegulations(.replicons.Select(Function(r) r.operons).IteratesALL) _
                    .ToArray
            }

            Dim usedIndex As Index(Of String) = m_compiledModel _
                .metabolismStructure _
                .compounds _
                .Select(Function(c) c.ID) _
                .Distinct _
                .Indexing

            ' join enzyme kinetics compounds
            Dim missing = m_compiledModel.metabolismStructure.enzymes _
                .Select(Function(enz) enz.catalysis) _
                .IteratesALL _
                .Select(Function(cat) cat.parameter) _
                .IteratesALL _
                .Select(Function(par) par.target) _
                .Where(Function(id) Not id.StringEmpty) _
                .Distinct _
                .Where(Function(id) Not id Like usedIndex) _
                .Select(Function(id)
                            Return New Compound With {
                                .ID = id,
                                .db_xrefs = Nothing,
                                .name = id
                            }
                        End Function) _
                .ToArray

            m_compiledModel.metabolismStructure.compounds = m_compiledModel _
                .metabolismStructure _
                .compounds _
                .JoinIterates(missing) _
                .ToArray

            Return 0
        End Function
    End Class
End Namespace
