﻿#Region "Microsoft.VisualBasic::ecdf2470c1e0364ff3f3e0478556c5f4, gr\Microsoft.VisualBasic.Imaging\SVG\XML\Xml\SvgElement.vb"

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

    '   Total Lines: 373
    '    Code Lines: 250 (67.02%)
    ' Comment Lines: 79 (21.18%)
    '    - Xml Docs: 88.61%
    ' 
    '   Blank Lines: 44 (11.80%)
    '     File Size: 14.84 KB


    '     Class SvgElement
    ' 
    '         Properties: AriaLabel, Fill, FillOpacity, Id, Stroke
    '                     StrokeDashArray, StrokeLineCap, StrokeOpacity, StrokeWidth, Style
    '                     TabIndex, Tag, Transform, Visible
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AddClass, (+2 Overloads) Create, GetClasses, GetStyle, GetSvgElement
    '                   HasClass, ParseClassAttribute, ParseStyleAttribute, ToString
    ' 
    '         Sub: RemoveClass, SetAttributeData, SetClassAttribute, SetStyle, SetStyleAttribute
    '              ToggleClass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml
Imports Microsoft.VisualBasic.Imaging.SVG.XML.Enums
Imports Microsoft.VisualBasic.MIME.Html.Language.CSS
Imports Microsoft.VisualBasic.Text.Xml

