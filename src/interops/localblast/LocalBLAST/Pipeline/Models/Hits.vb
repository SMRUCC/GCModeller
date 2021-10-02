#Region "Microsoft.VisualBasic::fe3337ab665c978d17782eb8a9724099, localblast\LocalBLAST\Pipeline\Models\Hits.vb"

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

    '     Class HitCollection
    ' 
    '         Properties: description, hits, QueryName
    ' 
    '         Function: getDictionary, GetHitByTagInfo, orderBySp, Take, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace Tasks.Models

    ''' <summary>
    ''' A collection of hits for the target query protein.
    ''' </summary>
    ''' <remarks>
    ''' 其实这个就是相当于一个KEGG里面的SSDB BBH结果文件
    ''' </remarks>
    Public Class HitCollection : Implements INamedValue

        ''' <summary>
        ''' The locus tag of the query protein.(主键蛋白质名称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property QueryName As String Implements INamedValue.Key

        ''' <summary>
        ''' Query protein functional annotation.
        ''' </summary>
        ''' <returns></returns>
        Public Property description As String

        ''' <summary>
        ''' Query hits protein.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property hits As Hit()
            Get
                Return hitTable.Values.ToArray
            End Get
            Set(value As Hit())
                If value.IsNullOrEmpty Then
                    hitTable = New Dictionary(Of Hit)
                Else
                    hitTable = New Dictionary(Of Hit)(getDictionary(value))
                End If
            End Set
        End Property

        Private Shared Function getDictionary(hits As IEnumerable(Of Hit)) As Dictionary(Of String, Hit)
            Return (From hit As Hit
                    In hits
                    Select hit
                    Group hit By hit.hitName Into Group) _
               .ToDictionary(Function(hit) hit.hitName,
                             Function(xhit)
                                 ' 20190705 因为在这里进行了分组和取重复的hit的第一条结果
                                 ' 所以可能会在这里造成数据丢失
                                 Return xhit.Group.First
                             End Function)
        End Function

        Dim hitTable As Dictionary(Of Hit)

        Public Overrides Function ToString() As String
            Return String.Format("{0}:   {1}", QueryName, description)
        End Function

        ''' <summary>
        ''' Gets hits protein tag inform by hit protein locus_tag
        ''' </summary>
        ''' <param name="hitName"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Hit(hitName As String) As Hit
            Get
                If hitTable.ContainsKey(hitName) Then
                    Return hitTable(hitName)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Function GetHitByTagInfo(tag As String) As Hit
            Dim LQuery = From hit As Hit
                         In hits
                         Where String.Equals(hit.tag, tag, StringComparison.OrdinalIgnoreCase)
                         Select hit
            Return LQuery.FirstOrDefault
        End Function

        ''' <summary>
        ''' 按照菌株排序
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Function orderBySp() As HitCollection
            Me.hits = LinqAPI.Exec(Of Hit) <= From hit As Hit
                                              In hitTable.Values
                                              Select hit
                                              Order By hit.tag Ascending
            Return Me
        End Function

        ''' <summary>
        ''' 按照物种编号取出数据构建一个新的bbh集合
        ''' </summary>
        ''' <param name="spTags"></param>
        ''' <returns></returns>
        Public Function Take(spTags As String()) As HitCollection
            Dim LQuery As Hit() =
                LinqAPI.Exec(Of Hit) <= From hitData As Hit
                                        In hits
                                        Where Array.IndexOf(spTags, hitData.tag) > -1
                                        Select hitData

            Return New HitCollection With {
                .hits = LQuery,
                .description = description,
                .QueryName = QueryName
            }
        End Function
    End Class
End Namespace
