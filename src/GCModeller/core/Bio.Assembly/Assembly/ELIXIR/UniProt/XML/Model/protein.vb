#Region "Microsoft.VisualBasic::f0a3ecda6652ba3a8bb9edd03f829cef, core\Bio.Assembly\Assembly\ELIXIR\UniProt\XML\Model\protein.vb"

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

    '   Total Lines: 39
    '    Code Lines: 25 (64.10%)
    ' Comment Lines: 8 (20.51%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (15.38%)
    '     File Size: 1.40 KB


    '     Class protein
    ' 
    '         Properties: alternativeNames, fullName, recommendedName, submittedName
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Assembly.Uniprot.XML

    ''' <summary>
    ''' Describes the names for the protein and parts thereof.
    ''' Equivalent to the flat file DE-line.
    ''' </summary>
    Public Class protein

        Public Property recommendedName As recommendedName
        Public Property submittedName As recommendedName

        <XmlElement("alternativeName")>
        Public Property alternativeNames As recommendedName()

        ''' <summary>
        ''' <see cref="recommendedName"/> -> <see cref="submittedName"/> -> <see cref="alternativeNames"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property fullName As String
            Get
                If recommendedName Is Nothing OrElse recommendedName.fullName Is Nothing Then
                    If submittedName Is Nothing OrElse submittedName.fullName Is Nothing Then
                        Return alternativeNames.FirstOrDefault().fullName.value
                    Else
                        Return submittedName.fullName.value
                    End If
                Else
                    Return recommendedName.fullName.value
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return fullName
        End Function
    End Class
End Namespace
