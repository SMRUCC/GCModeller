#Region "Microsoft.VisualBasic::ab7a27b9786d4a7ff11950b710ae2234, visualize\Circos\Circos\Karyotype\Adapters\DOOROperon.vb"

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

    '     Class DOOROperon
    ' 
    '         Properties: Size
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GenerateDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.DOOR

Namespace Karyotype

    ''' <summary>
    ''' 最外层的Ideogram，
    ''' </summary>
    Public Class DOOROperon : Inherits SkeletonInfo

        Sub New(DoorFile As String)
            Dim DOOR As DOOR = DOOR_API.Load(DoorFile)
            Me.bands = New List(Of Band)(GenerateDocument(DOOR))
            Me.Size = bands.Select(Function(x) {x.start, x.end}).IteratesALL.Max
            Call singleKaryotypeChromosome()
        End Sub

        Private Overloads Iterator Function GenerateDocument(DOOR As DOOR) As IEnumerable(Of Band)
            For Each Operon As Operon In DOOR.DOOROperonView
                Dim loci As Integer() = New Integer() {
                    CInt(Operon.InitialX.Location.Left),
                    CInt(Operon.LastGene.Location.Right)
                }
                Yield New Band With {
                    .chrName = "chr1",
                    .bandX = Operon.Key,
                    .bandY = Operon.InitialX.Synonym,
                    .start = loci.Min,
                    .end = loci.Max,
                    .color = "blue"
                }
            Next
        End Function

        Public Overrides ReadOnly Property Size As Integer
    End Class
End Namespace
