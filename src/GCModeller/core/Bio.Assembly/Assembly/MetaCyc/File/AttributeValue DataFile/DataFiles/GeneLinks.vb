#Region "Microsoft.VisualBasic::236c04fd9b8b3324cfe8f6430ca41c81, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\GeneLinks.vb"

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

    '   Total Lines: 66
    '    Code Lines: 49
    ' Comment Lines: 4
    '   Blank Lines: 13
    '     File Size: 2.39 KB


    '     Class GeneLinks
    ' 
    '         Properties: Objects
    ' 
    '     Structure GeneLink
    ' 
    '         Function: ToString
    ' 
    '         Sub: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

Namespace Assembly.MetaCyc.File.DataFiles.DataTabular

    Public Class GeneLinks

        <XmlArray("Links")> Public Property Objects As GeneLink()

        Public Shared Widening Operator CType(Path As String) As GeneLinks
            Dim File As String() = Nothing
            Dim prop As [Property] = Nothing
            Call FileReader.TabularParser(Path, prop, File, "").Assertion(MSG_TYPES.WRN)
            Dim LinkSource As IEnumerable(Of GeneLink) = From s As String
                                                         In File.AsParallel
                                                         Select CType(s, GeneLink)
            Return New GeneLinks With {
                .Objects = LinkSource.ToArray
            }
        End Operator
    End Class

    ''' <summary>
    ''' 在不同的数据库之间交换数据所需要的对象连接映射，即由PGDB中的Unique映射至通用基因号的关系对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure GeneLink

        <XmlAttribute> Dim GENEId As String
        <XmlAttribute> Dim CGSCId As String
        <XmlAttribute> Dim UniProtId As String
        <XmlAttribute> Dim GeneName As String

        Public Shared Widening Operator CType(s As String) As GeneLink
            Dim GeneLink As GeneLink = New GeneLink
            Dim Data As String() = s.Split(CChar(vbTab))

            GeneLink.GENEId = Data(0)
            GeneLink.CGSCId = Data(1)
            GeneLink.UniProtId = Data(2)
            GeneLink.GeneName = Data(3)

            Return GeneLink
        End Operator

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(512)

            Append(GENEId, sBuilder)
            Append(CGSCId, sBuilder)
            Append(UniProtId, sBuilder)
            Append(GeneName, sBuilder)

            Return sBuilder.ToString
        End Function

        Friend Sub Append(e As String, ByRef sbr As StringBuilder)
            If String.IsNullOrEmpty(e) Then
                sbr.Append("NULL, ")
            Else
                sbr.Append(String.Format("{0}, ", e))
            End If
        End Sub
    End Structure
End Namespace
