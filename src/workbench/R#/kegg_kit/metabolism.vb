#Region "Microsoft.VisualBasic::3c3174b9215a20e7916eb45a4ec0270a, R#\kegg_kit\metabolism.vb"

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

    '   Total Lines: 156
    '    Code Lines: 90 (57.69%)
    ' Comment Lines: 45 (28.85%)
    '    - Xml Docs: 82.22%
    ' 
    '   Blank Lines: 21 (13.46%)
    '     File Size: 5.78 KB


    ' Module metabolism
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: filterInvalidCompoundIds, GetAllCompounds, KEGGReconstruction, loadReactionCacheIndex, PickNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' The kegg metabolism model toolkit
''' </summary>
<Package("metabolism", Category:=APICategories.ResearchTools)>
Module metabolism

    Sub New()

    End Sub

    <ExportAPI("load.reaction.cacheIndex")>
    Public Function loadReactionCacheIndex(file As String) As MapCache
        Return MapCache.ParseText(file.SolveStream.LineTokens)
    End Function

    ''' <summary>
    ''' Get compounds kegg id which is related to the given KO id list
    ''' </summary>
    ''' <param name="enzymes">KO id list</param>
    ''' <param name="reactions"></param>
    ''' <returns></returns>
    <ExportAPI("related.compounds")>
    Public Function GetAllCompounds(enzymes$(), reactions As ReactionRepository) As String()
        Return reactions _
            .GetByKOMatch(KO:=enzymes) _
            .Select(Function(r) r.GetSubstrateCompounds) _
            .IteratesALL _
            .Distinct _
            .ToArray
    End Function

    '<ExportAPI("compound.origins")>
    'Public Function CreateCompoundOriginModel(repo As String, Optional compoundNames As Dictionary(Of String, String) = Nothing) As OrganismCompounds
    '    If compoundNames Is Nothing Then
    '        Return OrganismCompounds.LoadData(repo)
    '    Else
    '        Return OrganismCompounds.LoadData(repo, compoundNames)
    '    End If
    'End Function

    ''' <summary>
    ''' Removes invalid kegg compound id
    ''' </summary>
    ''' <param name="identified"></param>
    ''' <returns></returns>
    <ExportAPI("filter.invalid_keggIds")>
    Public Function filterInvalidCompoundIds(identified As String()) As String()
        Return identified _
            .Where(Function(id)
                       Return id.IsPattern(KEGGCompoundIDPatterns)
                   End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' do kegg pathway reconstruction by given protein annotation data
    ''' </summary>
    ''' <param name="reference">
    ''' the kegg reference maps
    ''' </param>
    ''' <param name="reactions">
    ''' a list of the kegg reaction data models
    ''' </param>
    ''' <param name="annotations">the <see cref="ProteinAnnotation"/> data stream with kegg ontology('ko' attribute) id.</param>
    ''' <param name="min_cov">coverage cutoff of the ratio of annotation protein hit against the all proteins on the pathway map</param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' A set of the kegg pathway object that contains with the KEGG id mapping(protein id mapping and assigned compound id list)
    ''' </returns>
    <ExportAPI("kegg_reconstruction")>
    <RApiReturn(GetType(DBGET.bGetObject.Pathway))>
    Public Function KEGGReconstruction(<RRawVectorArgument> reference As Object,
                                       <RRawVectorArgument> reactions As Object,
                                       <RRawVectorArgument> annotations As Object,
                                       Optional min_cov As Double = 0.3,
                                       Optional env As Environment = Nothing) As pipeline

        Dim rxnList As pipeline = pipeline.TryCreatePipeline(Of ReactionTable)(reactions, env)

        If rxnList.isError Then
            Return rxnList
        End If

        Dim maps As pipeline = pipeline.TryCreatePipeline(Of Map)(reference, env)

        If maps.isError Then
            Return maps
        End If

        Dim proteins As pipeline = pipeline.TryCreatePipeline(Of ProteinAnnotation)(annotations, env)

        If proteins.isError Then
            Return proteins
        End If

        Dim rxnIndex = rxnList.populates(Of ReactionTable)(env).CreateIndex
        Dim genes As ProteinAnnotation() = proteins.populates(Of ProteinAnnotation)(env).ToArray

        Return maps _
            .populates(Of Map)(env) _
            .KEGGReconstruction(genes, min_cov) _
            .Select(Function(pathway)
                        Return pathway.AssignCompounds(rxnIndex)
                    End Function) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' pick the reaction list from the kegg reaction
    ''' network repository by KO id terms
    ''' </summary>
    ''' <param name="reactions"></param>
    ''' <param name="terms">
    ''' the KO id terms
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("pickNetwork")>
    Public Function PickNetwork(reactions As ReactionRepository,
                                <RRawVectorArgument>
                                terms As Object,
                                Optional env As Environment = Nothing) As Object

        Dim KoIdlist As String()
        Dim stream As pipeline = pipeline.TryCreatePipeline(Of String)(terms, env, suppress:=True)

        If stream.isError Then
            stream = pipeline.TryCreatePipeline(Of BiDirectionalBesthit)(terms, env)

            If stream.isError Then
                Return stream.getError
            Else
                KoIdlist = stream _
                    .populates(Of BiDirectionalBesthit)(env) _
                    .Select(Function(hit) hit.term) _
                    .Distinct _
                    .ToArray
            End If
        Else
            KoIdlist = stream.populates(Of String)(env).ToArray
        End If

        Return reactions.GetByKOMatch(KoIdlist).ToArray
    End Function
End Module
