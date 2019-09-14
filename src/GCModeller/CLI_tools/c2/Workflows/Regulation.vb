#Region "Microsoft.VisualBasic::958ec2f43af8b7dfb57c79cd3274f8e5, CLI_tools\c2\Workflows\Regulation.vb"

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

    ' Class Regulation
    ' 
    '     Properties: BiologicalProcess, Effector, Family, MatchedRegulator, Name
    '                 Pcc, RegpreciseRegulog, Regulation, SameOperon, WGCNAWeight
    ' 
    '     Function: SetSameOperon
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Public Class Regulation : Inherits LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.Html.MEMEHtml.MEMEOutput

    ''' <summary>
    ''' 如果二者在同一个Operon之中，则，本字段为该Operon的值，否则为空。当相同的Operon的时候，此时可能为自调控了
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Same-Operon")> Public Property SameOperon As String

    ''' <summary>
    ''' GeneId Property
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("GeneId")> Public Overrides Property Name As String
        Get
            Return MyBase.Name
        End Get
        Set(value As String)
            MyBase.Name = value
        End Set
    End Property

    <Column("MatchedRegulator")> Public Overloads Property MatchedRegulator As String
    <Column("Family")> Public Property Family As String
    <Column("Effector")> Public Property Effector As String
    <Column("BiologicalProcess")> Public Property BiologicalProcess As String
    <Column("Regulation")> Public Property Regulation As String
    <Column("RegpreciseRegulog")> Public Property RegpreciseRegulog As String
    <Column("WGCNAWeight")> Public Property WGCNAWeight As Double
    <Column("Pcc")> Public Property Pcc As Double

    Friend Function SetSameOperon(DoorOperons As LANS.SystemsBiology.Assembly.Door.OperonView) As Regulation
        SameOperon = DoorOperons.SameOperon(MatchedRegulator, Name)
        Return Me
    End Function
End Class
