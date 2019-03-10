﻿#Region "Microsoft.VisualBasic::720e51e8038278a132bfd82aacc84ab7, gr\Microsoft.VisualBasic.Imaging\SVG\SVGWriter.vb"

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

    '     Module SVGWriter
    ' 
    '         Function: SVG, (+2 Overloads) WriteSVG
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.SVG.XML
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text

Namespace SVG

    ''' <summary>
    ''' Write the data in <see cref="GraphicsSVG"/> as a xml file.
    ''' </summary>
    Public Module SVGWriter

        Public Const Xmlns$ = "http://www.w3.org/2000/svg"
        Public Const Xlink$ = "http://www.w3.org/1999/xlink"

        ''' <summary>
        ''' Get the current svg model data from current graphics engine.
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="size$">默认是使用<see cref="GraphicsSVG"/>对象的内部大小</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function SVG(g As GraphicsSVG,
                            Optional size$ = Nothing,
                            Optional comment$ = Nothing,
                            Optional desc$ = Nothing) As SVGXml
            Return g.__svgData.GetSVG(size.SizeParser, xmlComment:=comment, desc:=desc)
        End Function

        ''' <summary>
        ''' 将画布<see cref="GraphicsSVG"/>之中的内容写入SVG文件
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="path$">``*.svg``保存的SVG文件的路径</param>
        ''' <returns></returns>
        <Extension> Public Function WriteSVG(g As GraphicsSVG, path$,
                                             Optional size$ = "1440,900",
                                             Optional comments$ = Nothing,
                                             Optional desc$ = Nothing) As Boolean
            Using file As FileStream = path.Open
                Call g.WriteSVG(
                    out:=file,
                    size:=size,
                    comments:=comments,
                    desc:=desc
                )
                Return True
            End Using
        End Function

        <Extension> Public Function WriteSVG(g As GraphicsSVG, out As Stream,
                                             Optional size$ = "1440,900",
                                             Optional comments$ = Nothing,
                                             Optional desc$ = Nothing) As Boolean
            Dim sz As Size = size.SizeParser
            Dim svg As SVGXml = g.__svgData.GetSVG(sz, comments, desc)
            Dim XML$ = svg.GetSVGXml
            Dim bytes As Byte() = TextEncodings.UTF8WithoutBOM.GetBytes(XML)

            Call out.Write(bytes, Scan0, bytes.Length)
            Call out.Flush()

            Return True
        End Function
    End Module
End Namespace
