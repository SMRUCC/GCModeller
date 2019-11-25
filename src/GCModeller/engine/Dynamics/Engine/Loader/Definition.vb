#Region "Microsoft.VisualBasic::291c6ebd315e25f639bc026a3bc7f352, engine\Dynamics\Definition.vb"

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

    ' Class Definition
    ' 
    '     Properties: ADP, AminoAcid, ATP, NucleicAcid, status
    '                 Water
    ' 
    ' Class NucleicAcid
    ' 
    '     Properties: A, C, G, U
    ' 
    ' Class AminoAcid
    ' 
    '     Properties: A, C, D, E, F
    '                 G, H, I, K, L
    '                 M, N, O, P, Q
    '                 R, S, T, U, V
    '                 W, Y
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' 因为物质编号可能会来自于不同的数据库，所以会需要使用这个对象将一些关键的物质映射为计算引擎所能够被识别的对象
''' </summary>
Public Class Definition

#Region "Object maps"

    ' 当初主要是使用这种固定的映射来处理一些特定的模板事件

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property ATP As String
    Public Property Water As String
    Public Property ADP As String

    Public Property NucleicAcid As NucleicAcid
    Public Property AminoAcid As AminoAcid
#End Region

    ''' <summary>
    ''' 对细胞的初始状态的定义
    ''' 初始物质浓度
    ''' </summary>
    ''' <returns></returns>
    Public Property status As Dictionary(Of String, Double)

    ''' <summary>
    ''' Get the KEGG compound <see cref="Definition"/>
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function KEGG(allCompounds As IEnumerable(Of String), Optional initMass# = 100) As Definition
        Dim initStatus = allCompounds _
            .ToDictionary(Function(cid) cid,
                            Function(cid)
                                Return initMass
                            End Function)
        Dim ntBase As New NucleicAcid With {
            .A = "C00212",
            .C = "C00475",
            .G = "C00387",
            .U = "C00299"
        }
        Dim aaResidue As New AminoAcid With {
            .A = "C00041",
            .U = "C05688",
            .G = "C00037",
            .C = "C00097",
            .D = "C00049",
            .E = "C00025",
            .F = "C00079",
            .H = "C00135",
            .I = "C00407",
            .K = "C00047",
            .L = "C00123",
            .M = "C00073",
            .N = "C00152",
            .O = "C16138",
            .P = "C00148",
            .Q = "C00064",
            .R = "C00062",
            .S = "C00065",
            .T = "C00188",
            .V = "C00183",
            .W = "C00078",
            .Y = "C00082"
        }

        Return New Definition With {
            .ADP = "C00008",
            .ATP = "C00002",
            .Water = "C00001",
            .NucleicAcid = ntBase,
            .AminoAcid = aaResidue,
            .status = initStatus
        }
    End Function

End Class
