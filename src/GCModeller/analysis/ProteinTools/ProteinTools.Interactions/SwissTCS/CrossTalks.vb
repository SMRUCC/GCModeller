#Region "Microsoft.VisualBasic::7795a6587d22628f17236813ea5eeee8, analysis\ProteinTools\ProteinTools.Interactions\SwissTCS\CrossTalks.vb"

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

    '     Class CrossTalks
    ' 
    '         Properties: InteractionType, Kinase, Probability, Regulator
    ' 
    '         Function: ToString, Trim, TrimedKinaseId, TrimedRegulatorId
    ' 
    '     Enum TCSComponentTypes
    ' 
    '         kinase, receiver
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract

Namespace SwissTCS

    ''' <summary>
    ''' 双组分系统的蛋白质之间的互作的可能性的高低
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CrossTalks : Implements IInteraction, INetworkEdge
        <Column("kinase")> Public Property Kinase As String Implements IInteraction.target
        <Column("regulator")> Public Property Regulator As String Implements IInteraction.source
        <Column("probability")> Public Property Probability As Double Implements INetworkEdge.value
        Public Property InteractionType As String Implements INetworkEdge.Interaction

        Public Shared Function Trim(sId As String) As String
            Dim p As Integer = InStr(sId, "(")

            If p > 0 Then
                Return Mid(sId, 1, p - 1)
            Else
                Return sId
            End If
        End Function

        Public Function TrimedKinaseId() As String
            Return Trim(Kinase)
        End Function

        Public Function TrimedRegulatorId() As String
            Return Trim(Regulator)
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1};  {2}", Kinase, Regulator, Probability)
        End Function
    End Class

    Public Enum TCSComponentTypes
        kinase
        receiver
    End Enum
End Namespace
