#Region "Microsoft.VisualBasic::6b4d9cb661b08e0e6195fa0288ed9c0d, analysis\RNA-Seq\WGCNA\Network\WeightNode.vb"

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

    '   Total Lines: 58
    '    Code Lines: 43 (74.14%)
    ' Comment Lines: 8 (13.79%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (12.07%)
    '     File Size: 2.31 KB


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

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph

Namespace Network

    ''' <summary>
    ''' CytoscapeEdges
    ''' </summary>
    Public Class Weight : Implements IInteraction

        <Column("fromNode")>
        Public Property fromNode As String Implements IInteraction.source
        <Column("toNode")>
        Public Property toNode As String Implements IInteraction.target
        <Column("weight")>
        Public Property weight As Double
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

        Public Shared Narrowing Operator CType(node As Weight) As Double
            If node Is Nothing Then
                Return 0
            Else
                Return node.weight
            End If
        End Operator
    End Class
End Namespace
