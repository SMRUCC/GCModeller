#Region "Microsoft.VisualBasic::33c0d3da7232ff42c50c9cddc98e0978, sub-system\FBA\FBA_Doc\OUTPUT.vb"

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

    '     Class TabularOUT
    ' 
    '         Properties: Flux, Rxn
    ' 
    '         Function: ToString
    ' 
    '     Class ObjectiveFunction
    ' 
    '         Properties: Associates, Coefficient, Comments, Factors, Info
    '                     Name, Result
    ' 
    '         Function: ToString
    ' 
    '         Sub: Add
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace FBA_OUTPUT

    ''' <summary>
    ''' RXN  --> flux result.
    ''' </summary>
    Public Class TabularOUT : Implements INamedValue

        Public Property Rxn As String Implements INamedValue.Key
        Public Property Flux As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' 记录不同的样品之间的FBA目标方程的计算结果的对象
    ''' </summary>
    Public Class ObjectiveFunction

        Public Property Result As KeyValuePairObject(Of String, Double)()
            Get
                If __innerResult Is Nothing Then
                    __innerResult = New List(Of KeyValuePairObject(Of String, Double))
                End If
                Return __innerResult.ToArray
            End Get
            Set(value As KeyValuePairObject(Of String, Double)())
                If value Is Nothing Then
                    __innerResult = New List(Of KeyValuePairObject(Of String, Double))
                Else
                    __innerResult = value.AsList
                End If
            End Set
        End Property

        Public Property Factors As String()
        <XmlAttribute> Public Property Coefficient As Double()
        Public Property Associates As String()
        Public Property Comments As String
        Public Property Name As String
        Public Property Info As String

        Dim __innerResult As List(Of KeyValuePairObject(Of String, Double))

        Public Sub Add(sample As String, result As Double)
            Call __innerResult.Add(sample, result)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
