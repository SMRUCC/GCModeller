﻿#Region "Microsoft.VisualBasic::e6dcbd15f0072d8ed1b7a03eb56eaf5b, engine\BootstrapLoader\Loader.vb"

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

'   Total Lines: 171
'    Code Lines: 125 (73.10%)
' Comment Lines: 14 (8.19%)
'    - Xml Docs: 71.43%
' 
'   Blank Lines: 32 (18.71%)
'     File Size: 6.82 KB


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

        Dim centralDogmaFluxLoader As CentralDogmaFluxLoader
        Dim proteinMatureFluxLoader As ProteinMatureFluxLoader
        Dim metabolismNetworkLoader As MetabolismNetworkLoader

        ReadOnly unitTest As Boolean = False

        ''' <summary>
        ''' This mass table object is generated automatically 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property massTable As MassTable
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

        Sub New(define As Definition, dynamics As FluxBaseline,
                Optional unitTest As Boolean = False,
                Optional massTable As MassTable = Nothing)

            Me.unitTest = unitTest
            Me.define = define
            Me.dynamics = dynamics
            Me.massTable = massTable

            If Me.define Is Nothing Then
                Me.define = New Definition
            End If
        End Sub

        Public Function GetFluxIndex() As Dictionary(Of String, String())
            Return fluxIndex _
                .ToDictionary(Function(m) m.Key,
                              Function(t)
                                  Return t.Value.ToArray
                              End Function)
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

        Public Function CreateEnvironment(cell As CellularModule) As (massTable As MassTable, processes As Channel())
            ' create the flux simulation environment
            If _massTable Is Nothing Then
                _massTable = New MassTable(cell.CellularEnvironmentName)
            End If

            _massLoader = New MassLoader(Me)
            _massLoader.doMassLoadingOn(cell)

            If unitTest Then
                ' add required
                Dim massTable As MassTable = _massLoader.massTable

                For Each id As String In define.AsEnumerable
                    For Each compart_id As String In massTable.compartment_ids
                        If Not massTable.Exists(id, compart_id) Then
                            Call massTable.addNew(id, MassRoles.compound, compart_id)
                        End If
                    Next
                Next
            End If

            Return cell.DoCall(AddressOf create)
        End Function

        Private Function create(cell As CellularModule) As (MassTable, Channel())
            Dim centralDogmas = cell.DoCall(AddressOf GetCentralDogmaFluxLoader().CreateFlux).AsList
            Dim proteinMatrues = cell.DoCall(AddressOf GetProteinMatureFluxLoader().CreateFlux).ToArray
            Dim metabolism = cell.DoCall(AddressOf GetMetabolismNetworkLoader().CreateFlux).ToArray
            Dim degradationFluxLoader As New BioMoleculeDegradation(Me) With {
                .proteinMatures = proteinMatrues
            }
            Dim degradation = cell.DoCall(AddressOf degradationFluxLoader.CreateFlux).ToArray
            Dim processes As Channel() = centralDogmas + proteinMatrues + metabolism + degradation

            For Each loader In New FluxLoader() {metabolismNetworkLoader, proteinMatureFluxLoader, centralDogmaFluxLoader, degradationFluxLoader}
                For Each link_ref As String In loader.LinkingMassSet
                    ' check of the broken mass reference
                    If Not massTable.ExistsAllCompartment(link_ref) Then
                        Dim warn As String = $"found broken mass reference: {link_ref}"

                        If strict Then
                            Throw New InvalidProgramException(warn)
                        Else
                            Call massTable.addNew(link_ref, MassRoles.compound, cell.CellularEnvironmentName)
                            Call warn.warning
                            Call warn.debug
                        End If
                    End If
                Next
            Next

            ' setup engine environment
            Return (massTable, processes)
        End Function
    End Class
End Namespace
