#Region "Microsoft.VisualBasic::9074b4dfdcbea67825f0f226254f5d3c, ..\GCModeller\data\ExternalDBSource\string-db\StrPNet\LDM\API.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.SBML.Level2.Elements
Imports SMRUCC.genomics.DatabaseServices.StringDB.StrPNet.TCS
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic

Namespace StringDB.StrPNet

    <PackageNamespace("StrPNet.API", Description:="Bacterial signal transduction network based on the string-db")>
    Public Module API

        <ExportAPI("Extract.Network"), Extension>
        Public Function ExportNetwork(Pathway As Pathway, TFRegulations As TFRegulation(), Inducers As SensorInducers()) As NetworkEdge()
            Dim RegulatedObjects = TFRegulations.GetItems(Pathway.TF)

            Dim ChunkList As List(Of Microsoft.VisualBasic.DataVisualization.Network.FileStream.NetworkEdge) = New List(Of DataVisualization.Network.FileStream.NetworkEdge)

            If Not Pathway.OCS.IsNullOrEmpty Then
                For Each OCS_Pathway In Pathway.OCS
                    Call ChunkList.AddRange(ChemotaxisInduction(OCS_Pathway.Key, Inducers.GetItem(OCS_Pathway.Key).Inducers))
                    Call ChunkList.Add(TFBinding(OCS_Pathway.Key, Pathway.TF, OCS_Pathway.Value))
                Next
            End If
            If Not Pathway.TCSSystem.IsNullOrEmpty Then
                For Each TCS_Pathway In Pathway.TCSSystem
                    Call ChunkList.AddRange(ChemotaxisInduction(TCS_Pathway.Chemotaxis, Inducers.GetItem(TCS_Pathway.Chemotaxis).Inducers))
                    Call ChunkList.Add(PhosphoTransfer(TCS_Pathway.Chemotaxis, TCS_Pathway.HK, TCS_Pathway.ChemotaxisHKConfidence))
                    Call ChunkList.Add(PhosphoTransfer(TCS_Pathway.HK, TCS_Pathway.RR, TCS_Pathway.HKRRConfidence))
                    Call ChunkList.Add(TFBinding(TCS_Pathway.RR, Pathway.TF, TCS_Pathway.RRTFConfidence))
                Next
            End If

            Call ChunkList.AddRange(Regulation(RegulatedObjects))

            Return ChunkList.ToArray
        End Function

        Private Function TFBinding(RR As String, TF As String, Confidence As Double) As DataVisualization.Network.FileStream.NetworkEdge
            Return New DataVisualization.Network.FileStream.NetworkEdge With {.FromNode = RR, .ToNode = TF, .InteractionType = "ProteinComplexAssembly", .Confidence = Confidence}
        End Function

        Private Function ChemotaxisInduction(Sensor As String, Inducers As String()) As DataVisualization.Network.FileStream.NetworkEdge()
            Dim LQuery = (From Inducer As String In Inducers Select New DataVisualization.Network.FileStream.NetworkEdge With {.FromNode = Inducer, .ToNode = Sensor, .InteractionType = "ChemotaxisInduction", .Confidence = 1}).ToArray
            Return LQuery
        End Function

        Private Function PhosphoTransfer(Donor As String, Reciever As String, Confidence As Double) As DataVisualization.Network.FileStream.NetworkEdge
            Return New DataVisualization.Network.FileStream.NetworkEdge With {.FromNode = Donor, .ToNode = Reciever, .InteractionType = "PhosphoTransfer", .Confidence = Confidence}
        End Function

        Private Function Regulation(TFRegulations As TFRegulation()) As DataVisualization.Network.FileStream.NetworkEdge()
            Dim ChunkList As List(Of DataVisualization.Network.FileStream.NetworkEdge) = New List(Of DataVisualization.Network.FileStream.NetworkEdge)
            For Each TFRegulation As TFRegulation In TFRegulations
                Dim RegulationEffect As String = If(TFRegulation.TFPcc > 0, "Activation", "Repression")
                Call ChunkList.AddRange((From GeneId As String
                                         In TFRegulation.OperonGenes
                                         Select New DataVisualization.Network.FileStream.NetworkEdge With {
                                             .FromNode = TFRegulation.Regulator,
                                             .ToNode = GeneId,
                                             .InteractionType = RegulationEffect,
                                             .Confidence = TFRegulation.TFPcc}).ToArray)
            Next

            Return ChunkList.ToArray
        End Function

        ''' <summary>
        ''' 能够激活<see cref="TCS.TCS.Chemotaxis">趋化性蛋白</see>以及<see cref="Pathway.OCS">单组份系统</see>的诱导物
        ''' </summary>
        ''' <param name="Inducers"></param>
        ''' <param name="Pi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Extract.Flux"), Extension>
        Public Function CreateFluxObject(Network As Network, Inducers As SensorInducers(), Optional Pi As String = "PI") As Global.SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction()
            Dim ChunkList As List(Of SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction) = New List(Of SystemsBiology.Assembly.SBML.Level2.Elements.Reaction)

            For Each item In Network.Pathway
                If Not item.OCS.IsNullOrEmpty Then
                    For Each OCS_Pathway In item.OCS
                        Call ChunkList.AddRange(CreateFluxObject(item.TF, OCS_Pathway, Inducers.GetItem(OCS_Pathway.Key).Inducers, Pi))
                    Next
                End If
                If Not item.TCSSystem.IsNullOrEmpty Then
                    For Each TCS_Pathway In item.TCSSystem
                        Call ChunkList.AddRange(CreateFluxObject(item.TF, TCS_Pathway, Inducers.GetItem(TCS_Pathway.Chemotaxis).Inducers, Pi))
                    Next
                End If
            Next

            Return ChunkList.ToArray
        End Function

        Private Function CreateFluxObject(TF As String, TCS As TCS.TCS, Inducers As String(), Pi As String) As Global.SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction()
            Dim ChunkBuffer As List(Of SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction) = (From Inducer As String In Inducers Select ChemotaxisInduction(TCS.Chemotaxis, Pi, Inducer)).ToList
            Call ChunkBuffer.Add(PhosphoTransfer(TCS.Chemotaxis, TCS.HK, Pi))
            Call ChunkBuffer.Add(PhosphoTransfer(TCS.HK, TCS.RR, Pi))
            Call ChunkBuffer.Add(PhosphoTransfer(TCS.RR, TF, Pi))

            Return ChunkBuffer.ToArray
        End Function

        Private Function CreateFluxObject(TF As String, OCS As KeyValuePair, Inducers As String(), Pi As String) As SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction()
            Dim ChunkBuffer As List(Of SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction) = (From Inducer As String In Inducers Select ChemotaxisInduction(OCS.Key, Pi, Inducer)).ToList
            ' Call ChunkBuffer.Add(PhosphoTransfer(OCS.Chemotaxis, OCS.TrNode, Pi))
            'Call ChunkBuffer.Add(PhosphoTransfer(OCS.TrNode, TF, Pi))

            Return ChunkBuffer.ToArray
        End Function

        Private Function ChemotaxisInduction(Chemotaxis As String, Pi As String, Inducer As String) As SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction
            Dim Reactants = {
                        New speciesReference With {.species = Inducer, .stoichiometry = 1},
                        New speciesReference With {.species = "ATP", .stoichiometry = 1},
                        New speciesReference With {.species = Chemotaxis, .stoichiometry = 1}}
            Dim Products = {
                        New speciesReference With {.species = Inducer, .stoichiometry = 1},
                        New speciesReference With {.species = "ADP", .stoichiometry = 1},
                        New speciesReference With {.species = String.Format("[{0}][{1}]", Chemotaxis, Pi), .stoichiometry = 1}}

            Return New Reaction With {
                .reversible = False,
                .Reactants = Reactants,
                .Products = Products,
                .id = String.Format("[{0}][{1}]", Inducer, Chemotaxis)
            }
        End Function

        Private Function PhosphoTransfer(Donor As String, Reciever As String, Pi As String) As SMRUCC.genomics.Assembly.SBML.Level2.Elements.Reaction
            Dim Reaction = New Reaction With {
                .reversible = False,
                                                                                .Products = {
                                                                                    New speciesReference With {.species = String.Format("[{0}][{1}]", Reciever, Pi), .stoichiometry = 1},
                                                                                    New speciesReference With {.species = Donor, .stoichiometry = 1}},
                                                                                .Reactants = {
                                                                                    New speciesReference With {.stoichiometry = 1, .species = String.Format("[{0}][{1}]", Donor, Pi)},
                                                                                    New speciesReference With {.stoichiometry = 1, .species = Reciever}},
                                                                                .id = String.Format("{0}|{1}-PHOSPHO-TRANSFER", Donor, Reciever)}
            Return Reaction
        End Function
    End Module
End Namespace
