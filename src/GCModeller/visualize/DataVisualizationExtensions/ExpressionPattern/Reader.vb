#Region "Microsoft.VisualBasic::b55c0bb570fadbf856f778540a718f49, GCModeller\visualize\DataVisualizationExtensions\ExpressionPattern\Reader.vb"

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

'   Total Lines: 77
'    Code Lines: 63
' Comment Lines: 0
'   Blank Lines: 14
'     File Size: 3.05 KB


'     Module Reader
' 
'         Function: readCenter, ReadExpressionPattern
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans
Imports Microsoft.VisualBasic.Linq

Namespace ExpressionPattern

    Public Module Reader

        <Extension>
        Public Function CastAsPatterns(matrix As DataSet()) As ExpressionPattern
            Dim patterns As New List(Of FuzzyCMeansEntity)

            For Each gene As DataSet In matrix
                Dim max_cluster As Integer = gene.Properties _
                    .OrderByDescending(Function(a) a.Value) _
                    .First _
                    .Key _
                    .Match("\d+") _
                    .DoCall(AddressOf Integer.Parse)

                Call patterns.Add(New FuzzyCMeansEntity With {
                    .uid = gene.ID,
                    .cluster = max_cluster,
                    .entityVector = {},
                    .MarkClusterCenter = Color.Black,
                    .memberships = gene.Properties _
                        .ToDictionary(Function(si) Integer.Parse(si.Key.Match("\d+")),
                                      Function(si)
                                          Return si.Value
                                      End Function)
                })
            Next

            Return New ExpressionPattern With {.Patterns = patterns.ToArray}
        End Function

        ''' <summary>
        ''' read the binary data file
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function ReadExpressionPattern(file As Stream) As ExpressionPattern
            Using bin As New BinaryDataReader(file) With {
                .ByteOrder = ByteOrder.BigEndian
            }
                Dim dims As Integer() = bin.ReadInt32s(2)
                Dim nsamples As Integer = bin.ReadInt32
                Dim sampleNames As String() = New String(nsamples - 1) {}

                For i As Integer = 0 To nsamples - 1
                    sampleNames(i) = bin.ReadString(BinaryStringFormat.ZeroTerminated)
                Next

                Dim nCenters As Integer = bin.ReadInt32
                Dim centers As Classify() = New Classify(nCenters - 1) {}

                For i As Integer = 0 To nCenters - 1
                    Dim bufSize As Integer = bin.ReadInt32
                    Dim buf As Byte() = bin.ReadBytes(bufSize)
                    Dim center As Classify = readCenter(buf)

                    centers(i) = center
                Next

                Dim nPatterns As Integer = bin.ReadInt32
                Dim patterns As FuzzyCMeansEntity() = New FuzzyCMeansEntity(nPatterns - 1) {}

                For i As Integer = 0 To nPatterns - 1
                    Dim bufSize As Integer = bin.ReadInt32
                    Dim buf As Byte() = bin.ReadBytes(bufSize)
                    Dim x As FuzzyCMeansEntity = Classify.Load(buf)

                    patterns(i) = x
                Next

                Return New ExpressionPattern With {
                    .centers = centers,
                    .[dim] = dims,
                    .Patterns = patterns,
                    .sampleNames = sampleNames
                }
            End Using
        End Function

        Private Function readCenter(buf As Byte()) As Classify
            Using ms As New MemoryStream(buf), bin As New BinaryDataReader(ms) With {
                .ByteOrder = ByteOrder.BigEndian
            }
                Dim centerId As Integer = bin.ReadInt32
                Dim centerSize As Integer = bin.ReadInt32
                Dim center As Double() = bin.ReadDoubles(centerSize)
                Dim memberSize As Integer = bin.ReadInt32
                Dim members As FuzzyCMeansEntity() = New FuzzyCMeansEntity(memberSize - 1) {}
                Dim bufSize As Integer

                For i As Integer = 0 To memberSize - 1
                    bufSize = bin.ReadInt32
                    buf = bin.ReadBytes(bufSize)
                    members(i) = Classify.Load(buf)
                Next

                Return New Classify With {
                    .members = New List(Of FuzzyCMeansEntity)(members),
                    .center = center,
                    .Id = centerId
                }
            End Using
        End Function
    End Module
End Namespace
