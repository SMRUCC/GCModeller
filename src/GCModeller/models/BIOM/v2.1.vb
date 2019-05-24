#Region "Microsoft.VisualBasic::85d714db4d4eb7b250db227d502a781b, v2.1.vb"

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

'     Module HDF5
' 
'         Function: ReadFile
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO.HDF5
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder.VBLanguage
Imports Microsoft.VisualBasic.Serialization.JSON

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
        Public Function ReadFile(biom As String) As v10.Json(Of Double)
            Dim hdf5 As New HDF5File(biom)
            Dim attributes = hdf5.attributes.AsCharacter.AsVBIdentifier
            Dim data As New v10.Json(Of Double)
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
            Dim observation_ids = hdf5("/observation/ids")
            Dim observation_data = hdf5("/observation/matrix/data")
            Dim observation_indices = hdf5("/observation/matrix/indices")
            Dim observation_indptr = hdf5("/observation/matrix/indptr")

            Dim sample_ids = hdf5("/sample/ids")
            Dim sample_data = hdf5("/sample/matrix/data")
            Dim sample_indices = hdf5("/sample/matrix/indices")
            Dim sample_indptr = hdf5("/sample/matrix/indptr")

            Dim observation_ids_data = observation_ids.dataset.data(hdf5.superblock)
            Dim observation_data_data = observation_data.dataset.data(hdf5.superblock)
            Dim sample_data_data = sample_data.dataset.data(hdf5.superblock)

            Return data
        End Function
    End Module
End Namespace
