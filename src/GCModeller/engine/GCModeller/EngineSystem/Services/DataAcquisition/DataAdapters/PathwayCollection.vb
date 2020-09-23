#Region "Microsoft.VisualBasic::db140bbc57e5979bbac791c6a8f8abb2, engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAdapters\PathwayCollection.vb"

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

    '     Class PathwayCollection
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class PathwayCollection : Inherits DataAdapter(Of EngineSystem.ObjectModels.SubSystem.PathwayCollection)
        Implements IDataAdapter

        Sub New(Source As EngineSystem.ObjectModels.SubSystem.PathwayCollection)
            Call MyBase.New(Source)
        End Sub

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "pathways"
            End Get
        End Property

        Public Overrides Function DataSource() As DataSource()
            Return MyBase.System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return System.get_DataSerializerHandles
        End Function
    End Class
End Namespace
