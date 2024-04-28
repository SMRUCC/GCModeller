#Region "Microsoft.VisualBasic::4a12022a11e1011188672a31c0200552, G:/GCModeller/src/GCModeller/foundation/PSICQUIC/psidev//XML/v30.vb"

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

    '   Total Lines: 23
    '    Code Lines: 7
    ' Comment Lines: 12
    '   Blank Lines: 4
    '     File Size: 605 B


    '     Class v30
    ' 
    '         Properties: minorVersion
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace XML

    ''' <summary>
    ''' PSI-MI XML v3.0 data interchange format
    ''' </summary>
    ''' <remarks>
    ''' Example:
    ''' 
    ''' ```xml
    ''' &lt;mif:entrySet level="3" minorVersion="0" version="0" xmlns:mif="http://psi.hupo.org/mi/mif300">
    '''     &lt;mif:entry>{1,unbounded}&lt;/mif:entry>
    ''' &lt;/mif:entrySet>
    ''' ```
    ''' </remarks>
    <XmlRoot("entrySet", [Namespace]:="http://psi.hupo.org/mi/mif300")>
    Public Class v30

        Public Property minorVersion As Integer

    End Class
End Namespace
