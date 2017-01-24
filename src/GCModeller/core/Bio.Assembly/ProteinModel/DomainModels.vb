#Region "Microsoft.VisualBasic::6af956088409b8d8c33cdc5375eb5bb8, ..\GCModeller\core\Bio.Assembly\ProteinModel\DomainModels.vb"

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

    ''' <summary>
    ''' The simple protein domain motif model.
    ''' </summary>
    Public Class DomainModel
        Implements INamedValue, IKeyValuePairObject(Of String, Location)
        Implements IMotifSite

        Public Property DomainId As String Implements INamedValue.Key, IKeyValuePairObject(Of String, Location).Identifier, IMotifSite.Type, IMotifSite.Name
        Public Property Start As Integer
        Public Property [End] As Integer

        Private Property Location As Location Implements IKeyValuePairObject(Of String, Location).Value, IMotifSite.Site
            Get
                Return New Location(Start, [End])
            End Get
            Set(value As Location)
                If Not value Is Nothing Then
                    Start = value.Left
                    [End] = value.Right
                End If
            End Set
        End Property

        Sub New(DomainId As String, Location As Location)
            Me.DomainId = DomainId
            Me.Location = Location
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", DomainId, Location.ToString)
        End Function
    End Class
End Namespace
