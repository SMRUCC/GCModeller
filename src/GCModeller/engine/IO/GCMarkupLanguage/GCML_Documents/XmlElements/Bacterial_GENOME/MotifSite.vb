#Region "Microsoft.VisualBasic::9edade0d91ddeeb0a58a7f87d97b55ab, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Bacterial_GENOME\MotifSite.vb"

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

    '   Total Lines: 32
    '    Code Lines: 15
    ' Comment Lines: 12
    '   Blank Lines: 5
    '     File Size: 1.12 KB


    '     Class MotifSite
    ' 
    '         Properties: MotifName, Regulators, SitePosition
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME
Imports Microsoft.VisualBasic

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    Public Class MotifSite

        ''' <summary>
        ''' 请使用GUID进行赋值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property MotifName As String
        ''' <summary>
        ''' 以ATG为界限的位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("ATG_Distance")> Public Property SitePosition As Integer

        <XmlElement("TF", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/transcript_factor")>
        Public Property Regulators As List(Of SignalTransductions.Regulator)

        Public Overrides Function ToString() As String
            Return MotifName
        End Function
    End Class
End Namespace
