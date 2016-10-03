#Region "Microsoft.VisualBasic::e66290399f7e4a8dd9376d6daa81dc4e, ..\interops\visualize\Circos\Circos\ConfFiles\Nodes\Ideogram.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Configurations.Nodes

    ''' <summary>
    ''' Spacing between ideograms. Suffix "r" denotes a relative value. It
    ''' Is relative To circle circumference (e.g. space Is 0.5% Of
    ''' circumference).
    ''' </summary>
    Public Class Spacing : Inherits CircosDocument
        Implements ICircosDocNode

        <Circos> Public Property [default] As String = "1u"
        <Circos> Public Property break As String = "0u"

        Public Overrides Function Build(IndentLevel As Integer) As String
            Return Me.GenerateCircosDocumentElement("spacing", IndentLevel, Nothing)
        End Function
    End Class

    Public Class Ideogram : Inherits CircosDocument
        Implements ICircosDocNode

        ''' <summary>
        ''' thickness (px) of chromosome ideogram
        ''' 
        ''' Thickness of ideograms, which can be absolute (e.g. pixels, "p"
        ''' suffix) Or relative ("r" suffix). When relative, it Is a fraction Of
        ''' image radius.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property thickness As String = "30p"
        <Circos> Public Property stroke_thickness As String = "0"

        ''' <summary>
        ''' # ideogram border color
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property stroke_color As String = "black"
        ''' <summary>
        ''' Ideograms can be drawn as filled, outlined, or both. When filled,
        ''' the color will be taken from the last field In the karyotype file,
        ''' Or Set by chromosomes_colors. Color names are discussed In
        '''
        ''' http://www.circos.ca/documentation/tutorials/configuration/configuration_files
        '''
        ''' When ``stroke_thickness=0p`` Or If the parameter Is missing, the ideogram Is
        ''' has no outline And the value Of stroke_color Is Not used.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property fill As String = yes

        ''' <summary>
        ''' # the default chromosome color is set here and any value
        ''' # defined in the karyotype file overrides it
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property fill_color As String = "black"

        ''' <summary>
        ''' Fractional radius position of chromosome ideogram within image.
        ''' 
        ''' Spacing between ideograms. Suffix "r" denotes a relative value. It
        ''' Is relative To circle circumference (e.g. space Is 0.5% Of
        ''' circumference).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property radius As String = "0.85r"
        <Circos> Public Property show_label As String = no
        ''' <summary>
        ''' see ``etc/fonts.conf`` for list of font names
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property label_font As String = "default"
        ''' <summary>
        ''' if ideogram radius is constant, and you'd like labels close to image edge, 
        ''' use the ``dims()`` Function To access the size Of the image
        ''' 
        ''' ``
        ''' label_radius  = dims(image,radius) - 60p
        ''' ``
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property label_radius As String = "dims(ideogram,radius) + 0.05r"
        <Circos> Public Property label_size As String = "36"
        <Circos> Public Property label_parallel As String = yes
        <Circos> Public Property label_case As String = "upper"

        ''' <summary>
        ''' # cytogenetic bands
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property band_stroke_thickness As String = "0"

        ''' <summary>
        ''' # show_bands determines whether the outline of cytogenetic bands
        ''' # will be seen
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property show_bands As String = yes

        ''' <summary>
        ''' # in order to fill the bands with the color defined in the karyotype
        ''' # file you must set fill_bands
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property fill_bands As String = yes

        Public Property Spacing As Spacing = New Spacing

        Public Overrides Function Build(IndentLevel As Integer) As String
            Return Me.GenerateCircosDocumentElement("ideogram", IndentLevel + 2, {Spacing})
        End Function
    End Class
End Namespace
