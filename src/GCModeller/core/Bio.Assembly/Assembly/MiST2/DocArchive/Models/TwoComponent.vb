#Region "Microsoft.VisualBasic::f92b55d1339792d1229a1f101fc7b5f5, GCModeller\core\Bio.Assembly\Assembly\MiST2\DocArchive\Models\TwoComponent.vb"

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

    '   Total Lines: 76
    '    Code Lines: 38
    ' Comment Lines: 30
    '   Blank Lines: 8
    '     File Size: 2.84 KB


    '     Class TwoComponent
    ' 
    '         Properties: HHK, HisK, HRR, Other, RR
    ' 
    '         Function: get_HisKinase, GetRR, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MiST2

    Public Class TwoComponent

        ''' <summary>
        ''' Histidine kinase
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("HisK")> Public Property HisK As Transducin()
        ''' <summary>
        ''' Hybrid histidine kinase
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("HybridHisK")> Public Property HHK As Transducin()
        ''' <summary>
        ''' Response regulator
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray("RespRegulator")> Public Property RR As Transducin()
        ''' <summary>
        ''' Hybrid response regulator
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("HybridRR")> Public Property HRR As Transducin()
        <XmlArray("Others")> Public Property Other As Transducin()

        ''' <summary>
        ''' 获取所有的双组份系统之中的RR蛋白质的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Function GetRR() As String()
            Dim LQuery = (From transducin As Transducin
                          In {RR, HRR}.ToVector
                          Select transducin.ID
                          Distinct).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 获取所有的双组份系统之中的Hisk蛋白质的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Function get_HisKinase() As String()
            Dim LQuery = (From item In {HisK, HHK}.ToVector Select item.ID Distinct).ToArray
            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("HK: {0}, HHK: {1}, RR: {2}, HRR: {3}, Other: {4}", HisK.Count, HHK.Count, RR.Count, HRR.Count, Other.Count)
        End Function

        Public Shared Narrowing Operator CType(TwoComponent As TwoComponent) As Transducin()
            Dim List As List(Of Transducin) = New List(Of Transducin)
            Call List.AddRange(TwoComponent.HisK)
            Call List.AddRange(TwoComponent.HHK)
            Call List.AddRange(TwoComponent.RR)
            Call List.AddRange(TwoComponent.HRR)
            Call List.AddRange(TwoComponent.Other)

            Return List.ToArray
        End Operator
    End Class
End Namespace
