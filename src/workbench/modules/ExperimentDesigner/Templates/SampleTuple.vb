﻿#Region "Microsoft.VisualBasic::8dd4c86e80538d2dfee5c082ea81314a, modules\ExperimentDesigner\Templates\SampleTuple.vb"

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

    ' Class SampleTuple
    ' 
    '     Properties: Label, sample1, sample2
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv

''' <summary>
''' Using for paired sample T-test
''' </summary>
<Template(ExperimentDesigner)>
Public Class SampleTuple

    <XmlAttribute> Public Property sample1 As String
    <XmlAttribute> Public Property sample2 As String

    ''' <summary>
    ''' Using this 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Label As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return sample1 & "/" & sample2
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{sample1} vs {sample2}"
    End Function
End Class
