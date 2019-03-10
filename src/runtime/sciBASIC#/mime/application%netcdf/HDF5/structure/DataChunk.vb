
'
' * Mostly copied from NETCDF4 source code.
' * refer : http://www.unidata.ucar.edu
' * 
' * Modified by iychoi@email.arizona.edu
' 



Imports Microsoft.VisualBasic.MIME.application.netCDF.HDF5.IO

Namespace HDF5.[Structure]

    Public Class DataChunk

        Private m_address As Long
        Private m_size As Integer
        Private m_filterMask As Integer
        Private m_offsets As Integer()
        Private m_filePos As Long

        Friend Sub New([in] As BinaryReader, sb As Superblock, address As Long, numberOfDimensions As Integer, last As Boolean)

            [in].offset = address

            Me.m_address = address
            Me.m_size = [in].readInt()
            Me.m_filterMask = [in].readInt()

            Me.m_offsets = New Integer(numberOfDimensions - 1) {}
            For i As Integer = 0 To numberOfDimensions - 1
                Me.m_offsets(i) = CInt([in].readLong())
            Next

            Me.m_filePos = If(last, -1, ReadHelper.readO([in], sb))
        End Sub

        Public Overridable ReadOnly Property address() As Long
            Get
                Return Me.m_address
            End Get
        End Property

        Public Overridable ReadOnly Property size() As Integer
            Get
                Return Me.m_size
            End Get
        End Property

        Public Overridable ReadOnly Property filterMask() As Integer
            Get
                Return Me.m_filterMask
            End Get
        End Property

        Public Overridable ReadOnly Property offsets() As Integer()
            Get
                Return Me.m_offsets
            End Get
        End Property

        Public Overridable ReadOnly Property filePosition() As Long
            Get
                Return Me.m_filePos
            End Get
        End Property

        Public Overridable Sub printValues()
            Console.WriteLine("DataChunk >>>")
            Console.WriteLine("address : " & Me.m_address)
            Console.WriteLine("size : " & Me.m_size)
            Console.WriteLine("filter mask : " & Me.m_filterMask)
            If Me.m_offsets IsNot Nothing Then
                For i As Integer = 0 To Me.m_offsets.Length - 1
                    Console.WriteLine("offsets[" & i & "] : " & Me.m_offsets(i))
                Next
            End If
            Console.WriteLine("file position : " & Me.m_filePos)

            Console.WriteLine("DataChunk <<<")
        End Sub
    End Class

End Namespace
