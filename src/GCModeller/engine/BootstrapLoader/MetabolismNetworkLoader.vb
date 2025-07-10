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

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
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

        Public Sub New(loader As Loader)
            MyBase.New(loader)

            ' content of these metabolite will be changed
            infinitySource = loader.define.GetInfinitySource
            loader.fluxIndex.Add(NameOf(MetabolismNetworkLoader), New List(Of String))
        End Sub

        ''' <summary>
        ''' create reaction flux data
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Iterator Function CreateFlux() As IEnumerable(Of Channel)
            Dim KOfunctions = cell.Genotype.centralDogmas _
                .Select(Function(cd) (If(cd.orthology, cd.geneID), cd.polypeptide)) _
                .GroupBy(Function(pro) pro.Item1) _
                .ToDictionary(Function(KO) KO.Key,
                              Function(ortholog)
                                  Return ortholog _
                                      .Select(Function(map) map.Item2) _
                                      .ToArray
                              End Function)
            Dim generals = loader.define.GenericCompounds

            If generals Is Nothing Then
                generals = New Dictionary(Of String, GeneralCompound)
            End If

            For Each reaction As Reaction In cell.Phenotype.fluxes
                If reaction.AllCompounds.Any(AddressOf generals.ContainsKey) Then
                    For Each instance In generalFluxExpansion(reaction, KOfunctions)
                        Yield instance
                    Next
                Else
                    Yield fluxByReaction(reaction, KOfunctions)
                End If
            Next
        End Function

        Private Function fluxByReaction(reaction As Reaction, KOfunctions As Dictionary(Of String, String())) As Channel
            Dim left As Variable() = MassTable.variables(reaction.equation.Reactants, infinitySource).ToArray
            Dim right As Variable() = MassTable.variables(reaction.equation.Products, infinitySource).ToArray
            Dim bounds As New Boundary With {
                .forward = reaction.bounds(1),
                .reverse = reaction.bounds(0)
            }
            Dim productCompart As String = reaction.equation.Products.Select(Function(c) c.Compartment).GroupBy(Function(c) c).OrderByDescending(Function(c) c.Count).First.Key
            Dim reactantCompart As String = reaction.equation.Reactants.Select(Function(c) c.Compartment).GroupBy(Function(c) c).OrderByDescending(Function(c) c.Count).First.Key

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
                .Select(Function(id) id & ".complex") _
                .ToArray

            Dim forward As Controls

            If Not reaction.kinetics.IsNullOrEmpty Then
                If reaction.kinetics.Length = 1 Then
                    Dim scalar As Kinetics = reaction.kinetics(0)

                    forward = New KineticsControls(
                        env:=loader.getKernel,
                        lambda:=scalar.CompileLambda,
                        raw:=scalar.formula,
                        pars:=scalar.paramVals _
                            .SafeQuery _
                            .Select(Function(a) a.ToString) _
                            .ToArray,
                        cellular_id:=reaction.enzyme_compartment
                    )
                    pull.AddRange(DirectCast(forward, KineticsControls).parameters)
                Else
                    ' multiple kineticis overlaps
                    forward = New KineticsOverlapsControls(
                        From k In reaction.kinetics Select New KineticsControls(
                            env:=loader.getKernel,
                            lambda:=k.CompileLambda,
                            raw:=k.formula,
                            pars:=k.paramVals _
                                .SafeQuery _
                                .Select(Function(a) a.ToString) _
                                .ToArray,
                            cellular_id:=reaction.enzyme_compartment
                        )
                    )
                    pull.AddRange(DirectCast(forward, KineticsOverlapsControls).parameters)
                End If
            Else
                forward = New AdditiveControls With {
                    .activation = MassTable _
                        .variables(enzymeProteinComplexes, 1, reaction.enzyme_compartment) _
                        .ToArray,
                    .baseline = 5
                }
            End If

            Dim metabolismFlux As New Channel(left, right) With {
                .bounds = bounds,
                .ID = reaction.ID,
                .forward = forward,
                .reverse = Controls.StaticControl(5)
            }

            ' 假设所有的反应过程化都存在产物抑制效应
            metabolismFlux.forward.inhibition = productInhibitionFactor(metabolismFlux.right, productCompart)
            metabolismFlux.reverse.inhibition = productInhibitionFactor(metabolismFlux.left, reactantCompart)

            Call loader.fluxIndex(NameOf(MetabolismNetworkLoader)).Add(metabolismFlux.ID)

            Return metabolismFlux
        End Function

        Private Iterator Function generalFluxExpansion(template As Reaction, KOfunctions As Dictionary(Of String, String())) As IEnumerable(Of Channel)
            Call $"Generic {template.ID} = {template.name}".__INFO_ECHO
        End Function

        Private Function productInhibitionFactor(factors As IEnumerable(Of Variable), compart_id As String) As Variable()
            Return factors _
                .Where(Function(fac) Not fac.mass.ID Like infinitySource) _
                .DoCall(Function(objects)
                            Return MassTable.variables(objects, loader.dynamics.productInhibitionFactor, compart_id)
                        End Function) _
                .ToArray
        End Function

        Protected Overrides Function GetMassSet() As IEnumerable(Of String)
            Return pull
        End Function
    End Class
End Namespace
