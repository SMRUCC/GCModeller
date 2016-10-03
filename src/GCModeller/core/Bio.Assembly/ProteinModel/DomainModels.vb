#Region "Microsoft.VisualBasic::ca89d9ae74472199261e80b667cacf87, ..\GCModeller\core\Bio.Assembly\ProteinModel\DomainModels.vb"

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

Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ProteinModel

    Public Class DomainModel
        Implements sIdEnumerable, IKeyValuePairObject(Of String, Location)

        Public Property DomainId As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, Location).Identifier
        Public Property Location As Location Implements IKeyValuePairObject(Of String, Location).Value

        Sub New(DomainId As String, Location As Location)
            Me.DomainId = DomainId
            Me.Location = Location
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", DomainId, Location.ToString)
        End Function
    End Class
End Namespace
