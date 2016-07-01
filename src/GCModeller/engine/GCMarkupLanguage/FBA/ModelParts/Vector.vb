#Region "Microsoft.VisualBasic::2ae92087c386f230cd1c9b6324be7ddd, ..\GCModeller\engine\GCMarkupLanguage\FBA\ModelParts\Vector.vb"

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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.SBML
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Net.Protocols

Namespace FBACompatibility

    Public Class Vector : Inherits Streams.Array.Double
        Implements FLuxBalanceModel.IMetabolite

        ''' <summary>
        ''' The Unique ID property for the metabolite.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Identifier As String Implements FLuxBalanceModel.IMetabolite.Identifier

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        <XmlAttribute>
        Public Property InitializeAmount As Double Implements FLuxBalanceModel.IMetabolite.InitializeAmount
    End Class
End Namespace
