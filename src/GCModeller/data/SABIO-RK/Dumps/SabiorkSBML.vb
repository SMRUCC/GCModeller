#Region "Microsoft.VisualBasic::9cb1b77a70d8e5f958f03cdfc3612244, GCModeller\data\SABIO-RK\Dumps\SabiorkSBML.vb"

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
    '    Code Lines: 20
    ' Comment Lines: 6
    '   Blank Lines: 6
    '     File Size: 1.15 KB


    '     Class SabiorkSBML
    ' 
    '         Properties: Buffer, CompoundSpecies, Fast, Identifiers, kineticLawID
    '                     LocalParameters, startValuepH, startValueTemperature
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace SBML

    ''' <summary>
    ''' 从sbml文件中的数据解析转换得到的GCModeller的内部数据存储格式
    ''' </summary>
    <XmlType("SABIORK")>
    Public Class SabiorkSBML : Inherits Equation

        ''' <summary>
        ''' http://sabiork.h-its.org/sabioRestWebServices/kineticLaws/123
        ''' </summary>
        Public Const URL_SABIORK_KINETIC_LAWS_QUERY As String = "http://sabiork.h-its.org/sabioRestWebServices/kineticLaws/"

        Public Property kineticLawID As Long
        Public Property startValuepH As Double
        Public Property startValueTemperature As Double
        Public Property Buffer As String
        Public Property Fast As Boolean

        Public Property CompoundSpecies As SBMLParser.CompoundSpecie()
        Public Property Identifiers As String()
        Public Property LocalParameters As [Property]()

        Public Overrides Function ToString() As String
            Return kineticLawID
        End Function
    End Class
End Namespace
