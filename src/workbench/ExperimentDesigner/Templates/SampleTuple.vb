#Region "Microsoft.VisualBasic::20ad113f881487210c06a71b47635740, ..\workbench\ExperimentDesigner\Templates\SampleTuple.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv

''' <summary>
''' Using for paired sample T-test
''' </summary>
<Template(ExperimentDesigner)>
Public Class SampleTuple

    <XmlAttribute> Public Property Sample1 As String
    <XmlAttribute> Public Property Sample2 As String

    ''' <summary>
    ''' Using this 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Label As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Sample1 & "/" & Sample2
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Sample1} vs {Sample2}"
    End Function
End Class

