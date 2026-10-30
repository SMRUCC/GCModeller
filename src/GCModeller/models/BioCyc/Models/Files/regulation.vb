#Region "Microsoft.VisualBasic::0560b25e8c0c224ae05907cc0c2d6eae, models\BioCyc\Models\Files\regulation.vb"

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

    '   Total Lines: 13
    '    Code Lines: 10 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (23.08%)
    '     File Size: 368 B


    ' Class regulation
    ' 
    '     Properties: mode, regulatedEntity, regulator
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("regulation.dat")>
Public Class regulation : Inherits Model

    <AttributeField("MODE")>
    Public Property mode As String
    <AttributeField("REGULATED-ENTITY")>
    Public Property regulatedEntity As String
    <AttributeField("REGULATOR")>
    Public Property regulator As String

    ''' <summary>
    ''' the DNA binding site of current regulation relationship
    ''' </summary>
    ''' <returns></returns>
    <AttributeField("ASSOCIATED-BINDING-SITE")> Public Property associatedBindingSite As String()
    <AttributeField("ACCESSORY-PROTEINS")> Public Property accessoryProteins As String()
    <AttributeField("MECHANISM")> Public Property mechanism As String
    <AttributeField("KI")> Public Property Ki As Double
    <AttributeField("GROWTH-CONDITIONS")> Public Property growthConditions As String
    <AttributeField("REGULON")> Public Property regulon As String
    <AttributeField("REQUIRED-BINDING-SITE")> Public Property requiredBindingSite As String()

End Class
