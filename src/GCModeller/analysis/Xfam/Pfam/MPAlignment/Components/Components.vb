#Region "Microsoft.VisualBasic::7f6f3956a1c829b894259bd825998567, ..\GCModeller\analysis\Xfam\Pfam\MPAlignment\Components\Components.vb"

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

Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace ProteinDomainArchitecture.MPAlignment

    ''' <summary>
    ''' Domain position specifc distributions
    ''' </summary>
    Public Class DomainDistribution

        <XmlAttribute> Public Property DomainId As String
        ''' <summary>
        ''' Position collection for this <see cref="DomainId">domain object item</see>
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property Distribution As Position()

        Public Shared ReadOnly Property EmptyDomain As DomainDistribution
            Get
                Dim nullDistr As Position() = {
                    New Position With {
                        .Left = 1,
                        .Right = 1
                    }
                }
                Dim empty As New DomainDistribution With {
                    .DomainId = EmptyId,
                    .Distribution = nullDistr
                }
                Return empty
            End Get
        End Property

        Public Const EmptyId As String = "*****"

        Public Overrides Function ToString() As String
            Return DomainId
        End Function
    End Class
End Namespace
