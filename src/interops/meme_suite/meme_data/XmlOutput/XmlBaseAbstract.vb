#Region "Microsoft.VisualBasic::0e52ca30ec62dca07dcdb1ca18fe0f8c, meme_suite\MEME.DocParser\XmlOutput\XmlBaseAbstract.vb"

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

    '     Class MEMEXmlBase
    ' 
    '         Properties: Release, Version
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class ModelBase
    ' 
    '         Properties: CommandLine, Host
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel

Namespace DocumentFormat.XmlOutput

    Public MustInherit Class MEMEXmlBase : Inherits XmlDataModel

        <XmlAttribute("version")> Public Property Version As String
        <XmlAttribute("release")> Public Property Release As String

        Protected Friend Sub New()
        End Sub
    End Class

    Public MustInherit Class ModelBase

        <XmlElement("command_line")>
        Public Property CommandLine As String
        <XmlElement("host")>
        Public Property Host As String

        Protected Friend Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{Host} >>>]   ""{CommandLine}"""
        End Function
    End Class
End Namespace
