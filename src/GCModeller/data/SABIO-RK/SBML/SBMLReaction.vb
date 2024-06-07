﻿#Region "Microsoft.VisualBasic::755b2db1ecef5e7c7393bce00adec796, data\SABIO-RK\SBML\SBMLReaction.vb"

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

    '   Total Lines: 40
    '    Code Lines: 29 (72.50%)
    ' Comment Lines: 4 (10.00%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 7 (17.50%)
    '     File Size: 1.24 KB


    '     Class SBMLReaction
    ' 
    '         Properties: ec_number, kineticLaw, kineticLawID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    ''' <summary>
    ''' SBML格式的数据文件之中的reaction模型定义
    ''' </summary>
    ''' 
    <XmlType("reaction")>
    Public Class SBMLReaction : Inherits Reaction

        Public Property kineticLaw As kineticLaw

        Public ReadOnly Property ec_number As String
            Get
                Return annotation.RDF.description.isVersionOf.Bag.list _
                    .Where(Function(li) InStr(li.resource, "ec-code") > 0) _
                    .FirstOrDefault.resource _
                    .Split("/"c) _
                    .Last
            End Get
        End Property

        Public ReadOnly Property kineticLawID As String
            Get
                Dim anno = kineticLaw.annotation?.sabiork

                If kineticLaw.annotation Is Nothing Then
                    Return ""
                ElseIf anno Is Nothing Then
                    Return kineticLaw.annotation.RDF.description.about.Match("\d+")
                Else
                    Return anno.kineticLawID
                End If
            End Get
        End Property

    End Class
End Namespace
