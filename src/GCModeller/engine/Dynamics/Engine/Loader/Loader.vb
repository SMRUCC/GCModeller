#Region "Microsoft.VisualBasic::2e85940b5a03306809eec7e6ae6d1c7e, engine\Dynamics\Loader.vb"

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

' Class Loader
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: CreateEnvironment, transcriptionTemplate, translationTemplate
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine.ModelLoader

    Public MustInherit Class FluxLoader

        Public ReadOnly Property MassTable As MassTable
            Get
                Return loader.massTable
            End Get
        End Property

        Protected ReadOnly loader As Loader

        Protected Sub New(loader As Loader)
            Me.loader = loader
        End Sub

        Public MustOverride Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)

    End Class

    ''' <summary>
    ''' Module loader
    ''' </summary>
    Public Class Loader

        ''' <summary>
        ''' This mass table object is generated automatically 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property massTable As New MassTable

        Friend ReadOnly define As Definition

        Sub New(define As Definition)
            Me.define = define
        End Sub

        Public Shared Function GetTranscriptionId(cd As CentralDogma) As String
            Return $"{cd.geneID}::transcript.process"
        End Function

        Public Shared Function GetTranslationId(cd As CentralDogma) As String
            Return $"{cd.geneID}::translate.process"
        End Function

        Public Shared Function GetProteinMatureId(protein As Protein) As String
            Return $"{protein.ProteinID}::mature.process"
        End Function

        ''' <summary>
        ''' 构建代谢网络
        ''' </summary>
        ''' <param name="cell"></param>
        ''' <returns></returns>
        Private Iterator Function metabolismNetwork(cell As CellularModule) As IEnumerable(Of Channel)
            Dim KOfunctions = cell.Genotype.centralDogmas _
                .Where(Function(cd) Not cd.orthology.StringEmpty) _
                .Select(Function(cd) (cd.orthology, cd.polypeptide)) _
                .GroupBy(Function(pro) pro.Item1) _
                .ToDictionary(Function(KO) KO.Key,
                              Function(ortholog)
                                  Return ortholog _
                                      .Select(Function(map) map.Item2) _
                                      .ToArray
                              End Function)

            For Each reaction As Reaction In cell.Phenotype.fluxes
                Dim left = massTable.variables(reaction.substrates)
                Dim right = massTable.variables(reaction.products)
                Dim bounds As New Boundary With {
                    .forward = reaction.bounds.Max,
                    .reverse = reaction.bounds.Min
                }

                ' KO
                Dim enzymeProteinComplexes As String() = reaction.enzyme _
                    .SafeQuery _
                    .Distinct _
                    .OrderBy(Function(KO) KO) _
                    .ToArray
                ' protein id
                enzymeProteinComplexes = enzymeProteinComplexes _
                    .Where(AddressOf KOfunctions.ContainsKey) _
                    .Select(Function(ko) KOfunctions(ko)) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray
                ' mature protein complex
                enzymeProteinComplexes = enzymeProteinComplexes _
                    .Select(Function(id) id & ".complex") _
                    .ToArray

                If reaction.is_enzymatic AndAlso enzymeProteinComplexes.Length = 0 Then
                    bounds = {0, 10}
                End If

                Dim metabolismFlux As New Channel(left, right) With {
                    .bounds = bounds,
                    .ID = reaction.ID,
                    .forward = New Controls With {
                        .activation = massTable _
                            .variables(enzymeProteinComplexes, 2) _
                            .ToArray,
                        .baseline = 15
                    },
                    .reverse = New Controls With {.baseline = 15}
                }

                Yield metabolismFlux
            Next
        End Function

        Public Function CreateEnvironment(cell As CellularModule) As Vessel
            ' 在这里需要首选构建物质列表
            ' 否则下面的转录和翻译过程的构建会出现找不到物质因子对象的问题
            For Each reaction As Reaction In cell.Phenotype.fluxes
                For Each compound In reaction.AllCompounds
                    If Not massTable.Exists(compound) Then
                        Call massTable.AddNew(compound)
                    End If
                Next
            Next

            Dim centralDogmas = cell.DoCall(AddressOf New CentralDogmaFluxLoader(Me).CreateFlux).AsList
            Dim proteinMatrues = cell.DoCall(AddressOf New ProteinMatureFluxLoader(Me).CreateFlux).ToArray
            Dim metabolism = cell.DoCall(AddressOf metabolismNetwork).ToArray

            Return New Vessel With {
                .Channels = centralDogmas + proteinMatrues + metabolism,
                .MassEnvironment = massTable.ToArray
            }
        End Function
    End Class
End Namespace