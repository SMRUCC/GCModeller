﻿#Region "Microsoft.VisualBasic::79c4fc0e77b1ee2c9d47833398caba1e, engine\BootstrapLoader\Definition\Definition.vb"

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

    '   Total Lines: 150
    '    Code Lines: 108 (72.00%)
    ' Comment Lines: 24 (16.00%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 18 (12.00%)
    '     File Size: 5.34 KB


    '     Enum GeneralCompound
    ' 
    '         DNA, Protein, RNA
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class Definition
    ' 
    '         Properties: ADP, AminoAcid, ATP, GenericCompounds, NucleicAcid
    '                     Oxygen, status, Water
    ' 
    '         Function: GenericEnumerator, GetInfinitySource, KEGG
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace Definitions

    Public Enum GeneralCompound
        DNA
        RNA
        Protein
    End Enum

    ''' <summary>
    ''' The definition for the mass environment for run the simulation
    ''' </summary>
    ''' <remarks>
    ''' 因为物质编号可能会来自于不同的数据库，所以会需要使用这个对象将一些关键的物质映射为计算引擎所能够被识别的对象
    ''' </remarks>
    Public Class Definition : Implements Enumeration(Of String)

#Region "Object maps"

        ' 当初主要是使用这种固定的映射来处理一些特定的模板事件

#Region "转录和翻译"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property ATP As String
        Public Property ADP As String
#End Region

#Region "模板或者说无限来源的物质"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property Water As String
        Public Property Oxygen As String
#End Region

        Public Property NucleicAcid As NucleicAcid
        Public Property AminoAcid As AminoAcid

        Public Property GenericCompounds As Dictionary(Of String, GeneralCompound)
#End Region

        ''' <summary>
        ''' 对细胞的初始状态的定义
        ''' 初始物质浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property status As Dictionary(Of String, Double)

        Public Function GetInfinitySource() As Index(Of String)
            Return {Water, Oxygen}.Where(Function(ref) Not ref Is Nothing).Indexing
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
            If Not ADP.StringEmpty(, True) Then Yield ADP
            If Not ATP.StringEmpty(, True) Then Yield ATP
            If Not Water.StringEmpty(, True) Then Yield Water
            If Not Oxygen.StringEmpty(, True) Then Yield Oxygen

            If Not GenericCompounds Is Nothing Then
                For Each key As String In GenericCompounds.Keys
                    Yield key
                Next
            End If

            If Not status Is Nothing Then
                For Each key As String In status.Keys
                    Yield key
                Next
            End If

            If Not NucleicAcid Is Nothing Then
                If Not NucleicAcid.A.StringEmpty(, True) Then Yield NucleicAcid.A
                If Not NucleicAcid.C.StringEmpty(, True) Then Yield NucleicAcid.C
                If Not NucleicAcid.G.StringEmpty(, True) Then Yield NucleicAcid.G
                If Not NucleicAcid.U.StringEmpty(, True) Then Yield NucleicAcid.U
            End If

            If Not AminoAcid Is Nothing Then
                For Each aa As String In AminoAcid.AsEnumerable
                    If Not aa.StringEmpty(, True) Then
                        Yield aa
                    End If
                Next
            End If
        End Function

        ''' <summary>
        ''' Get the KEGG compound <see cref="Definition"/>
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function KEGG(allCompounds As IEnumerable(Of String), Optional initMass# = 100) As Definition
            Dim initStatus As Dictionary(Of String, Double) = allCompounds _
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
                .Oxygen = "C00007",
                .NucleicAcid = ntBase,
                .AminoAcid = aaResidue,
                .status = initStatus,
                .GenericCompounds = New Dictionary(Of String, GeneralCompound) From {
                    {"C00017", GeneralCompound.Protein},
                    {"C00039", GeneralCompound.DNA},
                    {"C00046", GeneralCompound.RNA}
                }
            }
        End Function
    End Class
End Namespace
