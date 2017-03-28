#Region "Microsoft.VisualBasic::446d9f3271b4ba2416a847512833e796, ..\core\Bio.Assembly\Assembly\Expasy\csv\Enzyme.vb"

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

Imports System.Data.Linq.Mapping

Namespace Assembly.Expasy.Database.csv

    Public Class Enzyme

        <Column(Name:="id")> Public Property Identification As String
        Public Property Description As String
        Public Property AlternateName As String()
        Public Property Cofactor As String()
        Public Property PROSITE As String()
        Public Property Comments As String

        Public Overrides Function ToString() As String
            Return Identification
        End Function

        Public Shared Function CreateObject(EnzymeData As Database.Enzyme) As Enzyme
            Dim EnzymeObject As New Enzyme With {
                .AlternateName = EnzymeData.AlternateName,
                .Cofactor = EnzymeData.Cofactor,
                .Comments = EnzymeData.Comments,
                .Description = EnzymeData.Description,
                .Identification = EnzymeData.Identification,
                .PROSITE = EnzymeData.PROSITE
            }
            Return EnzymeObject
        End Function
    End Class
End Namespace
