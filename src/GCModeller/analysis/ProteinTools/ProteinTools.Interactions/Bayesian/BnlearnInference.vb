#Region "Microsoft.VisualBasic::f34a3adcaa7ff01c8fae89c989e3bd84, analysis\ProteinTools\ProteinTools.Interactions\Bayesian\BnlearnInference.vb"

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

    ' Class BnlearnInference
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: __R_script, CreateEvidence, CreatePartner
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

''' <summary>
''' 使用已经建立好的计算模型，利用bnlearn包进行推测
''' </summary>
''' <remarks></remarks>
Public Class BnlearnInference : Inherits IRScript

    Dim Evidence, TargetPartner As String

    Sub New(Target As String, PretendPartner As String)
        Me.Evidence = Target
        Me.TargetPartner = PretendPartner
    End Sub

    ''' <summary>
    ''' cpquery返回一个数值，以证据evidence为条件下事件event的条件概率
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function __R_script() As String
        Dim scriptBuilder As StringBuilder = New StringBuilder(4096)

        'cpquery(fitted, (B == "b"), (A == "a"))
        Call scriptBuilder.AppendLine("result <- cpquery(dip_network_model, ")
        Call scriptBuilder.AppendLine(String.Format("     event    = ({0}), ", CreatePartner(Evidence.Length, TargetPartner)))
        Call scriptBuilder.AppendLine(String.Format("     evidence = ({0}))", CreateEvidence(Evidence)))
        Call scriptBuilder.AppendLine("result")

        Return scriptBuilder.ToString
    End Function

    Protected Friend Shared Function CreateEvidence(Evidence As Char()) As String
        Dim sBuilder As StringBuilder = New StringBuilder(10 * 1024)
        For i As Integer = 0 To Evidence.Count - 1
            Call sBuilder.Append(String.Format("(aa_{0} == ""{1}"") & ", i, Evidence(i)))
        Next
        Call sBuilder.Remove(sBuilder.Length - 3, 3)
        Return sBuilder.ToString
    End Function

    Protected Friend Shared Function CreatePartner(EvidenceCounts As Integer, TargetPartner As Char()) As String
        Dim sBuilder As StringBuilder = New StringBuilder(10 * 1024)
        For i As Integer = 0 To TargetPartner.Count - 1
            Call sBuilder.Append(String.Format("(aa_{0} == ""{1}"") & ", i + EvidenceCounts, TargetPartner(i)))
        Next
        Call sBuilder.Remove(sBuilder.Length - 3, 3)
        Return sBuilder.ToString
    End Function
End Class
