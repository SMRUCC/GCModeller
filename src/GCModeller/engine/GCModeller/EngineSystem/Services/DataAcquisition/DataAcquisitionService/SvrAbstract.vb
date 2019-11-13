#Region "Microsoft.VisualBasic::074cbeca49e2eea82a87feee676248aa, engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAcquisitionService\SvrAbstract.vb"

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

    '     Interface IDataAdapter
    ' 
    '         Properties: TableName
    ' 
    '         Function: DataSource, DefHandles
    ' 
    '     Interface IDataSource
    ' 
    '         Properties: DataSource
    ' 
    '         Function: Get_DataSerializerHandles
    ' 
    '     Interface IDataSourceEntity
    ' 
    '         Properties: DataSource, SerialsHandle
    ' 
    '     Structure DataSource
    ' 
    '         Properties: Address, value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace EngineSystem.Services.DataAcquisition.Services

    Public Interface IDataAdapter
        Function DataSource() As DataSource()
        Function DefHandles() As HandleF()
        ReadOnly Property TableName As String
    End Interface

    Public Interface IDataSource
        ReadOnly Property DataSource As DataSource()
        Function Get_DataSerializerHandles() As HandleF()
    End Interface

    Public Interface IDataSourceEntity
        ReadOnly Property DataSource As DataSource
        ReadOnly Property SerialsHandle As HandleF
    End Interface

    Public Structure DataSource : Implements IAddressOf

        Public Property Address As Integer Implements IAddressOf.Address
        Public Property value As Double

        Sub New(raw As KeyValuePair(Of Long, Double))
            Address = raw.Key
            value = raw.Value
        End Sub

        Sub New(i As Long, value As Double)
            Me.Address = i
            Me.value = value
        End Sub

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.Address = address
        End Sub
    End Structure
End Namespace
