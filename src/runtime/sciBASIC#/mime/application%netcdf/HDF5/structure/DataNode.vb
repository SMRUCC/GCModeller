
'
' * Mostly copied from NETCDF4 source code.
' * refer : http://www.unidata.ucar.edu
' * 
' * Modified by iychoi@email.arizona.edu
' 


Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.netCDF.HDF5.IO
Imports BinaryReader = Microsoft.VisualBasic.MIME.application.netCDF.HDF5.IO.BinaryReader

Namespace HDF5.[Structure]

    Public Class DataNode

        Public Shared ReadOnly SIGNATURE As SByte() = New CharStream() From {"T"c, "R"c, "E"c, "E"c}

        Private m_address As Long
        Private m_layout As Layout

        Private m_level As Integer
        Private m_numberOfEntries As Integer
        Private m_currentNode As DataNode

        ' level 0 only
        Private m_entries As List(Of DataChunk)
        ' level > 0 only
        Private m_offsets As Integer()()
        ' int[nentries][ndim]; // other levels
        ' "For raw data chunk nodes, the child pointer is the address of a single raw data chunk"
        Private m_childPointer As Long()
        ' long[nentries];
        Private m_currentEntry As Integer
        ' track iteration; LOOK this seems fishy - why not an iterator ??

        Public Sub New([in] As BinaryReader, sb As Superblock, layout As Layout, address As Long)

            [in].offset = address

            Me.m_address = address
            Me.m_layout = layout
            Dim signature__1 As SByte() = [in].readBytes(4)

            For i As Integer = 0 To 3
                If signature__1(i) <> SIGNATURE(i) Then
                    Throw New IOException("signature is not valid")
                End If
            Next

            Dim type As Integer = [in].readByte()
            Me.m_level = [in].readByte()
            Me.m_numberOfEntries = [in].readShort()

            Dim size As Long = 8 + 2 * sb.sizeOfOffsets + Me.m_numberOfEntries * (8 + sb.sizeOfOffsets + 8 + layout.numberOfDimensions)

            Dim leftAddress As Long = ReadHelper.readO([in], sb)
            Dim rightAddress As Long = ReadHelper.readO([in], sb)

            If Me.m_level = 0 Then
                ' read all entries as a DataChunk
                Me.m_entries = New List(Of DataChunk)()

                For i As Integer = 0 To Me.m_numberOfEntries
                    Dim dc As New DataChunk([in], sb, [in].offset, layout.numberOfDimensions, (i = Me.m_numberOfEntries))
                    Me.m_entries.Add(dc)
                Next
            Else
                ' just track the offsets and node addresses
                Me.m_offsets = MAT(Of Integer)(Me.m_numberOfEntries + 1, layout.numberOfDimensions)
                Me.m_childPointer = New Long(Me.m_numberOfEntries) {}

                For i As Integer = 0 To Me.m_numberOfEntries
                    [in].skipBytes(8)
                    ' skip size, filterMask
                    For j As Integer = 0 To layout.numberOfDimensions - 1
                        Me.m_offsets(i)(j) = CInt([in].readLong())
                    Next

                    Me.m_childPointer(i) = If((i = Me.m_numberOfEntries), -1, ReadHelper.readO([in], sb))
                Next
            End If
        End Sub

        ' this finds the first entry we dont want to skip.
        ' entry i goes from [offset(i),offset(i+1))
        ' we want to skip any entries we dont need, namely those where want >= offset(i+1)
        ' so keep skipping until want < offset(i+1)
        Public Overridable Sub first([in] As BinaryReader, sb As Superblock)
            If Me.m_level = 0 Then

                ' note nentries-1 - assume dont skip the last one
                '                for (currentEntry = 0; currentEntry < nentries-1; currentEntry++) {
                '                	DataChunk entry = myEntries.get(currentEntry + 1);
                '                	if ((wantOrigin == null) || tiling.compare(wantOrigin, entry.offset) < 0) 
                '                		break;   // LOOK ??
                '                } 
                '                

                Me.m_currentEntry = 0
            Else
                Me.m_currentNode = Nothing
                Me.m_currentEntry = 0
                While Me.m_currentEntry < Me.m_numberOfEntries
                    Me.m_currentNode = New DataNode([in], sb, Me.m_layout, Me.m_childPointer(Me.m_currentEntry))
                    Me.m_currentNode.first([in], sb)
                    Exit While
                    Me.m_currentEntry += 1
                End While

                ' heres the case where its the last entry we want; the tiling.compare() above may fail
                If Me.m_currentNode Is Nothing Then
                    Me.m_currentEntry = Me.m_numberOfEntries - 1
                    Me.m_currentNode = New DataNode([in], sb, Me.m_layout, Me.m_childPointer(Me.m_currentEntry))
                    Me.m_currentNode.first([in], sb)
                End If
            End If
        End Sub

        ' LOOK - wouldnt be a bad idea to terminate if possible instead of running through all subsequent entries
        Public Overridable Function hasNext([in] As BinaryReader, sb As Superblock) As Boolean
            If Me.m_level = 0 Then
                Return (Me.m_currentEntry < Me.m_numberOfEntries)
            Else
                If Me.m_currentNode.hasNext([in], sb) Then
                    Return True
                End If

                Return (Me.m_currentEntry < Me.m_numberOfEntries - 1)
            End If
        End Function

        Public Overridable Function [next]([in] As BinaryReader, sb As Superblock) As DataChunk
            If Me.m_level = 0 Then
                Return Me.m_entries(System.Math.Max(System.Threading.Interlocked.Increment(Me.m_currentEntry), Me.m_currentEntry - 1))
            Else
                If Me.m_currentNode.hasNext([in], sb) Then
                    Return Me.m_currentNode.[next]([in], sb)
                End If

                Me.m_currentEntry += 1
                Me.m_currentNode = New DataNode([in], sb, Me.m_layout, Me.m_childPointer(Me.m_currentEntry))
                Me.m_currentNode.first([in], sb)

                Return Me.m_currentNode.[next]([in], sb)
            End If
        End Function
    End Class

End Namespace
