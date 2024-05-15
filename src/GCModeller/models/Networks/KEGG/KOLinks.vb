#Region "Microsoft.VisualBasic::294d500cbd8f1fd610ee629236c548f3, models\Networks\KEGG\KOLinks.vb"

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

    '   Total Lines: 62
    '    Code Lines: 36
    ' Comment Lines: 21
    '   Blank Lines: 5
    '     File Size: 2.27 KB


    ' Class KOLinks
    ' 
    '     Properties: definition, entry, name, pathways, reactions
    ' 
    '     Function: (+2 Overloads) Build
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB

''' <summary>
''' 这个模型是从蛋白的角度来看待个体和系统模块之间的关系的
''' </summary>
Public Class KOLinks

    ''' <summary>
    ''' KEGG直系同源
    ''' </summary>
    ''' <returns></returns>
    Public Property entry As String
    Public Property name As String
    Public Property definition As String
    Public Property pathways As NamedValue()
    Public Property reactions As String()

    ''' <summary>
    ''' 使用这个函数直接从KEGG的直系同源注释数据转换
    ''' </summary>
    ''' <param name="ko00001">
    ''' <see cref="Orthology"/>
    ''' </param>
    ''' <returns></returns>
    Public Shared Iterator Function Build(ko00001 As IEnumerable(Of Orthology)) As IEnumerable(Of KOLinks)
        For Each xml As Orthology In ko00001.SafeQuery.Where(Function(o) Not o.pathway.IsNullOrEmpty)
            Dim reactions$() = xml.xref.Terms _
                .Where(Function(l) l.name = "RN") _
                .Select(Function(x) x.comment) _
                .ToArray
            Dim pathways As NamedValue() = xml.pathway _
                .Select(Function(x)
                            Return New NamedValue(x.name, x.text.TrimNewLine().Trim)
                        End Function) _
                .ToArray

            Yield New KOLinks With {
                .definition = xml.Definition,
                .entry = xml.Entry,
                .name = xml.Name,
                .pathways = pathways,
                .reactions = reactions
            }
        Next
    End Function

    ''' <summary>
    ''' 使用这个函数直接从KEGG的直系同源注释数据转换
    ''' </summary>
    ''' <param name="ko00001">
    ''' <see cref="Orthology"/>
    ''' </param>
    ''' <returns></returns>
    Public Shared Function Build(ko00001 As String) As IEnumerable(Of KOLinks)
        Return (ls - l - r - "*.XML" <= ko00001) _
            .Select(Function(file) file.LoadXml(Of Orthology)) _
            .DoCall(AddressOf Build)
    End Function
End Class
