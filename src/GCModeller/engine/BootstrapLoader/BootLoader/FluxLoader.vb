#Region "Microsoft.VisualBasic::a08f4f2b5367acc62def8a3e0be25062, engine\BootstrapLoader\FluxLoader.vb"

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

'   Total Lines: 41
'    Code Lines: 27 (65.85%)
' Comment Lines: 3 (7.32%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 11 (26.83%)
'     File Size: 1.30 KB


'     Class FluxLoader
' 
'         Properties: LinkingMassSet, MassTable
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateFlux
' 
' 
' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace ModelLoader

    ''' <summary>
    ''' Helper module for convert the cellular module as the dynamics channels
    ''' </summary>
    Public MustInherit Class FluxLoader

        Public ReadOnly Property MassTable As MassTable
            Get
                Return loader.massTable
            End Get
        End Property

        Protected ReadOnly loader As Loader
        Protected cell As CellularModule

        Public ReadOnly Property LinkingMassSet As String()

        Protected Sub New(loader As Loader)
            Me.loader = loader
        End Sub

        Public Iterator Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)
            Me.cell = cell

            For Each flux As Channel In CreateFlux()
                Yield flux
            Next

            _LinkingMassSet = GetMassSet.ToArray
        End Function

        Protected MustOverride Function CreateFlux() As IEnumerable(Of Channel)
        Protected MustOverride Function GetMassSet() As IEnumerable(Of String)

    End Class
End Namespace
