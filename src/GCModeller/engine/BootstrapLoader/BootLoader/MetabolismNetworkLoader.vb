#Region "Microsoft.VisualBasic::bc43891e981fdf6261b19d30ec984d93, engine\BootstrapLoader\MetabolismNetworkLoader.vb"

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

'   Total Lines: 165
'    Code Lines: 130 (78.79%)
' Comment Lines: 13 (7.88%)
'    - Xml Docs: 53.85%
' 
'   Blank Lines: 22 (13.33%)
'     File Size: 7.50 KB


'     Class MetabolismNetworkLoader
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateFlux, fluxByReaction, generalFluxExpansion, GetMassSet, productInhibitionFactor
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.[Default]
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Namespace ModelLoader

    ''' <summary>
    ''' 构建代谢网络
    ''' </summary>
    Public Class MetabolismNetworkLoader : Inherits FluxLoader

        Dim infinitySource As Index(Of String)

        ReadOnly pull As New List(Of String)
        ReadOnly default_compartment As [Default](Of String)
        ReadOnly culturalMedium As String
        ReadOnly geneIndex As New Dictionary(Of String, CentralDogma)

        Public Const MembraneTransporter As String = "MembraneTransporter"

        Public Sub New(loader As Loader)
            MyBase.New(loader)

            ' content of these metabolite will be changed
            default_compartment = loader.massTable.defaultCompartment
            infinitySource = loader.define.GetInfinitySource
            culturalMedium = loader.define.CultureMedium

            If infinitySource.Any Then
                Call $"{infinitySource.Objects.GetJson} are assume as infinity content.".debug
            End If

            loader.fluxIndex.Add(NameOf(MetabolismNetworkLoader), New List(Of String))
            loader.fluxIndex.Add(MembraneTransporter, New List(Of String))
        End Sub

        ''' <summary>
        ''' create reaction flux data
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Iterator Function CreateFlux() As IEnumerable(Of Channel)
            Dim KOfunctions As Dictionary(Of String, String()) = cell.Genotype.centralDogmas _
                .Select(Function(cd) (If(cd.orthology, cd.geneID), cd.polypeptide)) _
                .GroupBy(Function(pro) pro.Item1) _
                .ToDictionary(Function(KO) KO.Key,
                              Function(ortholog)
                                  Return ortholog _
                                      .Select(Function(map) map.Item2) _
                                      .ToArray
                              End Function)
            Dim generals As Dictionary(Of String, GeneralCompound) = loader.define.GenericCompounds

            If generals Is Nothing Then
                generals = New Dictionary(Of String, GeneralCompound)
            End If

            For Each gene As CentralDogma In cell.Genotype.centralDogmas.SafeQuery
                geneIndex(gene.geneID) = gene
            Next

            Call VBDebugger.EchoLine("Initialize of the metabolism network...")

            For Each reaction As Reaction In TqdmWrapper.Wrap(cell.Phenotype.fluxes, wrap_console:=App.EnableTqdm)
                If reaction.AllCompounds.Any(AddressOf generals.ContainsKey) Then
                    For Each instance In generalFluxExpansion(reaction, KOfunctions)
                        Yield instance
                    Next
                Else
                    Yield fluxByReaction(reaction, KOfunctions)
                End If
            Next
        End Function

        Private Function compart_id([set] As IEnumerable(Of CompoundSpecieReference)) As String
            Dim top_compart = [set].Select(Function(c) c.Compartment) _
                .GroupBy(Function(c) c) _
                .OrderByDescending(Function(c) c.Count) _
                .First

            Return top_compart.Key Or default_compartment
        End Function

        Private Sub SetParameterLinks(kc As KineticsControls)
            Call pull.AddRange(kc.parameters)
        End Sub

        Private Iterator Function SetParameterLinks(params As IEnumerable(Of String), enzymeProteinComplexes As String()) As IEnumerable(Of String)
            For Each par As String In params
                If Not par.IsNumeric(, True) Then
                    ' processing of the protein id mapping?
                    Dim gene As CentralDogma = geneIndex.TryGetValue(par)

                    If Not gene.polypeptide.StringEmpty(, True) Then
                        par = enzymeProteinComplexes _
                            .Where(Function(id) id.StartsWith(gene.polypeptide)) _
                            .FirstOrDefault
                    End If
                End If

                Yield par
            Next
        End Function

        Private Sub SetParameterLinks(kc As KineticsOverlapsControls)
            Call pull.AddRange(kc.parameters)
        End Sub

        Private Function fluxByReaction(reaction As Reaction, KOfunctions As Dictionary(Of String, String())) As Channel
            Dim compart_idset = reaction.equation.Reactants _
                .JoinIterates(reaction.equation.Products) _
                .Select(Function(f) f.Compartment) _
                .Where(Function(s) Not s Is Nothing) _
                .Distinct _
                .ToArray
            Dim compart_suffix As String
            Dim is_transport As Boolean = reaction.equation.Products _
                .Any(Function(c)
                         Return reaction.equation.Reactants _
                             .Any(Function(ci) c.ID = ci.ID)
                     End Function)

            If reaction.enzyme_compartment = MetabolicModel.Membrane Then
                is_transport = True
            End If
            If compart_idset.IsNullOrEmpty Then
                If is_transport Then
                    compart_idset = New String() {MetabolicModel.Membrane}
                Else
                    compart_idset = New String() {default_compartment}
                End If
            End If

            If Not is_transport Then
                compart_suffix = compart_idset(0)
            Else
                compart_suffix = $"Transport[{compart_idset.JoinBy(",")}]"

                If compart_idset.Length <= 1 Then
                    For Each ref In reaction.equation.Reactants
                        ref.Compartment = default_compartment
                    Next
                    For Each ref In reaction.equation.Products
                        ref.Compartment = culturalMedium
                    Next
                End If
            End If

            Dim left As Variable() = MassTable.variables(reaction.equation.Reactants, infinitySource).ToArray
            Dim right As Variable() = MassTable.variables(reaction.equation.Products, infinitySource).ToArray
            Dim bounds As New Boundary With {
                .forward = reaction.bounds(1),
                .reverse = reaction.bounds(0)
            }
            Dim productCompart As String = compart_id(reaction.equation.Products)
            Dim reactantCompart As String = compart_id(reaction.equation.Reactants)

            ' KO
            Dim enzymeProteinComplexes As String() = reaction.enzyme _
                .SafeQuery _
                .Where(Function(str) Not str Is Nothing) _
                .Distinct _
                .OrderBy(Function(KO) KO) _
                .ToArray
            ' protein id
            enzymeProteinComplexes = enzymeProteinComplexes _
                .Where(AddressOf KOfunctions.ContainsKey) _
                .Select(Function(KO) KOfunctions(KO)) _
                .IteratesALL _
                .Where(Function(si) Not si.StringEmpty(, True)) _
                .Distinct _
                .ToArray
            ' mature protein complex
            enzymeProteinComplexes = enzymeProteinComplexes _
                .Select(Function(id)
                            Return id
                            ' Return id & ".complex"
                        End Function) _
                .ToArray

            Dim forward As Controls
            Dim reverse As Controls = New AdditiveControls With {
                .activation = right,
                .baseline = 5
            }

            ' it's enzymatic
            If Not reaction.kinetics.IsNullOrEmpty Then
                If reaction.kinetics.Length = 1 Then
                    Dim scalar As Kinetics = reaction.kinetics(0)

                    forward = New KineticsControls(
                        env:=loader.massTable,
                        lambda:=scalar.CompileLambda(geneIndex),
                        raw:=scalar.formula,
                        pars:=SetParameterLinks(scalar.paramVals _
                            .SafeQuery _
                            .Select(Function(a) a.ToString), enzymeProteinComplexes) _
                            .ToArray,
                        cellular_id:=reaction.enzyme_compartment Or default_compartment
                    )
                    SetParameterLinks(DirectCast(forward, KineticsControls))
                Else
                    ' multiple kineticis overlaps
                    forward = New KineticsOverlapsControls(
                        From k In reaction.kinetics Select New KineticsControls(
                            env:=loader.massTable,
                            lambda:=k.CompileLambda(geneIndex),
                            raw:=k.formula,
                            pars:=SetParameterLinks(k.paramVals _
                                .SafeQuery _
                                .Select(Function(a) a.ToString), enzymeProteinComplexes) _
                                .ToArray,
                            cellular_id:=reaction.enzyme_compartment Or default_compartment
                        )
                    )
                    SetParameterLinks(DirectCast(forward, KineticsOverlapsControls))
                End If
            ElseIf Not enzymeProteinComplexes.IsNullOrEmpty Then
                ' it's enzymatic, but has no kinetics law data
                forward = New AdditiveControls With {
                    .activation = MassTable _
                        .variables(enzymeProteinComplexes, 1, reaction.enzyme_compartment) _
                        .ToArray,
                    .baseline = 5
                }
            Else
                ' it's non-enzymatic
                forward = New AdditiveControls With {
                    .activation = left,
                    .baseline = 5
                }
            End If

            Dim metabolismFlux As New Channel(left, right) With {
                .bounds = bounds,
                .ID = reaction.ID & "@" & compart_suffix,
                .forward = forward,
                .reverse = reverse
            }

            ' 假设所有的反应过程化都存在产物抑制效应
            metabolismFlux.forward.inhibition = productInhibitionFactor(metabolismFlux.right, productCompart)
            metabolismFlux.reverse.inhibition = productInhibitionFactor(metabolismFlux.left, reactantCompart)

            If metabolismFlux.isBroken Then
                Throw New InvalidDataException(String.Format(metabolismFlux.Message, metabolismFlux.ID))
            End If

            If is_transport Then
                Call loader.fluxIndex(MembraneTransporter).Add(metabolismFlux.ID)
            Else
                Call loader.fluxIndex(NameOf(MetabolismNetworkLoader)).Add(metabolismFlux.ID)
            End If

            Return metabolismFlux
        End Function

        Private Iterator Function generalFluxExpansion(template As Reaction, KOfunctions As Dictionary(Of String, String())) As IEnumerable(Of Channel)
            Call $"Generic {template.ID} = {template.name}".info
        End Function

        Private Function productInhibitionFactor(factors As IEnumerable(Of Variable), compart_id As String) As Variable()
            Return factors _
                .Where(Function(fac) Not fac.mass.ID Like infinitySource) _
                .DoCall(Function(objects)
                            Return MassTable.variables(objects, loader.dynamics.productInhibitionFactor, compart_id)
                        End Function) _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetMassSet() As IEnumerable(Of String)
            Return pull _
                .Distinct _
                .Where(Function(id) Not id.IsNumeric(, True))
        End Function
    End Class
End Namespace
