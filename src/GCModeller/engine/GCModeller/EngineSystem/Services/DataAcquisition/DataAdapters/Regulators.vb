#Region "Microsoft.VisualBasic::2a86d11d523e9cf666028d0eb2444ea4, ..\GCModeller\engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAdapters\Regulators.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class TFRegulators : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
        Implements IDataAdapter

        Dim _Regulators As EngineSystem.ObjectModels.Entity.Compound()

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(System)

            Dim ChunkBuffer = (From Model In System.NetworkComponents.AsParallel
                               Where Not Model.MotifSites.IsNullOrEmpty
                               Select (From item
                                       In Model.MotifSites
                                       Select (From n In item.Regulators Select n.EntityBaseType).ToArray).ToArray.MatrixToVector).ToArray.MatrixToVector
            Me._Regulators = (From item As ObjectModels.Entity.Compound In ChunkBuffer Select item Distinct).ToArray
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From item In _Regulators Let value = item.DataSource Select value).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From item In _Regulators Select item.SerialsHandle).ToArray
            Return LQuery
        End Function

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "TFRegulatorCompounds"
            End Get
        End Property
    End Class
End Namespace
