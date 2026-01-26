#Region "Microsoft.VisualBasic::a8b7e01e320f8b2a92f288acb768045a, core\Bio.Annotation\PTF\ProteinAnnotation.vb"

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

    '   Total Lines: 63
    '    Code Lines: 34 (53.97%)
    ' Comment Lines: 21 (33.33%)
    '    - Xml Docs: 95.24%
    ' 
    '   Blank Lines: 8 (12.70%)
    '     File Size: 2.10 KB


    '     Class ProteinAnnotation
    ' 
    '         Properties: attributes, description, geneId, geneName, locus_id
    '                     sequence
    ' 
    '         Function: [get], has, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Namespace Ptf

    ''' <summary>
    ''' A uniy protein annotation model in GCModeller softwares
    ''' </summary>
    Public Class ProteinAnnotation : Implements INamedValue

        ''' <summary>
        ''' A unique symbol id
        ''' </summary>
        ''' <returns></returns>
        Public Property geneId As String Implements INamedValue.Key
        Public Property locus_id As String
        Public Property geneName As String

        ''' <summary>
        ''' full name or description
        ''' </summary>
        ''' <returns></returns>
        Public Property description As String
        ''' <summary>
        ''' usually this property is a collection of gene id in other database
        ''' </summary>
        ''' <returns></returns>
        Public Property attributes As Dictionary(Of String, String())
        Public Property sequence As String

        Default Public Property attr(key As String) As String
            Get
                Return attributes _
                    .TryGetValue(key) _
                    .SafeQuery _
                    .FirstOrDefault
            End Get
            Set(value As String)
                attributes(key) = {value}
            End Set
        End Property

        ''' <summary>
        ''' current protein annotation has the required attribute?
        ''' </summary>
        ''' <param name="attrName"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function has(attrName As String) As Boolean
            Return attributes IsNot Nothing AndAlso
                attributes.ContainsKey(attrName) AndAlso
                Not _attributes(attrName).IsNullOrEmpty
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function [get](attrName As String) As String()
            Return attributes.TryGetValue(attrName)
        End Function

        Public Overrides Function ToString() As String
            Return $"{geneId}: {description}"
        End Function
    End Class
End Namespace
