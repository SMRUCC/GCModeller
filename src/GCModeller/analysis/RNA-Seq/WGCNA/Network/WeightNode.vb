#Region "Microsoft.VisualBasic::db0765489c330f499e84c3d9288659c6, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixDatabase\WGCNA\WeightNode.vb"

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

    '     Class Weight
    ' 
    '         Properties: direction, fromAltName, FromNode, toAltName, ToNode
    '                     Weight
    ' 
    '         Function: (+2 Overloads) Find, GetOpposite, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract

Namespace Network

    ''' <summary>
    ''' CytoscapeEdges
    ''' </summary>
    Public Class Weight : Implements IInteraction

        <Column("fromNode")>
        Public Property FromNode As String Implements IInteraction.source
        <Column("toNode")>
        Public Property ToNode As String Implements IInteraction.target
        <Column("weight")>
        Public Property Weight As Double
        Public Property direction As String
        Public Property fromAltName As String
        Public Property toAltName As String

        Public Overrides Function ToString() As String
            Return $"{FromNode} <--> {ToNode};  {Weight}"
        End Function

        ''' <summary>
        ''' Gets the opposite gene in this interaction relationship
        ''' </summary>
        ''' <param name="Regulator"></param>
        ''' <returns></returns>
        Public Function GetOpposite(Regulator As String) As String
            If String.Equals(Regulator, FromNode) Then
                Return ToNode
            ElseIf String.Equals(Regulator, ToNode) Then
                Return FromNode
            Else
                Return ""
            End If
        End Function

        Public Shared Function Find(Id1 As String, Id2 As String, source As Weight()) As Weight
            Dim LQuery = (From wNode As Weight In source.AsParallel
                          Where (String.Equals(Id1, wNode.FromNode) AndAlso
                          String.Equals(Id2, wNode.ToNode)) OrElse
                          (String.Equals(Id2, wNode.FromNode) AndAlso String.Equals(Id1, wNode.ToNode))
                          Select wNode).FirstOrDefault
            Return LQuery
        End Function

        Public Shared Function Find(Id As String, source As Weight()) As Weight()
            Dim LQuery = (From item As Weight
                          In source.AsParallel
                          Where String.Equals(item.FromNode, Id) OrElse
                          String.Equals(item.ToNode, Id)
                          Select item).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
