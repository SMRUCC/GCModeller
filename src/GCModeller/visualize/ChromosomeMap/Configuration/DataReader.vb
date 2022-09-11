#Region "Microsoft.VisualBasic::f58d9e7c0fbbf6f52650a83790463ed1, GCModeller\visualize\ChromosomeMap\Configuration\DataReader.vb"

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

    '   Total Lines: 67
    '    Code Lines: 35
    ' Comment Lines: 24
    '   Blank Lines: 8
    '     File Size: 3.02 KB


    '     Class DataReader
    ' 
    ' 
    '         Enum TextAlignment
    ' 
    '             Left, Middle, Right
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: AddLegend, DefaultRNAColor, DeletionMutation, FLAG_HEIGHT, FlagLength
    '                 FunctionAlignment, FunctionAnnotationFont, GeneObjectHeight, IntegrationMutant, LegendFont
    '                 LineHeight, LineLength, LocusTagFont, Margin, NoneCogColor
    '                 Resolution, ribosomalRNAColor, SavedFormat, SecondaryRuleFont, tRNAColor
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Imaging
Imports Microsoft.VisualBasic.Serialization

Namespace Configuration

    ''' <summary>
    ''' 配置数据的数据模型，请使用<see cref="Config.ToConfigurationModel"></see>方法来初始化本数据模型对象，真正所被使用到的内存之中的配置文件的对象模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataReader

        Public Enum TextAlignment
            Left
            Right
            Middle
        End Enum

        ''' <summary> 
        ''' This section will configure the drawing size options
        ''' ----------------------------------------------------
        ''' Due to the GDI+ limitations in the .NET Framework, the image size is limited by your computer memory size, if you want to 
        ''' drawing a very large size image, please running this script on a 64bit platform operating system, or you will get a 
        ''' exception about the GDI+ error: parameter is not valid and then you should try a smaller resolution of the drawing output image.
        ''' Value format: &lt;Width(Integer)&gt;[,&lt;Height(Integer)&gt;]
        ''' Example:
        ''' Both specific the size property: 12000,8000
        ''' Which means the drawing script will generate a image file in resolution of width is value 12000 pixels and image height 
        ''' is 8000 pixels.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <UseCustomMapping>
        Public Property Resolution As Size
        Public Property GeneObjectHeight As Integer
        ''' <summary>
        ''' The chromosome segment length of each line on the chromosome map, this property can effects the drawing model scale factor.
        ''' (在ChromosomeMap之中的每一行基因组片段的长度值，通过这个属性可以影响图形的缩放大小)
        ''' </summary>
        ''' <returns></returns>
        Public Property LineLength As Double
        <MappingsIgnored> Public Property LineHeight As Integer
        Public Property Margin As Integer
        Public Property FlagLength As Integer
        <MappingsIgnored> Public Property FLAG_HEIGHT As String
        Public Property FunctionAlignment As TextAlignment
        Public Property LocusTagFont As Font
        Public Property FunctionAnnotationFont As Font
        Public Property SecondaryRuleFont As Font

        Public Property AddLegend As Boolean
        Public Property LegendFont As Font

#Region "Color Configurations"

        Public Property DeletionMutation As Color
        Public Property IntegrationMutant As Color
        Public Property tRNAColor As Color
        Public Property DefaultRNAColor As Color
        Public Property ribosomalRNAColor As Color
        Public Property NoneCogColor As Color
#End Region

        Public Property SavedFormat As ImageFormat
    End Class
End Namespace
