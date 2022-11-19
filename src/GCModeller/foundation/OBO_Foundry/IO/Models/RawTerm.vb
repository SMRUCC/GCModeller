#Region "Microsoft.VisualBasic::5b1f3435ac1f887017e9e4f292a0ed32, GCModeller\foundation\OBO_Foundry\IO\Models\RawTerm.vb"

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

    '   Total Lines: 39
    '    Code Lines: 21
    ' Comment Lines: 12
    '   Blank Lines: 6
    '     File Size: 1.29 KB


    '     Structure RawTerm
    ' 
    '         Properties: data, type
    ' 
    '         Function: GetData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace IO.Models

    Public Structure RawTerm

        Public Const Key_relationship$ = "relationship"
        Public Const Key_is_a$ = "is_a"

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

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
