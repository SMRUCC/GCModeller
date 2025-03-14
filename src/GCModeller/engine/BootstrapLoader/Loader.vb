﻿#Region "Microsoft.VisualBasic::11b1e4b9e7c486b8beee4ae3e82c6380, engine\BootstrapLoader\Loader.vb"

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

    '   Total Lines: 155
    '    Code Lines: 113 (72.90%)
    ' Comment Lines: 13 (8.39%)
    '    - Xml Docs: 76.92%
    ' 
    '   Blank Lines: 29 (18.71%)
    '     File Size: 6.18 KB


    '     Class Loader
    ' 
    '         Properties: isLoadded, massLoader, massTable, strict
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: create, CreateEnvironment, GetCentralDogmaFluxLoader, GetFluxIndex, getKernel
    '                   GetMetabolismNetworkLoader, GetProteinMatureFluxLoader, GetProteinMatureId, GetTranscriptionId, GetTranslationId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Namespace ModelLoader

    ''' <summary>
    ''' Module loader
    ''' </summary>
    Public Class Loader

        ''' <summary>
        ''' some necessary constant mapping of the id name
        ''' </summary>
        Friend ReadOnly define As Definition
        Friend ReadOnly dynamics As FluxBaseline

        Dim vcellEngine As Vessel
        Dim centralDogmaFluxLoader As CentralDogmaFluxLoader
        Dim proteinMatureFluxLoader As ProteinMatureFluxLoader
        Dim metabolismNetworkLoader As MetabolismNetworkLoader

        ''' <summary>
        ''' This mass table object is generated automatically 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property massTable As New MassTable
        Public ReadOnly Property massLoader As MassLoader

        Protected Friend ReadOnly fluxIndex As New Dictionary(Of String, List(Of String))

        Public ReadOnly Property isLoadded As Boolean
            Get
                Return Not centralDogmaFluxLoader Is Nothing AndAlso
                       Not proteinMatureFluxLoader Is Nothing AndAlso
                       Not metabolismNetworkLoader Is Nothing
            End Get
        End Property

        Public Property strict As Boolean = False

        Sub New(define As Definition, dynamics As FluxBaseline)
            Me.define = define
            Me.dynamics = dynamics

            If Me.define Is Nothing Then
                Me.define = New Definition
            End If
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getKernel() As Vessel
            Return vcellEngine
        End Function

        Public Function GetFluxIndex() As Dictionary(Of String, String())
            Return fluxIndex _
                .ToDictionary(Function(m) m.Key,
                              Function(t)
                                  Return t.Value.ToArray
                              End Function)
        End Function

        Public Shared Function GetTranscriptionId(cd As CentralDogma) As String
            Return $"{cd.geneID}::transcript.process"
        End Function

        Public Shared Function GetTranslationId(cd As CentralDogma) As String
            Return $"{cd.geneID}::translate.process"
        End Function

        Public Shared Function GetProteinMatureId(protein As Protein) As String
            Return $"{protein.ProteinID}::mature.process"
        End Function

        Public Function GetCentralDogmaFluxLoader() As CentralDogmaFluxLoader
            If centralDogmaFluxLoader Is Nothing Then
                centralDogmaFluxLoader = New CentralDogmaFluxLoader(Me)
            End If

            Return centralDogmaFluxLoader
        End Function

        Public Function GetProteinMatureFluxLoader() As ProteinMatureFluxLoader
            If proteinMatureFluxLoader Is Nothing Then
                proteinMatureFluxLoader = New ProteinMatureFluxLoader(Me)
            End If

            Return proteinMatureFluxLoader
        End Function

        Public Function GetMetabolismNetworkLoader() As MetabolismNetworkLoader
            If metabolismNetworkLoader Is Nothing Then
                metabolismNetworkLoader = New MetabolismNetworkLoader(Me)
            End If

            Return metabolismNetworkLoader
        End Function

        Public Function CreateEnvironment(cell As CellularModule, <Out> ByRef core As Vessel) As Vessel
            vcellEngine = core

            ' create the flux simulation environment
            _massLoader = New MassLoader(Me)
            _massLoader.doMassLoadingOn(cell)

            Return cell.DoCall(AddressOf create)
        End Function

        Private Function create(cell As CellularModule) As Vessel
            Dim centralDogmas = cell.DoCall(AddressOf GetCentralDogmaFluxLoader().CreateFlux).AsList
            Dim proteinMatrues = cell.DoCall(AddressOf GetProteinMatureFluxLoader().CreateFlux).ToArray
            Dim metabolism = cell.DoCall(AddressOf GetMetabolismNetworkLoader().CreateFlux).ToArray
            Dim degradationFluxLoader As New BioMoleculeDegradation(Me) With {
                .proteinMatures = proteinMatrues
            }
            Dim degradation = cell.DoCall(AddressOf degradationFluxLoader.CreateFlux).ToArray
            Dim processes As Channel() = centralDogmas + proteinMatrues + metabolism + degradation

            For Each link_ref As String In New FluxLoader() {
                metabolismNetworkLoader,
                proteinMatureFluxLoader,
                centralDogmaFluxLoader,
                degradationFluxLoader
            }.Select(Function(loader) loader.LinkingMassSet) _
             .IteratesALL _
             .Distinct

                ' check of the broken mass reference
                If Not massTable.Exists(link_ref) Then
                    Dim warn As String = $"found broken mass reference: {link_ref}"

                    If strict Then
                        Throw New InvalidProgramException(warn)
                    Else
                        Call massTable.AddNew(link_ref, MassRoles.compound)
                        Call warn.Warning
                        Call VBDebugger.EchoLine("[warn] " & warn)
                    End If
                End If
            Next

            ' setup engine environment
            Return vcellEngine _
                .load(massTable.AsEnumerable) _
                .load(processes)
        End Function
    End Class
End Namespace
