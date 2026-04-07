#Region "Microsoft.VisualBasic::a1b72f474b32c318ed30736bc598e36b, engine\BootstrapLoader\BootLoader\TransmembraneFluxLoader.vb"

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

    '   Total Lines: 43
    '    Code Lines: 33 (76.74%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (23.26%)
    '     File Size: 1.62 KB


    '     Class TransmembraneFluxLoader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateFlux, GetMassSet
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace ModelLoader

    Public Class TransmembraneFluxLoader : Inherits FluxLoader

        ReadOnly culturalMedium As String

        Public Sub New(loader As Loader)
            MyBase.New(loader)
            Me.culturalMedium = loader.define.CultureMedium
        End Sub

        Protected Overrides Iterator Function CreateFlux() As IEnumerable(Of Channel)
            Dim cellular_id As String = cell.CellularEnvironmentName

            If loader.define.transmembrane Is Nothing Then
                Return
            End If

            For Each id As String In loader.define.transmembrane.passive.SafeQuery
                Dim left = MassTable.variable(id, cellular_id)
                Dim right = MassTable.variable(id, culturalMedium)
                Dim flux As New Channel(left, right) With {
                    .bounds = New Boundary(10, 10),
                    .forward = Controls.StaticControl(10),
                    .reverse = Controls.StaticControl(10),
                    .ID = $"[passive] transmembrane transportation of {id} from cell {cellular_id} to {culturalMedium}",
                    .name = .ID
                }

                Call loader.fluxIndex(MetabolismNetworkLoader.MembraneTransporter).Add(flux.ID)

                Yield flux
            Next
        End Function

        Protected Overrides Function GetMassSet() As IEnumerable(Of String)
            Return New String() {}
        End Function
    End Class
End Namespace
