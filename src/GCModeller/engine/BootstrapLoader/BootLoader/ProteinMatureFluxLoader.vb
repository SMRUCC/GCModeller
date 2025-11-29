#Region "Microsoft.VisualBasic::8b5d78666c01de88ab3acca70ad42193, engine\BootstrapLoader\ProteinMatureFluxLoader.vb"

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

'   Total Lines: 74
'    Code Lines: 55 (74.32%)
' Comment Lines: 4 (5.41%)
'    - Xml Docs: 75.00%
' 
'   Blank Lines: 15 (20.27%)
'     File Size: 3.02 KB


'     Class ProteinMatureFluxLoader
' 
'         Properties: polypeptides, proteinComplex
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateFlux, GetMassSet
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule

Namespace ModelLoader

    ''' <summary>
    ''' 构建酶成熟的过程
    ''' </summary>
    Public Class ProteinMatureFluxLoader : Inherits FluxLoader

        Public ReadOnly Property polypeptides As String()
        Public ReadOnly Property proteinComplex As String()

        ReadOnly pull As New List(Of String)
        ReadOnly polypeptideIds As New Index(Of String)

        Public Sub New(loader As Loader)
            MyBase.New(loader)

            loader.fluxIndex.Add(NameOf(ProteinMatureFluxLoader), New List(Of String))
        End Sub

        Protected Overrides Iterator Function CreateFlux() As IEnumerable(Of Channel)
            Dim polypeptides As New List(Of String)
            Dim proteinComplex As New List(Of String)
            Dim flux As Channel
            Dim cellular_id As String = cell.CellularEnvironmentName

            For Each proteinId As String In cell.GetPolypeptideIds
                Call polypeptideIds.Add(proteinId)
            Next

            For Each complex As Protein In cell.Phenotype.proteins
                For Each compound In complex.compounds.SafeQuery
                    If Not MassTable.Exists(compound, cellular_id) Then
                        Call MassTable.addNew(compound, MassRoles.compound, cellular_id)
                    End If
                Next
                ' polypeptide or other protein complex
                For Each peptide As String In complex.polypeptides
                    If peptide Like polypeptideIds Then
                        peptide = "*" & peptide
                    End If

                    If Not MassTable.Exists(peptide, cellular_id) Then
                        Call ("Missing protein complex component polypeptide: " & peptide).warning
                        Call MassTable.addNew(peptide, MassRoles.polypeptide, cellular_id)
                    Else
                        polypeptides += peptide
                    End If
                Next

                Dim unformed = MassTable.variables(complex, cellular_id, polypeptideIds).ToArray
                Dim complexID As String = loader.massLoader.proteinComplex(complex.ProteinID)
                Dim mature As Variable = MassTable.variable(complexID, cellular_id)

                proteinComplex += complexID

                ' 酶的成熟过程也是一个不可逆的过程
                flux = New Channel(unformed, {mature}) With {
                    .ID = DataHelper.GetProteinMatureId(complex, cellular_id),
                    .reverse = Controls.StaticControl(0),
                    .forward = Controls.StaticControl(loader.dynamics.proteinMatureBaseline),
                    .bounds = New Boundary With {
                        .forward = loader.dynamics.proteinMatureCapacity,
                        .reverse = 0
                    },
                    .name = $"Protein mature from components {complex.polypeptides.JoinBy(",")} to protein complex {complex.ProteinID} in cell {cellular_id}"
                }

                If flux.isBroken Then
                    Throw New InvalidDataException(String.Format(flux.Message, flux.ID))
                End If

                loader.fluxIndex(NameOf(ProteinMatureFluxLoader)).Add(flux.ID)

                Yield flux
            Next

            _polypeptides = polypeptides
            _proteinComplex = proteinComplex
        End Function

        Protected Overrides Function GetMassSet() As IEnumerable(Of String)
            Return pull
        End Function
    End Class
End Namespace
