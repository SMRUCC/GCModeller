#Region "Microsoft.VisualBasic::80194d027a2a3a3d3ee01a08aab1b427, markdown2pdf\PdfConvert\Arguments\Document.vb"

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

    '     Class PdfDocument
    ' 
    '         Properties: Html, LocalConfigMode, Url
    ' 
    '         Function: GetDocument
    ' 
    '     Class HTMLDocument
    ' 
    '         Properties: HTML
    ' 
    '         Function: GetDocument
    ' 
    '     Class PDFContent
    ' 
    '         Properties: footer, globalOptions, header, outline, page
    '                     pagesize, state, TOC
    ' 
    '         Function: (+2 Overloads) DefaultPDFStyle, DefaultStyleDocument, ToString
    ' 
    '     Interface IPDFDocument
    ' 
    '         Properties: HTML
    ' 
    '         Function: GetDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Linq
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Arguments

    ''' <summary>
    ''' 如果需要打印多个网页到一个pdf文件之中，必须要使用这个对象来完成
    ''' 此时，<see cref="PdfDocument.Html"/>属性将不再适用
    ''' </summary>
    Public Class PdfDocument : Inherits PDFContent
        Implements IPDFDocument(Of String)

        Public Property Url As String()
        Public Property Html As String Implements IPDFDocument(Of String).HTML

        ''' <summary>
        ''' 如果这个属性为真，则会将配置参数拷贝给每一个页面
        ''' </summary>
        ''' <returns></returns>
        Public Property LocalConfigMode As Boolean = False

        Public Overrides Function GetDocument() As String Implements IPDFDocument(Of String).GetDocument
            Return Html
        End Function
    End Class

    Public Class HTMLDocument : Inherits PDFContent
        Implements IPDFDocument(Of XElement)

        Public Property HTML As XElement Implements IPDFDocument(Of XElement).HTML

        Public Overrides Function GetDocument() As String Implements IPDFDocument(Of XElement).GetDocument
            Return HTML.ToString
        End Function
    End Class

    Public MustInherit Class PDFContent

        Public Property state As Object

        <Prefix("--header")>
        Public Property header As Decoration
        <Prefix("--footer")>
        Public Property footer As Decoration
        Public Property globalOptions As GlobalOptions
        Public Property outline As Outline
        Public Property page As Page
        Public Property pagesize As PageSize
        Public Property TOC As TOC

#Region "Public methods"
        Public MustOverride Function GetDocument() As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
#End Region

        Public Shared Function DefaultPDFStyle(data$, Optional isUrl As Boolean = True) As PdfDocument
            Return DefaultStyleDocument(Of PdfDocument)(
                Sub(doc)
                    If isUrl Then
                        doc.Url = {data}
                    Else
                        doc.Html = data
                    End If
                End Sub)
        End Function

        Public Shared Function DefaultPDFStyle(html As XElement) As HTMLDocument
            Return DefaultStyleDocument(Of HTMLDocument)(Sub(pdf) pdf.HTML = html)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function DefaultStyleDocument(Of T As {New, PDFContent})(write As Action(Of T)) As T
            With Activator.CreateInstance(Of T)
                Call write(.ByRef)

                ' set default style
                .footer = New Decoration With {
                    .spacing = "10",
                    .fontname = FontFace.MicrosoftYaHei,
                    .fontsize = 11,
                    .line = True,
                    .right = $"{Decoration.page}/{Decoration.topage}"
                }
                .pagesize = New PageSize With {
                    .pagesize = QPrinter.A4
                }
                .TOC = New TOC With {
                    .disabledottedlines = True,
                    .tocheadertext = "[TOC]",
                    .toctextsizeshrink = 0.9
                }
                .header = New Decoration With {
                    .center = Decoration.title,
                    .spacing = 10,
                    .line = True
                }
                .globalOptions = New GlobalOptions With {
                    .marginbottom = 20,
                    .orientation = Orientations.Portrait,
                    .imagequality = 100,
                    .marginleft = 10,
                    .marginright = 30
                }
                .page = New Page With {
                    .debugjavascript = True,
                    .enableforms = True,
                    .javascriptdelay = 1000,
                    .keeprelativelinks = True,
                    .loaderrorhandling = handlers.ignore,
                    .loadmediaerrorhandling = handlers.ignore
                }

                Return .ByRef
            End With
        End Function
    End Class

    Public Interface IPDFDocument(Of T)
        Property HTML As T
        Function GetDocument() As String
    End Interface
End Namespace
