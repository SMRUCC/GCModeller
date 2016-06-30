#Region "Microsoft.VisualBasic::dc8b44696c784241876a6d5c8dc774b2, ..\Bio.Assembly\Assembly\Expasy\CsvDumps.vb"

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

Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.Expasy.Database.CsvExport

    Public Class Enzyme
        Public Property Identification As String
        Public Property Description As String
        Public Property AlternateName As String()
        Public Property Cofactor As String()
        Public Property PROSITE As String()
        Public Property Comments As String

        Public Overrides Function ToString() As String
            Return Identification
        End Function

        Public Shared Function CreateObject(EnzymeData As Database.Enzyme) As Enzyme
            Dim EnzymeObject As Enzyme =
                New Enzyme With {
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

    Public Class SwissProt
        Public Property [Class] As String
        Public Property Entry As String
        Public Property seq As String

        Public Overrides Function ToString() As String
            Return Entry
        End Function

        Public Shared Function CreateObjects(Enzyme As Database.Enzyme) As SwissProt()
            Dim LQuery = (From Id As String
                          In Enzyme.SwissProt
                          Select New SwissProt With {
                              .Class = Enzyme.Identification,
                              .Entry = Id}).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
