#Region "Microsoft.VisualBasic::fd057f70c970b125dc09404a789b4bd1, analysis\Metagenome\Metagenome\OTUTable\OTUCosineComparer.vb"

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

    '   Total Lines: 39
    '    Code Lines: 30 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (23.08%)
    '     File Size: 1.22 KB


    ' Class OTUCosineComparer
    ' 
    '     Properties: OTU_ids
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetObject, GetSimilarity
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports Cosine = Microsoft.VisualBasic.Math

Public Class OTUCosineComparer : Inherits ComparisonProvider

    ReadOnly OTUs As New Dictionary(Of String, OTUTable)
    ReadOnly sampleids As String()

    Public ReadOnly Property OTU_ids As IEnumerable(Of String)
        Get
            Return OTUs.Keys
        End Get
    End Property

    Public Sub New(OTUs As IEnumerable(Of OTUTable), equals As Double, gt As Double)
        MyBase.New(equals, gt)

        For Each otu As OTUTable In OTUs.SafeQuery
            Call Me.OTUs.Add(otu.ID, otu)
        Next

        sampleids = Me.OTUs.Values.PropertyNames
    End Sub

    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Dim otu1 As OTUTable = OTUs(x)
        Dim otu2 As OTUTable = OTUs(y)
        Dim P As Double() = otu1(sampleids)
        Dim Q As Double() = otu2(sampleids)

        Return Cosine.SSM_SIMD(P, Q)
    End Function

    Public Overrides Function GetObject(id As String) As Object
        Return OTUs(id)
    End Function
End Class

