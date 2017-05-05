#Region "Microsoft.VisualBasic::9d6d9b11b365e2bad72e7d5facf56894, ..\GCModeller\engine\GCModeller\GUID\EnzymeClass.vb"

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

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace EngineSystem.ObjectModels.PoolMappings

    Public Structure EnzymeClass
        Implements IAddressOf
        Implements IReadOnlyId
        Implements IPoolHandle

        Public Property Handle As Integer Implements IAddressOf.Address

        Public ReadOnly Property ECNumber As String Implements IReadOnlyId.Identity, IPoolHandle.locusId

        Sub New([Class] As String)
            ECNumber = [Class]
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", Handle, Me.ECNumber)
        End Function
    End Structure

    Public Interface IMappingEdge(Of T As IPoolHandle, TMappingType As Feature.MappingFeature(Of T)) : Inherits IAddressOf
        ReadOnly Property MappingHandler As T

        Sub set_Nodes(MappingNodes As TMappingType())
    End Interface

    Public Interface IPoolHandle : Inherits IAddressOf

        ReadOnly Property locusId As String
    End Interface

    Public Structure MotifClass : Implements IPoolHandle

        Public ReadOnly Property MotifId As String Implements IPoolHandle.locusId
            Get
                Throw New NotImplementedException
            End Get
        End Property

        Public Property Handle As Integer Implements IAddressOf.Address

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
