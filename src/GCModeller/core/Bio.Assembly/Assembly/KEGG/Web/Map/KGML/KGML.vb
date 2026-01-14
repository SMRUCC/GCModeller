#Region "Microsoft.VisualBasic::af369249722a6d669c5157e5e80f51ea, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\KGML.vb"

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

    '   Total Lines: 35
    '    Code Lines: 24 (68.57%)
    ' Comment Lines: 5 (14.29%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 6 (17.14%)
    '     File Size: 1.34 KB


    '     Class pathway
    ' 
    '         Properties: image, items, link, name, number
    '                     org, reactions, relations, title
    ' 
    '         Function: ResourceURL, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' The kegg pathway map layout data in KGML file format.
    ''' 
    ''' + pathway map: http://www.kegg.jp/kegg-bin/download?entry=xcb00280&amp;format=kgml
    ''' </summary>
    Public Class pathway

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property org As String
        <XmlAttribute> Public Property number As String
        <XmlAttribute> Public Property title As String
        <XmlAttribute> Public Property image As String
        <XmlAttribute> Public Property link As String

#Region "pathway network"
        <XmlElement(NameOf(entry))> Public Property items As entry()
        <XmlElement(NameOf(relation))> Public Property relations As relation()
        <XmlElement(NameOf(reaction))> Public Property reactions As reaction()
#End Region

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ResourceURL(entry As String) As String
            Return $"http://www.kegg.jp/kegg-bin/download?entry={entry}&format=kgml"
        End Function

        Public Overrides Function ToString() As String
            Return $"[{name}] {title}"
        End Function
    End Class
End Namespace