Namespace SVG.XML

    ''' <summary>
    ''' The basically SVG XML document node, it can be tweaks on the style by using CSS
    ''' </summary>
    Public MustInherit Class SvgElement

        Protected ReadOnly Element As XmlElement

        ''' <summary>
        ''' html element node id liked identifer 
        ''' </summary>
        ''' <returns></returns>
        Public Property Id As String
            Get
                Return Element.GetAttribute("id")
            End Get
            Set(value As String)
                Element.SetAttribute("id", value)
            End Set
        End Property

        Public Property TabIndex As Integer?
            Get
                Return Element.GetAttribute("tabindex", CType(Nothing, Integer?))
            End Get
            Set(value As Integer?)
                Element.SetAttribute("tabindex", value)
            End Set
        End Property

        ' TODO Add https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/Presentation

        Public Property Fill As String
            Get
                Return Element.GetAttribute("fill", defaultValue:=Attributes.FillAndStroke.Fill)
            End Get
            Set(value As String)
                Element.SetAttribute("fill", value)
            End Set
        End Property

        Public Property FillOpacity As Double
            Get
                Return Element.GetAttribute("fill-opacity", Attributes.FillAndStroke.FillOpacity)
            End Get
            Set(value As Double)
                Element.SetAttribute("fill-opacity", value)
            End Set
        End Property

        ''' <summary>
        ''' the stroke color, value of this property should be html color code
        ''' </summary>
        ''' <returns></returns>
        Public Property Stroke As String
            Get
                Return Element.GetAttribute("stroke", defaultValue:=Attributes.FillAndStroke.Stroke)
            End Get
            Set(value As String)
                Element.SetAttribute("stroke", value)
            End Set
        End Property

        Public Property StrokeOpacity As Double
            Get
                Return Element.GetAttribute("stroke-opacity", Attributes.FillAndStroke.StrokeOpacity)
            End Get
            Set(value As Double)
                Element.SetAttribute("stroke-opacity", value)
            End Set
        End Property

        Public Property StrokeWidth As Double
            Get
                Return Element.GetAttribute("stroke-width", Attributes.FillAndStroke.StrokeWidth)
            End Get
            Set(value As Double)
                Element.SetAttribute("stroke-width", value)
            End Set
        End Property

        Public Property StrokeLineCap As SvgStrokeLineCap
            Get
                Return Element.GetAttribute(Of SvgStrokeLineCap)("stroke-linecap", Attributes.FillAndStroke.StrokeLineCap)
            End Get
            Set(value As SvgStrokeLineCap)
                Element.SetAttribute("stroke-linecap", value)
            End Set
        End Property

        ''' <summary>
        ''' The stroke-dasharray attribute is a presentation attribute defining the pattern of 
        ''' dashes and gaps used to paint the outline of the shape;
        ''' </summary>
        ''' <returns></returns>
        Public Property StrokeDashArray() As Double()
            Get
                Return Element.GetAttribute("stroke-dasharray", Attributes.FillAndStroke.StrokeDashArray)
            End Get
            Set(value As Double())
                Element.SetAttribute("stroke-dasharray", value.JoinBy(" "))
            End Set
        End Property

        Public Property Transform As String
            Get
                Return Element.GetAttribute("transform")
            End Get
            Set(value As String)
                Element.SetAttribute("transform", value)
            End Set
        End Property

        Public Property Visible As Boolean
            Get
                Return GetStyle("display") <> "none"
            End Get
            Set(value As Boolean)
                SetStyle("display", If(value, String.Empty, "none"))
            End Set
        End Property

        ''' <summary>
        ''' current element node css style
        ''' </summary>
        ''' <returns></returns>
        Public Property Style As String
            Get
                Return Element.GetAttribute("style")
            End Get
            Set(value As String)
                Element.SetAttribute("style", value)
            End Set
        End Property

        ''' <summary>
        ''' The `aria-label` attribute is part of the Accessible Rich Internet Applications (ARIA) specification,
        ''' which is designed to make web content and web applications more accessible to people with disabilities.
        ''' Here's an overview of the `aria-label` attribute:<br />
        ''' 
        ''' **Purpose:**
        ''' - The `aria-label` attribute is used to provide a text label for an element that is not otherwise 
        '''   labeled through text content or other attributes like `aria-labelledby`.
        ''' - It is particularly useful for elements that do not have a visible text label, such as icons,
        '''   buttons, or other interactive elements.<br />
        '''   
        ''' **Usage:**
        ''' - The value of the `aria-label` attribute is a string that contains the text label for the element.
        ''' - This text label is intended to be read by assistive technologies like screen readers, which can 
        '''   announce the label to users who cannot see the screen.<br />
        '''   
        ''' **Example:**
        ''' 
        ''' ```html
        ''' &lt;button aria-label="Close">×&lt;!-- This is an icon representing a close action -->&lt;/button>
        ''' ```
        ''' 
        ''' In this example, a screen reader would announce "Close" when the user navigates to the button, even 
        ''' though the visible content of the button is just an "×" symbol.<br />
        ''' 
        ''' **Benefits:**
        ''' - Improves accessibility by providing a clear description of the element's purpose or content.
        ''' - Allows developers to create more semantic and accessible user interfaces without relying on off-screen 
        '''   text or other less semantic methods.<br />
        '''   
        ''' **Considerations:**
        ''' - The `aria-label` should be concise and descriptive.
        ''' - It should not duplicate text that is already provided in the visible content of the element.
        ''' - Avoid using `aria-label` for elements that already have a text label visible on the screen, as this
        '''   can cause redundancy for screen reader users.<br />
        '''   
        ''' **Compatibility:**
        ''' - The `aria-label` attribute is supported by most modern web browsers and assistive technologies.
        ''' </summary>
        ''' <returns></returns>
        Public Property AriaLabel As String
            Get
                Return Element.GetAttribute("aria-label")
            End Get
            Set(value As String)
                Element.SetAttribute("aria-label", value)
            End Set
        End Property

        ''' <summary>
        ''' get/set of the xml attribute value of current svg element.
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns>The value of the specified attribute. An empty string is returned if a matching
        ''' attribute is not found or if the attribute does not have a specified or default
        ''' value.</returns>
        Default Public Property Attribute(name As String) As String
            Get
                Return Element.GetAttribute(name)
            End Get
            Set(value As String)
                Element.SetAttribute(name, value)
            End Set
        End Property

        ''' <summary>
        ''' get the xml tag name of current svg element
        ''' </summary>
        ''' <returns>
        ''' the tag name string in lower case
        ''' </returns>
        Public ReadOnly Property Tag As String
            Get
                Return Strings.LCase(Element.LocalName)
            End Get
        End Property

        Protected Friend Sub New(element As XmlElement)
            If element Is Nothing Then
                Throw New ArgumentNullException(NameOf(element))
            Else
                Me.Element = element
            End If
        End Sub

        Public Function GetClasses() As IEnumerable(Of String)
            Return ParseClassAttribute()
        End Function

        Public Function HasClass(name As String) As Boolean
            Return GetClasses().Contains(name)
        End Function

        Public Function AddClass(name As String) As SvgElement
            Dim classes = ParseClassAttribute()
            classes.Add(name)
            SetClassAttribute(classes)
            Return Me
        End Function

        Public Sub RemoveClass(name As String)
            Dim classes = ParseClassAttribute()
            classes.Remove(name)
            SetClassAttribute(classes)
        End Sub

        Public Sub ToggleClass(name As String)
            If HasClass(name) Then
                RemoveClass(name)
            Else
                AddClass(name)
            End If
        End Sub

        Protected Function GetStyle(name As String) As String
            Dim styles = ParseStyleAttribute()
            Return styles(name)
        End Function

        Public Sub SetStyle(name As String, value As String)
            Dim styles = ParseStyleAttribute()
            styles(name) = value
            SetStyleAttribute(styles)
        End Sub

        Public Sub SetAttributeData(name As String, value As String)
            If value.StringEmpty Then
                Element.RemoveAttribute(name)
            Else
                Element.SetAttribute(name, value)
            End If
        End Sub

        Public Function GetSvgElement() As XmlElement
            Return Element
        End Function

        Public Overrides Function ToString() As String
            Return Id & $"({AriaLabel})"
        End Function

        Private Function ParseClassAttribute() As HashSet(Of String)
            Return New HashSet(Of String)(Element.GetAttribute("class").Split({" "c}, StringSplitOptions.RemoveEmptyEntries))
        End Function

        Private Sub SetClassAttribute(classes As IEnumerable(Of String))
            If classes Is Nothing OrElse Not classes.Any() Then
                Element.RemoveAttribute("class")
                Return
            End If

            Dim value = String.Join(" ", classes)
            Element.SetAttribute("class", value)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function ParseStyleAttribute() As Dictionary(Of String, String)
            Return CssParser.ParseStyle(Element.GetAttribute("style")).Properties
        End Function

        Private Sub SetStyleAttribute(styles As IReadOnlyDictionary(Of String, String))
            If styles Is Nothing OrElse Not styles.Any() Then
                Element.RemoveAttribute("style")
                Return
            End If

            Dim value = String.Join(";", styles.[Select](Function(kvp) $"{kvp.Key}: {kvp.Value}"))
            Element.SetAttribute("style", value)
        End Sub

        Public Shared Function Create(Of T As SvgElement)(e As XmlElement) As SvgElement
            Select Case GetType(T)
                Case GetType(SvgCircle) : Return New SvgCircle(e)
                Case GetType(SvgRect) : Return New SvgRect(e)
                Case GetType(SvgLine) : Return New SvgLine(e)
                Case GetType(SvgEllipse) : Return New SvgEllipse(e)
                Case GetType(SvgImage) : Return New SvgImage(e)
                Case GetType(SvgPath) : Return New SvgPath(e)
                Case GetType(SvgPolygon) : Return New SvgPolygon(e)
                Case GetType(SvgPolyLine) : Return New SvgPolyLine(e)
                Case GetType(SvgText) : Return New SvgText(e)
                Case GetType(SvgTitle) : Return New SvgTitle(e)

                Case GetType(SvgGroup) : Return New SvgGroup(e)
                Case GetType(SvgClipPath) : Return New SvgClipPath(e)
                Case GetType(SvgMarker) : Return New SvgMarker(e)

                Case Else
                    Throw New NotImplementedException(GetType(T).FullName)
            End Select
        End Function

        ''' <summary>
        ''' create svg element based on the given <see cref="XmlElement.Name"/>
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Public Shared Function Create(e As XmlElement) As SvgElement
            Select Case Strings.LCase(e.Name)
                Case "circle" : Return New SvgCircle(e)
                Case "rect" : Return New SvgRect(e)
                Case "line" : Return New SvgLine(e)
                Case "ellipse" : Return New SvgEllipse(e)
                Case "image" : Return New SvgImage(e)
                Case "path" : Return New SvgPath(e)
                Case "polygon" : Return New SvgPolygon(e)
                Case "polyline" : Return New SvgPolyLine(e)
                Case "text" : Return New SvgText(e)
                Case "title" : Return New SvgTitle(e)

                Case "g" : Return New SvgGroup(e)
                Case "clippath" : Return New SvgClipPath(e)
                Case "marker" : Return New SvgMarker(e)
                Case "defs" : Return New SvgDefs(e)

                Case "style" : Return New SvgStyle(e)
                Case "font" : Return New SvgFont(e)
                Case "font-face" : Return New SvgFontFace(e)
                Case "missing-glyph" : Return New SvgMissingGlyph(e)
                Case "glyph" : Return New SvgGlyph(e)
                Case "metadata" : Return New SvgMetadata(e)
                Case "foreignobject" : Return New SvgForeignObject(e)

                Case "switch" : Return New SvgSwitch(e)

                Case Else
                    Throw New NotImplementedException(e.Name)
            End Select
        End Function
    End Class
End Namespace
