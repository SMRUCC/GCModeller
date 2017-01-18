#Region "Microsoft.VisualBasic::421a3b7fde7595736cc2f670740fb2da, ..\GCModeller\engine\GCTabular\Compiler\SignalTransductionNetwork\PartsAPI.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.Model.Network.STRING.TCS

Namespace Compiler.Components

    Public Module PartsAPI

        Public Function PhosphoTransfer_(Donor As String, Reciever As String, Confidence As Double, Pi As String) As FileStream.MetabolismFlux
            Dim Equation As String = SMRUCC.genomics.ComponentModel.EquaionModel.EquationBuilder.ToString(
                                           New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, String.Format("[{0}][PI]", Donor)), New KeyValuePair(Of Double, String)(1, Reciever)},
                                           New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, Donor), New KeyValuePair(Of Double, String)(1, String.Format("[{0}][PI]", Reciever))}, False)
            Dim Reaction = New FileStream.MetabolismFlux With
                           {
                               .Equation = Equation,
                               .p_Dynamics_K_1 = Confidence,
                               ._Internal_compilerRight = New KeyValuePair(Of Double, String)() {
                                   New KeyValuePair(Of Double, String)(1, String.Format("[{0}][{1}]", Reciever, Pi)),
                                   New KeyValuePair(Of Double, String)(1, Donor)},
                               ._Internal_compilerLeft = New KeyValuePair(Of Double, String)() {
                                   New KeyValuePair(Of Double, String)(1, String.Format("[{0}][{1}]", Donor, Pi)),
                                   New KeyValuePair(Of Double, String)(1, Reciever)},
                               .Identifier = String.Format("{0}|{1}-PHOSPHO-TRANSFER", Donor, Reciever),
                               .LOWER_Bound = 0,
                               .UPPER_Bound = 1000}
            Return Reaction
        End Function

        Public Function ProteinComplexAssemble(ComponentA As String, ComponentB As String, Confidence As Double) As FileStream.MetabolismFlux
            Dim Equation As String = ComponentModel.EquaionModel.EquationBuilder.ToString(New KeyValuePair(Of Double, String)() {
                                                                                          New KeyValuePair(Of Double, String)(1, ComponentA),
                                                                                          New KeyValuePair(Of Double, String)(1, ComponentB)},
                                                                                          New KeyValuePair(Of Double, String)() {New KeyValuePair(Of Double, String)(1, String.Format("[{0}][{1}]", ComponentA, ComponentB))}, True)
            Dim UniqueId As String = String.Format("[{0}]+", ComponentA)
            Dim DBLink = New String() {New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "SignalTransductionNetwork.ProteinComplexAssemble", .AccessionId = UniqueId}.GetFormatValue}
            Dim DataModel As FileStream.MetabolismFlux = New FileStream.MetabolismFlux With {.Identifier = UniqueId,
                                                                                             .p_Dynamics_K_1 = Confidence, .UPPER_Bound = 100, .LOWER_Bound = -100, .p_Dynamics_K_2 = Confidence, .Equation = Equation} ' .DBLinks = DBLink,
            Return DataModel
        End Function

        Public Function CreateFluxObject(STrpProfile As SMRUCC.genomics.Model.Network.[STRING].Network, Inducers As SMRUCC.genomics.Model.Network.[STRING].TCS.SensorInducers(), Optional Pi As String = "PI") _
      As GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly()

            Dim ChunkList As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly) =
                New List(Of GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly)

            For Each item In STrpProfile.Pathway
                If Not item.OCS.IsNullOrEmpty Then
                    For Each Pathway In item.OCS
                        Call ChunkList.AddRange(CreateFluxObject(item.TF, Pathway, Inducers.GetItem(Pathway.Key).Inducers, Pi))
                    Next
                End If
                If Not item.TCSSystem.IsNullOrEmpty Then
                    For Each Pathway In item.TCSSystem
                        Call ChunkList.AddRange(CreateFluxObject(item.TF, Pathway, Inducers.GetItem(Pathway.Chemotaxis).Inducers, Pi))
                    Next
                End If
            Next

            Dim Index = (From item In ChunkList Select item.Identifier Distinct).ToArray
            ChunkList = (From Id As String In Index Select ChunkList.GetItem(Id)).ToList

            Dim SelfLoop = (From item In ChunkList
                            Let Metabolite As String() = (From [sub] In item.Metabolites Select [sub].species Distinct Order By Len(species) Ascending).ToArray
                            Let Shortest = Metabolite.First Let Longest = Metabolite.Last
                            Where Metabolite.Count = 2 AndAlso String.Equals(Longest, String.Format("[{0}][PI]", Shortest)) Select item).ToArray
            For Each Item In SelfLoop
                Call ChunkList.Remove(Item)
            Next

            Return ChunkList.ToArray
        End Function

        Private Function CreateFluxObject(TF As String,
                                                 TCS As TCS,
                                                 Inducers As String(),
                                                 Pi As String) _
            As GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly()

            Dim ChunkBuffer As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly) =
                (From Inducer As String
                 In Inducers
                 Select ChemotaxisInduction(TCS.Chemotaxis, Pi, Inducer)).ToList

            Call ChunkBuffer.Add(PhosphoTransfer(TCS.Chemotaxis, TCS.HK, TCS.ChemotaxisHKConfidence, Pi))
            Dim TCSCrossTalk = PhosphoTransfer(TCS.HK, TCS.RR, TCS.HKRRConfidence, Pi)
            TCSCrossTalk.TCSCrossTalk = True
            Call ChunkBuffer.Add(TCSCrossTalk)
            Call ChunkBuffer.Add(PhosphoTransfer(TCS.RR, TF, TCS.RRTFConfidence, Pi))

            Return ChunkBuffer.ToArray
        End Function

        Private Function CreateFluxObject(TF As String, OCS As KeyValuePair, Inducers As String(), Pi As String) _
            As GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly()
            Dim ChunkBuffer As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly) =
                (From Inducer As String In Inducers Select ChemotaxisInduction(OCS.Key, Pi, Inducer)).ToList

            Return ChunkBuffer.ToArray
        End Function

        Private Function ChemotaxisInduction(Chemotaxis As String, Pi As String, Inducer As String) _
            As GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly

            Dim Reactants = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = Inducer, .StoiChiometry = 1},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = "ATP", .StoiChiometry = 1},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = Chemotaxis, .StoiChiometry = 1}}

            Dim Products = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = Inducer, .StoiChiometry = 1},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = "ADP", .StoiChiometry = 1},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = String.Format("[{0}][{1}]", Chemotaxis, Pi), .StoiChiometry = 1}}

            Return New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly With {
                .ComplexComponents = New String() {Inducer, "ATP", Chemotaxis},
                .Reversible = False, .Reactants = Reactants, .Products = Products, .Identifier = String.Format("[{0}][{1}]", Inducer, Chemotaxis),
                .LOWER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = -10},
                .UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = 10},
                .p_Dynamics_K_1 = 1}
        End Function

        Private Function PhosphoTransfer(Donor As String, Reciever As String, Confidence As Double, Pi As String) _
            As GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly

            Dim Reaction = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With {
                 .Reversible = False, .p_Dynamics_K_1 = Confidence,
                .Products = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = String.Format("[{0}][{1}]", Reciever, Pi), .StoiChiometry = 1},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = Donor, .StoiChiometry = 1}},
                .Reactants = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = String.Format("[{0}][{1}]", Donor, Pi)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = Reciever}},
                .Identifier = String.Format("{0}|{1}-PHOSPHO-TRANSFER", Donor, Reciever),
                .LOWER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = -10},
                .UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = 10}}

            Return Reaction
        End Function
    End Module
End Namespace
