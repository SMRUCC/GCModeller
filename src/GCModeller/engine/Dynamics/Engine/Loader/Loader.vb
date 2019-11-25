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
            Dim metabolism = cell.DoCall(AddressOf New MetabolismNetworkLoader(Me).CreateFlux).ToArray

            Return New Vessel With {
                .Channels = centralDogmas + proteinMatrues + metabolism,
                .MassEnvironment = massTable.ToArray
            }
        End Function
    End Class
End Namespace