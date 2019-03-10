﻿#Region "Microsoft.VisualBasic::218f3256519c21007fa48264ea83c152, models\SBML\SBML\Specifics\MetaCyc\Property\ReaderBase.vb"

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

    '     Class ReaderBase
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __getValue, GetEnumerator, IEnumerable_GetEnumerator
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Model.SBML.Components

Namespace Specifics.MetaCyc

    Public MustInherit Class ReaderBase(Of TMap)
        Implements IEnumerable(Of [Property])

        Protected ReadOnly __source As Dictionary(Of String, [Property])
        Protected ReadOnly __mapsKey As IReadOnlyDictionary(Of TMap, String)

        Sub New(source As IEnumerable(Of [Property]), maps As IReadOnlyDictionary(Of TMap, String))
            __source = source.ToDictionary(Function(x) x.Name)
            __mapsKey = maps
        End Sub

        Protected Function __getValue(key As TMap) As String
            Dim name As String = __mapsKey(key)
            If __source.ContainsKey(name) Then
                Return __source(name).value
            Else
                Return ""
            End If
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of [Property]) Implements IEnumerable(Of [Property]).GetEnumerator
            For Each x In __source
                Yield x.Value
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
