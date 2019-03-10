﻿#Region "Microsoft.VisualBasic::790a80df0bd47057f777c43049d0926d, gr\Microsoft.VisualBasic.Imaging\d3js\scale\ordinal.vb"

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

    '     Class OrdinalScale
    ' 
    '         Function: (+3 Overloads) domain
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting

Namespace d3js.scale

    ''' <summary>
    ''' Unlike continuous scales, ordinal scales have a discrete domain and range. 
    ''' For example, an ordinal scale might map a set of named categories to a 
    ''' set of colors, or determine the horizontal positions of columns in a column 
    ''' chart.
    ''' (相当于cytoscape之中的离散型映射)
    ''' </summary>
    Public Class OrdinalScale : Inherits IScale(Of OrdinalScale)

        Dim factors As Factor(Of String)()
        Dim index As Index(Of String)

        Default Public Overrides ReadOnly Property Value(x As Double) As Double
            Get
                Return Me(x.ToString)
            End Get
        End Property

        Default Public Overrides ReadOnly Property Value(term As String) As Double
            Get
                If Not index.NotExists(term) Then
                    Return factors(index(term)).Value
                Else
                    For Each factor In factors.SeqIterator
                        With factor.value
                            If term < .FactorValue Then
                                If factor.i = 0 Then
                                    Return .Value
                                End If
                                Return (factors(factor.i - 1).Value + .Value) / 2
                            End If
                        End With
                    Next

                    Return factors.Last.Value
                End If
            End Get
        End Property

        Public Overrides Function domain(values As IEnumerable(Of Double)) As OrdinalScale
            Return domain(values.ToStringArray)
        End Function

        ''' <summary>
        ''' If domain is specified, sets the domain to the specified array of values. 
        ''' The first element in domain will be mapped to the first element in the range, 
        ''' the second domain value to the second range value, and so on. Domain values 
        ''' are stored internally in a map from stringified value to index; the resulting 
        ''' index is then used to retrieve a value from the range. Thus, an ordinal scale’s 
        ''' values must be coercible to a string, and the stringified version of the domain 
        ''' value uniquely identifies the corresponding range value. If domain is not specified, 
        ''' this method returns the current domain.
        '''
        ''' Setting the domain On an ordinal scale Is Optional If the unknown value Is implicit 
        ''' (the Default). In this Case, the domain will be inferred implicitly from usage by 
        ''' assigning Each unique value passed To the scale a New value from the range. Note 
        ''' that an explicit domain Is recommended To ensure deterministic behavior, As inferring 
        ''' the domain from usage will be dependent On ordering.
        ''' </summary>
        ''' <param name="values"></param>
        ''' <returns></returns>
        Public Overrides Function domain(values As IEnumerable(Of String)) As OrdinalScale
            factors = values.factors
            index = factors _
                .Select(Function(x) x.FactorValue) _
                .Indexing
            Return Me
        End Function

        Public Overrides Function domain(values As IEnumerable(Of Integer)) As OrdinalScale
            Return domain(values.ToStringArray)
        End Function

        Public Shared Narrowing Operator CType(ordinal As OrdinalScale) As Double()
            Return ordinal._range.ToArray
        End Operator
    End Class
End Namespace
