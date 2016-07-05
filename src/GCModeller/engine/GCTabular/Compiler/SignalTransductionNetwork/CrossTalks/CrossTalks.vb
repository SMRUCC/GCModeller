#Region "Microsoft.VisualBasic::ce287ad9fb77ca130c88502d3e84609e, ..\GCModeller\engine\GCTabular\Compiler\SignalTransductionNetwork\CrossTalks\CrossTalks.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Assembly

Namespace Compiler.Components

    ''' <summary>
    ''' 双组分系统的蛋白质之间的互作的可能性的高低
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CrossTalks : Implements IInteraction

        <Column("kinase")> Public Property Kinase As String Implements IInteraction.source
        <Column("regulator")> Public Property Regulator As String Implements IInteraction.target
        <Column("probability")> Public Property Probability As Double

        Public Shared Function Trimed(strId As String) As String
            Dim p As Integer = InStr(strId, "(")

            If p > 0 Then
                Return Mid(strId, 1, p - 1)
            Else
                Return strId
            End If
        End Function

        Public Function TrimedKinaseId() As String
            Return Trimed(Kinase)
        End Function

        Public Function TrimedRegulatorId() As String
            Return Trimed(Regulator)
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1};  {2}", Kinase, Regulator, Probability)
        End Function

        Public Function get_InternalGUID() As String
            Return String.Format("{0} ==> {1}", Kinase, Regulator)
        End Function
    End Class
End Namespace

