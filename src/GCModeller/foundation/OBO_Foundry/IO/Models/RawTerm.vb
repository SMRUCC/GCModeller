#Region "Microsoft.VisualBasic::8f0e53130d61d52c030913283520e371, foundation\OBO_Foundry\IO\Models\RawTerm.vb"

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

    '   Total Lines: 53
    '    Code Lines: 34 (64.15%)
    ' Comment Lines: 12 (22.64%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (13.21%)
    '     File Size: 2.01 KB


    '     Structure RawTerm
    ' 
    '         Properties: data, type
    ' 
    '         Function: GetData, GetValueSet, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace IO.Models

    Public Structure RawTerm

        Public Const Key_relationship$ = "relationship"
        Public Const Key_is_a$ = "is_a"
        Public Const Key_xref As String = "xref"
        Public Const Key_name As String = "name"
        Public Const Key_id As String = "id"
        Public Const Key_def As String = "def"
        Public Const Key_synonym As String = "synonym"
        Public Const Key_property_value As String = "property_value"

        ''' <summary>
        ''' Example: ``[Term]``
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String
        ''' <summary>
        ''' 在这里不使用字典是因为为了Xml序列化的考虑
        ''' </summary>
        ''' <returns></returns>
        Public Property data As NamedValue(Of String())()

        ''' <summary>
        ''' Create dictionary table from <see cref="data"/>
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetData() As Dictionary(Of String, String())
            Return data.ToDictionary(Function(x) x.Name,
                                     Function(x)
                                         Return x.Value
                                     End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetValueSet() As Dictionary(Of NamedCollection(Of String))
            Return data _
                .Select(Function(v) New NamedCollection(Of String)(v)) _
                .ToDictionary
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function ExtractBasic() As BasicTerm
            Dim data As Dictionary(Of String, String()) = GetData()

            Return New BasicTerm With {
                .id = data.TryGetValue(Key_id).DefaultFirst,
                .def = data.TryGetValue(Key_def).JoinBy(vbCrLf),
                .name = data.TryGetValue(Key_name).DefaultFirst,
                .[namespace] = data.TryGetValue("namespace").DefaultFirst,
                .is_a = data.TryGetValue(Key_is_a)
            }
        End Function
    End Structure
End Namespace
