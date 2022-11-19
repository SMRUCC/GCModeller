#Region "Microsoft.VisualBasic::1749e9eaa54bfb9ab966d4b1f7ac54ae, GCModeller\visualize\DataVisualizationExtensions\ExpressionPattern\Writer.vb"

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

    '   Total Lines: 64
    '    Code Lines: 47
    ' Comment Lines: 1
    '   Blank Lines: 16
    '     File Size: 2.17 KB


    '     Module Writer
    ' 
    '         Function: WriteCenter, WriteExpressionPattern
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans

Namespace ExpressionPattern

    Public Module Writer

        Public Function WriteExpressionPattern(pattern As ExpressionPattern, file As Stream) As Boolean
            Using bin As New BinaryDataWriter(file) With {
                .ByteOrder = ByteOrder.BigEndian
            }
                Call bin.Write(pattern.dim)
                Call bin.Write(pattern.sampleNames.Length)
                Call pattern.sampleNames.ForEach(Sub(name, i) Call bin.Write(name, BinaryStringFormat.ZeroTerminated))

                Call bin.Write(pattern.centers.Length)

                For Each center In pattern.centers
                    Dim buf = WriteCenter(center)

                    Call bin.Write(buf.Length)
                    Call bin.Write(buf)
                Next

                ' save patterns
                Call bin.Write(pattern.Patterns.Length)

                For Each data As FuzzyCMeansEntity In pattern.Patterns
                    Dim buf As Byte() = Classify.GetBuffer(data)

                    Call bin.Write(buf.Length)
                    Call bin.Write(buf)
                Next

                Call bin.Flush()
            End Using

            Return True
        End Function

        Private Function WriteCenter(center As Classify) As Byte()
            Using ms As New MemoryStream, bin As New BinaryDataWriter(ms) With {
                .ByteOrder = ByteOrder.BigEndian
            }
                Call bin.Write(center.Id)
                Call bin.Write(center.center.Length)
                Call bin.Write(center.center)
                Call bin.Write(center.members.Count)

                For Each data As FuzzyCMeansEntity In center.members
                    Dim buf As Byte() = Classify.GetBuffer(data)

                    Call bin.Write(buf.Length)
                    Call bin.Write(buf)
                Next

                Call bin.Flush()

                Return ms.ToArray
            End Using
        End Function
    End Module
End Namespace
