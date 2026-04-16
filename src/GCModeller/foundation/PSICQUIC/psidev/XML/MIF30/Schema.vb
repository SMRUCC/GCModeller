#Region "Microsoft.VisualBasic::a6caf1fa285560766a0273c65c8a93f7, foundation\PSICQUIC\psidev\XML\MIF30\Schema.vb"

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

    '   Total Lines: 55
    '    Code Lines: 19 (34.55%)
    ' Comment Lines: 30 (54.55%)
    '    - Xml Docs: 93.33%
    ' 
    '   Blank Lines: 6 (10.91%)
    '     File Size: 1.89 KB


    '     Class entry
    ' 
    '         Properties: attributeList, availabilityList, experimentList, interactionList, interactorList
    '                     source
    ' 
    '     Class source
    ' 
    '         Properties: attributeList, bibref, names, release, releaseDate
    '                     xref
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace XML.MIF30

    ''' <summary>
    ''' Describes one or more interactions as a self-contained unit. Multiple entries from different files can be
    ''' concatenated into a Single entrySet.
    ''' </summary>
    ''' <remarks>
    ''' Example:
    ''' 
    ''' ```xml
    ''' &lt;mif:entry xmlns:mif="http://psi.hupo.org/mi/mif300">
    '''    &lt;MIF:source release="" releaseDate="">{0,1}&lt;/mif:source>
    '''    &lt;MIF:availabilityList>{0,1}&lt;/mif:availabilityList>
    '''    &lt;MIF:experimentList>{0,1}&lt;/mif:experimentList>
    '''    &lt;MIF:interactorList>{0,1}&lt;/mif:interactorList>
    '''    &lt;MIF:interactionList>{1,1}&lt;/mif:interactionList>
    '''    &lt;MIF:attributeList>{0,1}&lt;/mif:attributeList>
    ''' &lt;/mif:entry>
    ''' ```
    ''' </remarks>
    Public Class entry

        Public Property source As source
        Public Property availabilityList
        Public Property experimentList
        Public Property interactorList
        Public Property interactionList
        Public Property attributeList

    End Class

    ''' <summary>
    ''' Example:
    ''' 
    ''' ```
    ''' &lt;mif:source release="" releaseDate="" xmlns:mif="http://psi.hupo.org/mi/mif300">
    '''    &lt;MIF:names>{0,1}&lt;/mif:names>
    '''    &lt;MIF:bibref>{0,1}&lt;/mif:bibref>
    '''    &lt;MIF:xref>{0,1}&lt;/mif:xref>
    '''    &lt;MIF:attributeList>{0,1}&lt;/mif:attributeList>
    ''' &lt;/mif:source>
    ''' ```
    ''' </summary>
    Public Class source

        <XmlAttribute> Public Property release As String
        <XmlAttribute> Public Property releaseDate As String
        Public Property names As String
        Public Property bibref As String
        Public Property xref As String
        Public Property attributeList
    End Class
End Namespace
