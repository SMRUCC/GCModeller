#Region "Microsoft.VisualBasic::5a9d6e01196472e72828aa0be55f0acc, models\BIOM\v2.1.vb"

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

'     Class Json
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO.HDF5
Imports Microsoft.VisualBasic.Scripting.Runtime

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
            Dim attributes = hdf5.attributes.AsCharacter
            Dim data As New v10.Json(Of Double)

            With attributes
                data.id = !id
                data.type = !type
                data.format_url = !format_url
                data.date = Date.Parse(!creation_date)

            End With

            Return data
        End Function
    End Module
End Namespace
