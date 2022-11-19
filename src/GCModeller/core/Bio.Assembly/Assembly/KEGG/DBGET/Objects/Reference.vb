#Region "Microsoft.VisualBasic::bee731e8b55c138718d4b7ad944a6d3c, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Reference.vb"

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

    '   Total Lines: 52
    '    Code Lines: 26
    ' Comment Lines: 17
    '   Blank Lines: 9
    '     File Size: 1.68 KB


    '     Class Reference
    ' 
    '         Properties: Authors, DOI, Journal, PMID, Reference
    '                     Title
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 参考文献
    ''' </summary>
    Public Class Reference

        <XmlElement>
        Public Property Authors As String()
        Public Property Title As String

        <XmlAttribute> Public Property Journal As String
        <XmlAttribute> Public Property Reference As String
        <XmlAttribute> Public Property DOI As String

        Public Property PMID As String

        Sub New()
        End Sub

        ''' <summary>
        ''' 解析Disease文件之中的参考文献数据
        ''' </summary>
        ''' <param name="meta$"></param>
        ''' <remarks>
        ''' Example as:
        ''' 
        ''' ```
        ''' REFERENCE   PMID:19585782 (description, env_factor)
        '''   AUTHORS   Larsen JC, Johnson NH
        '''   TITLE     Pathogenesis Of Burkholderia pseudomallei And Burkholderia mallei.
        '''   JOURNAL   Mil Med 174:647-51 (2009)
        ''' ```
        ''' </remarks>
        Sub New(meta$())
            Dim data = meta.ToDictionary(
                Function(k) Mid(k, 1, 12).StripBlank,
                Function(v) Mid(v, 13).StripBlank)

            Authors = data.TryGetValue("AUTHORS").StringSplit(",\s*")
            Title = data.TryGetValue("TITLE")
            Journal = data.TryGetValue("JOURNAL")
            Reference = data.TryGetValue("REFERENCE")
        End Sub

        Public Overrides Function ToString() As String
            Return $"{ String.Join(", ", Authors) }. {Title}. {Journal}.  PMID:{Reference}"
        End Function
    End Class
End Namespace
