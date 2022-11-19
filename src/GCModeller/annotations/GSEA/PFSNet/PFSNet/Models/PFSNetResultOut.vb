#Region "Microsoft.VisualBasic::d7ce293996c3de2468d3f856e2c61a62, GCModeller\annotations\GSEA\PFSNet\PFSNet\Models\PFSNetResultOut.vb"

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

    '   Total Lines: 43
    '    Code Lines: 15
    ' Comment Lines: 22
    '   Blank Lines: 6
    '     File Size: 1.40 KB


    '     Class PFSNetResultOut
    ' 
    '         Properties: class1, class2, phenotype1, phenotype2, tag
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace DataStructure

    ''' <summary>
    ''' The xml format pfsnet calculation result data.(PfsNET结果Xml文件)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PFSNetResultOut

        ''' <summary>
        ''' The data tag value for the current PfsNET evaluation.(本次计算结果的数据标签)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property tag As String

        Public Property class1 As String
        Public Property class2 As String

        ''' <summary>
        ''' The mutation phenotype 1 evaluation data for the significant sub network.(Class1)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property phenotype1 As PFSNetGraph()
        ''' <summary>
        ''' The another mutation phenotype evaluation data for the significant sub network.(Class2)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property phenotype2 As PFSNetGraph()

        Public Overrides Function ToString() As String
            Return tag
        End Function
    End Class
End Namespace
