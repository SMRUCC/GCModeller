#Region "Microsoft.VisualBasic::7c98e3e631dfb2cdc2bd345823fc88de, GCModeller\core\Bio.Assembly\ProteinModel\DomainModels.vb"

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

    '   Total Lines: 46
    '    Code Lines: 36
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 1.49 KB


    '     Class DomainModel
    ' 
    '         Properties: DomainId, ends, Location, start
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ProteinModel

    ''' <summary>
    ''' The simple protein domain motif model.
    ''' </summary>
    Public Class DomainModel
        Implements INamedValue, IKeyValuePairObject(Of String, Location)
        Implements IMotifSite
        Implements IMotifDomain

        Public Property DomainId As String Implements INamedValue.Key,
            IKeyValuePairObject(Of String, Location).Key,
            IMotifSite.name,
            IMotifSite.family,
            IMotifDomain.Id
        Public Property start As Integer
        Public Property ends As Integer

        Private Property Location As Location Implements IKeyValuePairObject(Of String, Location).Value, IMotifSite.site, IMotifDomain.location
            Get
                Return New Location(start, ends)
            End Get
            Set(value As Location)
                If Not value Is Nothing Then
                    start = value.Left
                    ends = value.Right
                End If
            End Set
        End Property

        Sub New(domainId As String, Location As Location)
            Me.DomainId = domainId
            Me.Location = Location
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", DomainId, Location.ToString)
        End Function
    End Class
End Namespace
