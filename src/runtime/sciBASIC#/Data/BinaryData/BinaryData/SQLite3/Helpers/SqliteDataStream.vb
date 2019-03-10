﻿#Region "Microsoft.VisualBasic::2500fce1fd3a6e3ca9acf948a7b8af42, Data\BinaryData\BinaryData\SQLite3\Helpers\SqliteDataStream.vb"

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

    '     Class SqliteDataStream
    ' 
    '         Properties: CanRead, CanSeek, Length, Position
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Read, Seek
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.Internal

Namespace ManagedSqlite.Core.Helpers
    Friend Class SqliteDataStream
        Inherits ReadonlyStream
        Private ReadOnly _reader As ReaderBase
        Private _currentPage As UInteger
        Private _dataOffset As UShort
        Private _dataLengthRemaining As UShort
        Private _nextPage As UInteger

        Private _position As Long
        Private _fullLengthRemaining As Long

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <param name="page">The initial page</param>
        ''' <param name="dataOffset">The offset to the initial data</param>
        ''' <param name="dataLength">The length of the initial data in the initial page</param>
        ''' <param name="overflowPage">The pointer to the first overflow page</param>
        ''' <param name="fullDataSize">The final length of all data in this stream</param>
        Public Sub New(reader As ReaderBase, page As UInteger, dataOffset As UShort, dataLength As UShort, overflowPage As UInteger, fullDataSize As Long)
            _reader = reader
            _currentPage = page
            _dataOffset = dataOffset
            _dataLengthRemaining = dataLength
            _nextPage = overflowPage

            Length = fullDataSize
            _fullLengthRemaining = fullDataSize
        End Sub

        Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
            ' Check if we're EOF
            If _dataLengthRemaining = 0 Then
                Return 0
            End If

            ' Read as much as possible on this page
            Dim canRead As UShort = CUShort(System.Math.Min(count, _dataLengthRemaining))

            _reader.SeekPage(_currentPage, _dataOffset)
            _reader.Read(buffer, offset, canRead)

            ' Current page tracking
            _dataOffset += canRead
            _dataLengthRemaining -= canRead

            ' Global state tracking
            _position += canRead
            _fullLengthRemaining -= canRead

            ' Special case, moving to the next page
            If _dataLengthRemaining = 0 AndAlso _nextPage > 0 Then
                _currentPage = _nextPage

                ' Overflow pages begin with a 4 byte header pointing to the next page
                _reader.SeekPage(_currentPage)
                _nextPage = _reader.ReadUInt32()

                _dataOffset = 4
                _dataLengthRemaining = CUShort(System.Math.Min(_reader.PageSize - _dataOffset - _reader.ReservedSpace, _fullLengthRemaining))
            End If

            Return canRead
        End Function

        Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
            Throw New NotImplementedException()
        End Function

        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return True
            End Get
        End Property
        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property Length() As Long

        Public Overrides Property Position() As Long
            Get
                Return _position
            End Get
            Set
                Throw New NotImplementedException()
            End Set
        End Property
    End Class
End Namespace

