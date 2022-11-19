#Region "Microsoft.VisualBasic::c3fb051734f387fcf3c2d9a7e13a1404, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\KEGGOrganism\Organism.vb"

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

    '   Total Lines: 50
    '    Code Lines: 32
    ' Comment Lines: 11
    '   Blank Lines: 7
    '     File Size: 1.76 KB


    '     Class OrganismInfo
    ' 
    '         Properties: Aliases, code, Comment, Created, DataSource
    '                     Definition, FullName, Keywords, Lineage, Reference
    '                     Sequence, Taxonomy, TID
    ' 
    '         Function: ShowOrganism, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.bGetObject.Organism

    ''' <summary>
    ''' http://www.kegg.jp/kegg-bin/show_organism?org={code}
    ''' </summary>
    Public Class OrganismInfo

        ''' <summary>
        ''' T number
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property TID As String
        ''' <summary>
        ''' 物种在KEGG数据库之中的简要缩写代码
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property code As String
        <XmlAttribute> Public Property Taxonomy As String

        Public Property Aliases As String
        Public Property FullName As String
        Public Property Definition As String
        Public Property Lineage As String
        Public Property DataSource As NamedValue()
        Public Property Keywords As String()
        Public Property Comment As String
        Public Property Sequence As String
        Public Property Created As String
        Public Property Reference As Reference()

        Public Overrides Function ToString() As String
            Return $"({code}) {FullName}"
        End Function

        Public Shared Function ShowOrganism(code As String, Optional cache$ = "./.kegg/show_organism") As OrganismInfo
            Static handlers As New Dictionary(Of String, ShowOrganism)

            Return handlers _
                .ComputeIfAbsent(
                    key:=cache,
                    lazyValue:=Function() New ShowOrganism(cache)
                ) _
                .Query(Of OrganismInfo)(code, ".html")
        End Function
    End Class
End Namespace
