#Region "Microsoft.VisualBasic::66515c60a5355e0d7ad2e2ef9b1dd678, GCModeller\models\BIOM\BIOM\v2.1.vb"

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

    '   Total Lines: 121
    '    Code Lines: 80
    ' Comment Lines: 25
    '   Blank Lines: 16
    '     File Size: 4.79 KB


    '     Module HDF5
    ' 
    '         Function: matrixRows, observationRows, ReadFile, sampleColumns
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.HDF5
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder.VBLanguage
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.BIOM.v10
Imports SMRUCC.genomics.foundation.BIOM.v10.components

Namespace v21

    ''' <summary>
    ''' V2.1 BIOM hdf5 file parser
    ''' </summary>
    <HideModuleName> Public Module HDF5

        ''' <summary>
        ''' Parse v2.1 biom hdf5 file.
        ''' </summary>
        ''' <param name="biom"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' All of the integer/long value that read from hdf5 file will be convert to double type.
        ''' </remarks>
        Public Function ReadFile(biom As String) As v10.BIOMDataSet(Of Double)
            Dim hdf5 As New HDF5File(biom)
            Dim attributes = hdf5.attributes.AsCharacter.AsVBIdentifier
            Dim data As New v10.BIOMDataSet(Of Double) With {
                .matrix_type = matrix_type.dense,
                .matrix_element_type = "float",
                .comment = "Imports from v2.1 BIOM hdf5 file.",
                .format = "1.0.0"
            }
            Dim version As Integer()

            With attributes
                version = !format_version.LoadJSON(Of Integer())

                data.id = !id
                data.type = !type
                data.format_url = !format_url
                data.date = Date.Parse(!creation_date)
                data.generated_by = !generated_by
                data.shape = !shape.LoadJSON(Of Integer())
            End With

            If Not version.SequenceEqual({2, 1}) Then
                Throw New InvalidProgramException("Target biom hdf5 file is not in v2.1 version!")
            Else
                ' Call hdf5.superblock.CreateFileDump(Console.Out)
            End If

            ' observation/
            ' 相当于row数据
            data.rows = hdf5.observationRows.ToArray
            ' 矩阵数据从observation里面提取
            data.data = hdf5.matrixRows.ToArray

            ' sample/
            ' 相当于column数据
            data.columns = hdf5.sampleColumns.ToArray

            Return data
        End Function

        <Extension>
        Private Iterator Function matrixRows(biom As HDF5File) As IEnumerable(Of Double())
            Dim observation_data As Array = biom("/observation/matrix/data").data
            Dim otuNumbers As Integer = DirectCast(biom("/observation/ids").data, Array).Length
            Dim a As Array

            If observation_data.Rank = 1 Then
                For i As Integer = 0 To otuNumbers - 1
                    Yield {CDbl(observation_data.GetValue(i))}
                Next
            ElseIf observation_data.Rank = 2 Then
                For i As Integer = 0 To otuNumbers - 1
                    a = observation_data.GetValue(i)
                    a = (From x In a Select CDbl(x)).ToArray

                    Yield a
                Next
            Else
                Throw New NotSupportedException
            End If
        End Function

        <Extension>
        Private Iterator Function sampleColumns(biom As HDF5File) As IEnumerable(Of column)
            Dim sample_ids As Array = biom("/sample/ids").data
            ' Dim sample_data = biom("/sample/matrix/data").data
            ' Dim sample_indices = biom("/sample/matrix/indices").data
            ' Dim sample_indptr = biom("/sample/matrix/indptr").data
            ' Dim sample_collapsed_ids = biom("/sample/metadata/collapsed_ids").data

            For i As Integer = 0 To sample_ids.Length - 1
                Yield New column With {
                    .id = sample_ids.GetValue(i)
                }
            Next
        End Function

        <Extension>
        Private Iterator Function observationRows(biom As HDF5File) As IEnumerable(Of row)
            Dim observation_ids As Array = biom("/observation/ids").data
            ' Dim observation_data = biom("/observation/matrix/data").data
            ' Dim observation_indices = biom("/observation/matrix/indices").data
            ' Dim observation_indptr As Integer() = biom("/observation/matrix/indptr").data
            ' 一个otu就是一个taxonomy
            Dim observation_taxonomy As String()() = biom("/observation/metadata/taxonomy").data

            For i As Integer = 0 To observation_ids.Length - 1
                Yield New row With {
                    .id = observation_ids.GetValue(i),
                    .metadata = New meta With {
                        .taxonomy = observation_taxonomy(i)
                    }
                }
            Next
        End Function
    End Module
End Namespace
