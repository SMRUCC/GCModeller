#Region "Microsoft.VisualBasic::c983d14c16def748d3502bc0fd48937a, GCModeller\engine\BootstrapLoader\Loader.vb"

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

    '   Total Lines: 116
    '    Code Lines: 86
    ' Comment Lines: 7
    '   Blank Lines: 23
    '     File Size: 4.71 KB


    '     Class Loader
    ' 
    '         Properties: isLoadded, massLoader, massTable
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: create, CreateEnvironment, GetCentralDogmaFluxLoader, GetFluxIndex, GetMetabolismNetworkLoader
    '                   GetProteinMatureFluxLoader, GetProteinMatureId, GetTranscriptionId, GetTranslationId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

        Friend ReadOnly define As Definition
        Friend ReadOnly dynamics As FluxBaseline
        Friend ReadOnly vcellEngine As New Vessel

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

        Sub New(define As Definition, dynamics As FluxBaseline)
            Me.define = define
            Me.dynamics = dynamics

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

        Public Function CreateEnvironment(cell As CellularModule) As Vessel
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

            Return vcellEngine _
                .load(massTable.AsEnumerable) _
                .load(processes)
        End Function
    End Class
End Namespace
