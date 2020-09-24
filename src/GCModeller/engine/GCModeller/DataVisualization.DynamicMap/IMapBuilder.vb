#Region "Microsoft.VisualBasic::0fdcc7899c46304e27ab027a25b91746, engine\GCModeller\DataVisualization.DynamicMap\IMapBuilder.vb"

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

    '     Class IMapBuilder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Component
    ' 
    '         Properties: Quantity
    ' 
    '     Class ComponentInteraction
    ' 
    '         Properties: InteractionType, value
    ' 
    '         Function: ShadowCopy
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace DataVisualization.DynamicMap

    Public MustInherit Class IMapBuilder : Inherits RuntimeObject

        Protected _cellSystemODM As EngineSystem.ObjectModels.SubSystem.CellSystem

        Sub New(ObjectModel As EngineSystem.ObjectModels.SubSystem.CellSystem)
            _cellSystemODM = ObjectModel
        End Sub
    End Class

    Public Class Component : Inherits Network.FileStream.Node
        Public Property Quantity As Double
    End Class

    Public Class ComponentInteraction : Inherits Network.FileStream.NetworkEdge

        Dim _InteractionType As InteractionTypes

        Public Shadows Property InteractionType As InteractionTypes
            Get
                Return _InteractionType
            End Get
            Set(value As InteractionTypes)
                _InteractionType = value
                MyBase.Interaction = value.ToString
            End Set
        End Property

        <Column("FluxValue")> Public Overrides Property value As Double
            Get
                Return MyBase.value
            End Get
            Set(value As Double)
                MyBase.value = value
            End Set
        End Property

        Public Function ShadowCopy() As ComponentInteraction
            Return New ComponentInteraction With {
                .FromNode = FromNode,
                .ToNode = ToNode,
                .InteractionType = InteractionType
            }
        End Function
    End Class
End Namespace
