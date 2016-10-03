#Region "Microsoft.VisualBasic::11e6241959e2d117185782a370e38d88, ..\GCModeller\data\GO_gene-ontology\GeneOntology\Files\Obo\Typedef.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.OBO_Foundry

Namespace OBO

    Public Class Typedef : Inherits base

        ''' <summary>
        ''' !通常是有一定含义的字符串， 而不是数字
        ''' </summary>
        ''' <returns></returns>
        <Field("is_anonymous")> Public Property is_anonymous As String
        <Field("alt_id")> Public Property alt_id As String()
        <Field("def")> Public Property def As String
        <Field("comment")> Public Property comment As String
        <Field("subset")> Public Property subset As String()
        <Field("synonym")> Public Property synonym As String()
        <Field("xref")> Public Property xref As String()
        ''' <summary>
        ''' !该关系仅对domain指定术语的亚类起作用
        ''' </summary>
        ''' <returns></returns>
        <Field("domain")> Public Property domain As String
        ''' <summary>
        ''' !任何具有这个关系的术语都属于range指定术语的亚类
        ''' </summary>
        ''' <returns></returns>
        <Field("range")> Public Property range As String
        <Field("is_anti_symmetric")> Public Property is_anti_symmetric As String
        ''' <summary>
        ''' !可否构建循环作用
        ''' </summary>
        ''' <returns></returns>
        <Field("is_cyclic")> Public Property is_cyclic As String
        ''' <summary>
        ''' !是否自反
        ''' </summary>
        ''' <returns></returns>
        <Field("is_reflexive")> Public Property is_reflexive As String
        ''' <summary>
        ''' !是否对称
        ''' </summary>
        ''' <returns></returns>
        <Field("is_symmetric")> Public Property is_symmetric As String
        ''' <summary>
        ''' !传递关系？
        ''' </summary>
        ''' <returns></returns>
        <Field("is_transitive")> Public Property is_transitive As String
        <Field("is_a")> Public Property is_a As String
        ''' <summary>
        ''' !和另一关系相反。适用于原关系的两个术语，可以反方向适用另一关系。
        ''' </summary>
        ''' <returns></returns>
        <Field("inverse_of")> Public Property inverse_of As String
        ''' <summary>
        ''' !将关系传递给下一个
        ''' </summary>
        ''' <returns></returns>
        <Field("transitive_over")> Public Property transitive_over As String
        <Field("relationship")> Public Property relationship As String
        <Field("is_obsolete")> Public Property is_obsolete As String
        <Field("replaced_by")> Public Property replaced_by As String
        <Field("consider")> Public Property consider As String
        <Field("is_metadata_tag")> Public Property is_metadata_tag As String
        <Field("is_class_level")> Public Property is_class_level As String
        <Field("holds_over_chain")> Public Property holds_over_chain As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
