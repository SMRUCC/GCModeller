﻿#Region "Microsoft.VisualBasic::53dea019f8bfec97593a9ae1597454b5, gr\Microsoft.VisualBasic.Imaging\SVG\Geometry\GeometryElement.vb"

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

'   Total Lines: 64
'    Code Lines: 52 (81.25%)
' Comment Lines: 1 (1.56%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 11 (17.19%)
'     File Size: 2.65 KB


'     Class GeometryElement
' 
'         Properties: svgElement, X, Y
' 
'         Function: LoadElements
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml
Imports Microsoft.VisualBasic.Imaging.SVG.PathHelper
Imports Microsoft.VisualBasic.Imaging.SVG.XML

Namespace SVG

    Public Class GeometryElement : Implements Layout2D

        Public Property X As Double Implements Layout2D.X
        Public Property Y As Double Implements Layout2D.Y
        Public Property svgElement As SvgElement

        ''' <summary>
        ''' get the xml element tag name
        ''' </summary>
        ''' <returns>
        ''' <see cref="SvgElement.Tag"/>
        ''' </returns>
        Public ReadOnly Property tag As String
            Get
                Return svgElement.Tag
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{tag}@({X},{Y})] {svgElement.ToString}"
        End Function

        Public Function CheckPossibleCircleShape() As Boolean
            ' for a circle shape drawing element:
            ' 1. svg circle element
            ' 2. svg circle path
            ' 
            If tag = "circle" Then
                Return True
            ElseIf tag <> "path" Then
                ' is not a shape element
                Return False
            End If

            Dim path As Command() = Interpreter.ParsePathCommands(DirectCast(svgElement, SvgPath).D).ToArray

            ' signature 1: M A A Z
            If path.Length = 4 AndAlso
                TypeOf path(0) Is M AndAlso
                TypeOf path(1) Is A AndAlso
                TypeOf path(2) Is A AndAlso
                TypeOf path(3) Is Z Then

                Return True
            End If
            ' signature 2: M C C C C
            If path.Length = 6 AndAlso TypeOf path(0) Is M AndAlso
                TypeOf path(1) Is C AndAlso
                TypeOf path(2) Is C AndAlso
                TypeOf path(3) Is C AndAlso
                TypeOf path(4) Is C AndAlso
                TypeOf path(5) Is Z Then

                Return True
            End If

            Return False
        End Function

        Public Shared Iterator Function LoadElements(svg As SvgContainer, offsetX As Double, offsetY As Double) As IEnumerable(Of GeometryElement)
            Dim list As XmlNodeList = svg.GetSvgElement.ChildNodes
            Dim node As XmlElement

            For i As Integer = 0 To list.Count - 1
                node = TryCast(list(i), XmlElement)

                If Not node Is Nothing Then
                    If node.Name = "i:pgf" Then
                        Continue For
                    End If

                    Dim svgElement As SvgElement = SvgElement.Create(node)
                    Dim x As Double = Val(svgElement("x")) + offsetX
                    Dim y As Double = Val(svgElement("y")) + offsetY
                    Dim transform As New Transform(svgElement("transform"))

                    With transform.translate
                        x += .X
                        y += .Y
                    End With

                    If TypeOf svgElement Is SvgGroup Then
                        For Each element As GeometryElement In LoadElements(svgElement, x, y)
                            Yield element
                        Next
                    ElseIf TypeOf svgElement Is SvgText Then
                        Yield New GeometryElement With {
                            .X = x,
                            .Y = y,
                            .svgElement = svgElement
                        }
                    ElseIf TypeOf svgElement Is SvgPath Then
                        Dim op = Interpreter.ParsePathCommands(DirectCast(svgElement, SvgPath).D).ToArray
                        Dim moveTo = op.FirstOrDefault

                        ' move to is nothing means invalid path data
                        If Not moveTo Is Nothing Then
                            Dim M As M = DirectCast(moveTo, M)

                            Yield New GeometryElement With {
                                .svgElement = svgElement,
                                .X = x + M.X,
                                .Y = y + M.Y
                            }
                        End If
                    End If
                End If
            Next
        End Function
    End Class
End Namespace