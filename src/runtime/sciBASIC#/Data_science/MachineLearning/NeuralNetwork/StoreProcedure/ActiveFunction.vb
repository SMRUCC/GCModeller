﻿#Region "Microsoft.VisualBasic::b892bb6ab5ede99150b3757f60d16d4c, Data_science\MachineLearning\NeuralNetwork\StoreProcedure\ActiveFunction.vb"

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

'     Class ActiveFunction
' 
'         Properties: [Function], Arguments, name
' 
'         Function: HasKey, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.Activations
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace NeuralNetwork.StoreProcedure

    ''' <summary>
    ''' 激活函数的存储于XML文档之中的数据模型
    ''' </summary>
    Public Class ActiveFunction

        ''' <summary>
        ''' The function name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property name As String
        ''' <summary>
        ''' 函数对象的构造参数列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 因为无法将字典对象进行Xml序列化, 所以在这里使用键值对集合来表示
        ''' </remarks>
        <XmlElement>
        Public Property Arguments As NamedValue()

        Default Public ReadOnly Property Item(name As String) As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Arguments _
                    .FirstOrDefault(Function(tag) tag.name.TextEquals(name)) _
                   ?.text
            End Get
        End Property

        ''' <summary>
        ''' 通过这个只读属性来得到激活函数的对象模型
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property [Function]() As IActivationFunction
            Get
                With Me
                    Select Case name
                        Case NameOf(Activations.BipolarSigmoid)
                            If HasKey("alpha") Then
                                Return New BipolarSigmoid(!alpha)
                            Else
                                Return New BipolarSigmoid
                            End If
                        Case NameOf(Activations.SigmoidFunction)
                            Return New SigmoidFunction
                        Case NameOf(Activations.Sigmoid)
                            If HasKey("alpha") Then
                                Return New Sigmoid(!alpha)
                            Else
                                Return New Sigmoid
                            End If
                        Case NameOf(Activations.Threshold)
                            Return New Threshold
                        Case NameOf(Activations.ReLU)
                            Return New ReLU
                        Case Else
#If DEBUG Then
                            Call $"Missing model: {name}".Warning
#End If
                            Return New Activations.Sigmoid
                    End Select
                End With
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function HasKey(name As String) As Boolean
            If Arguments Is Nothing Then
                Return False
            Else
                Return Arguments.Any(Function(tag) tag.name.TextEquals(name))
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{name}({Arguments.Select(Function(a) $"{a.name}:={a.text}").JoinBy(", ")})"
        End Function

        ''' <summary>
        ''' 将激活函数从数据模型转换为对象模型
        ''' </summary>
        ''' <param name="af"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(af As ActiveFunction) As IActivationFunction
            Return af.Function
        End Operator

#Region "Default Expressions"
        Public Shared ReadOnly Property ReLU As DefaultValue(Of String) = "ReLU()"
        Public Shared ReadOnly Property Threshold As DefaultValue(Of String) = "Threshold()"
        Public Shared ReadOnly Property Sigmoid As DefaultValue(Of String) = "Sigmoid(alpha:=2.0)"
        Public Shared ReadOnly Property BipolarSigmoid As DefaultValue(Of String) = "BipolarSigmoid(alpha:=2.0)"
#End Region

        ''' <summary>
        ''' 将文本表达式解析为激活函数的模型
        ''' </summary>
        ''' <param name="expression">这个字符串表达式应该是<see cref="IActivationFunction.ToString()"/>的函数输出结果字符串</param>
        ''' <returns></returns>
        Public Shared Function Parse(expression As String) As ActiveFunction
            Dim func As Func = FuncParser.TryParse(expression)

            Return New ActiveFunction With {
                .name = func.Name,
                .Arguments = func.Args _
                    .Select(Function(a) a.GetTagValue(":=")) _
                    .Select(Function(a)
                                Return New NamedValue With {
                                    .name = a.Name,
                                    .text = a.Value
                                }
                            End Function) _
                    .ToArray
            }
        End Function
    End Class
End Namespace
