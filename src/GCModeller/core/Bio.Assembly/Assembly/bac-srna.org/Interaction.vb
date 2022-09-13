#Region "Microsoft.VisualBasic::b3b0b9e0b0bc767459852f900d8927fa, GCModeller\core\Bio.Assembly\Assembly\bac-srna.org\Interaction.vb"

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

    '   Total Lines: 49
    '    Code Lines: 21
    ' Comment Lines: 21
    '   Blank Lines: 7
    '     File Size: 1.61 KB


    '     Structure Interaction
    ' 
    '         Properties: Name, Organism, Reference, Regulation, sRNAid
    '                     TargetName
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Assembly.Bac_sRNA.org

    ''' <summary>
    ''' <see cref="Interaction.sRNAid"/> --> <see cref="Interaction.TargetName"/>
    ''' </summary>
    Public Structure Interaction

        <Column(Name:="sRNAid")> <XmlAttribute>
        Public Property sRNAid As String

        ''' <summary>
        ''' The bacterial organism species name.
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="organism")> <XmlElement>
        Public Property Organism As String
        ''' <summary>
        ''' srna_name
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="srna_name")> <XmlAttribute>
        Public Property Name As String
        <Column(Name:="regulation")> <XmlElement>
        Public Property Regulation As String

        ''' <summary>
        ''' siRNA所作用的目标基因的基因名或者基因号，这个属性总是不会空的
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="target_name")> <XmlElement>
        Public Property TargetName As String

        ''' <summary>
        ''' Reference (PMID)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column(Name:="Reference (PMID)")> <XmlAttribute>
        Public Property Reference As String

        Public Overrides Function ToString() As String
            Return sRNAid
        End Function
    End Structure
End Namespace
