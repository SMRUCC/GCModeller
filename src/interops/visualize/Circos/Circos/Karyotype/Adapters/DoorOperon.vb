#Region "Microsoft.VisualBasic::347c7f0cb3ce5fcae7c6f03483e404ce, ..\interops\visualize\Circos\Circos\Karyotype\Adapters\DoorOperon.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Text
Imports SMRUCC.genomics.Assembly.DOOR
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language

Namespace Karyotype

    ''' <summary>
    ''' 最外层的Ideogram，
    ''' </summary>
    Public Class DOOROperon : Inherits SkeletonInfo

        Sub New(DoorFile As String)
            Dim DOOR As DOOR = DOOR_API.Load(DoorFile)
            Me.__bands = New List(Of Band)(GenerateDocument(DOOR))
            Me.Size = __bands.Select(Function(x) {x.start, x.end}).IteratesALL.Max
            Call __karyotype()
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
