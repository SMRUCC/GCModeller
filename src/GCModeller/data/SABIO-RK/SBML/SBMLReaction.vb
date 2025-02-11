﻿#Region "Microsoft.VisualBasic::4f2b727999e654fafa729eb939fb35ad, data\SABIO-RK\SBML\SBMLReaction.vb"

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

    '   Total Lines: 47
    '    Code Lines: 31 (65.96%)
    ' Comment Lines: 8 (17.02%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 8 (17.02%)
    '     File Size: 1.54 KB


    '     Class SBMLReaction
    ' 
    '         Properties: ec_number, kineticLaw, kineticLawID, metaid
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
    <XmlType("sabiork_reaction", [Namespace]:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class SBMLReaction : Inherits Reaction

        Public Property kineticLaw As kineticLaw

        <XmlAttribute>
        Public Property metaid As String

        ''' <summary>
        ''' get enzyme number that associated with current reaction model
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ec_number As String
            Get
                Return annotation.RDF.description(0).isVersionOf.Bag.list _
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
                    Return kineticLaw.annotation.RDF.description(0).about.Match("\d+")
                Else
                    Return anno.kineticLawID
                End If
            End Get
        End Property

    End Class
End Namespace
