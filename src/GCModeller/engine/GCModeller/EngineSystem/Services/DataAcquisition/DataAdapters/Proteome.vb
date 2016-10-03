#Region "Microsoft.VisualBasic::9562de1d3b30395990f3c41fe48394df, ..\GCModeller\engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAdapters\Proteome.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

    Public Class Proteome : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ProteinAssembly)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "proteome"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ProteinAssembly)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = From Protein In MyBase.System.Proteins Let value = Protein.DataSource Select value  '
            Return LQuery.ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In MyBase.System.Proteins Select item.SerialsHandle).ToArray
        End Function
    End Class
End Namespace
