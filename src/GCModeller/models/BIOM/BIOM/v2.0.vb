#Region "Microsoft.VisualBasic::7e98afeafe1f51fb0d9d08f45c066ce5, GCModeller\models\BIOM\BIOM\v2.0.vb"

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

    '   Total Lines: 59
    '    Code Lines: 36
    ' Comment Lines: 12
    '   Blank Lines: 11
    '     File Size: 2.20 KB


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

Namespace v20

    ''' <summary>
    ''' v2.0 BIOM hdf5 file parser
    ''' </summary>
    <HideModuleName> Public Module HDF5

        ''' <summary>
        ''' Parse v2.0 biom hdf5 file.
        ''' </summary>
        ''' <param name="biom"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' All of the integer/long value that read from hdf5 file will be convert to double type.
        ''' </remarks>
        Public Function ReadFile(biom As String) As v10.BIOMDataSet(Of Double)

            Throw New NotImplementedException

            Dim hdf5 As New HDF5File(biom)
            Dim attributes = hdf5.attributes.AsCharacter.AsVBIdentifier
            Dim data As New v10.BIOMDataSet(Of Double)
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

            If Not version.SequenceEqual({2, 0}) Then
                Throw New InvalidProgramException("Target biom hdf5 file is not in v2.0 version!")
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

            Return data
        End Function
    End Module
End Namespace
