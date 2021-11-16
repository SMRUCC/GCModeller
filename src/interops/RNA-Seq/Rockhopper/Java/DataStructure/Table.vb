#Region "Microsoft.VisualBasic::6fd6655afd24495755945487628a468f, RNA-Seq\Rockhopper\Java\DataStructure\Table.vb"

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

    '     Class Table
    ' 
    '         Properties: loadFactor
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: [get], containsKey, exceedsLoadFactor, getKeyAtIndex, getValueAtIndex
    '                   hash, size
    ' 
    '         Sub: (+2 Overloads) add, Main, remove
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'
' * Copyright 2014 Brian Tjaden
' *
' * This file is part of Rockhopper.
' *
' * Rockhopper is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * any later version.
' *
' * Rockhopper is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' *
' * You should have received a copy of the GNU General Public License
' * (in the file gpl.txt) along with Rockhopper.  
' * If not, see <http://www.gnu.org/licenses/>.
' 


Imports Oracle.Java.util.concurrent.atomic

Namespace Java

    ''' <summary>
    ''' Hashtable implementation. Uses two parallel arrays. 
    ''' Uses open addressing for collision resolution.
    ''' </summary>
    Public Class Table

        Public capacity As Integer
        Private _size As New AtomicInteger()
        ' Size of table
        Private _loadFactor As Double = 0.9
        Private keys As AtomicLongArray
        Private values As AtomicIntegerArray
        Private prime As Long = 16777619
        Private offset As Long = 2166136261
        Private hashPower As Integer = CInt(Math.Pow(2, Assembler.k)) - 1

        Public Sub New()
            If Assembler.CAPACITY_POWER = 25 Then
                capacity = 30000001
            ElseIf Assembler.CAPACITY_POWER >= 31 Then
                ' Max int
                capacity = CInt(Math.Truncate(Math.Pow(2, 31) - 7))
            ElseIf Assembler.CAPACITY_POWER Mod 4 = 0 Then
                capacity = CInt(Math.Truncate(Math.Pow(2, Assembler.CAPACITY_POWER) + 1))
            Else
                capacity = CInt(Math.Truncate(Math.Pow(2, Assembler.CAPACITY_POWER) - 1))
            End If
            keys = New AtomicLongArray(capacity)
            values = New AtomicIntegerArray(capacity)
        End Sub

        ''' <summary>
        ''' If "key" is not in table, add it with "value" of 1.
        ''' If "key" is in table, increment its "value".
        ''' </summary>
        Public Overridable Sub add(key As Long)
            Dim index As Integer = hash(key)
            While values.[Get](index) <> 0
                If keys.[Get](index) = CLng(key) Then
                    values.incrementAndGet(index)
                    Return
                End If
                index = (index + 1) Mod capacity
            End While
            keys.[Set](index, CLng(key))
            values.incrementAndGet(index)
            _size.incrementAndGet()
        End Sub

        ''' <summary>
        ''' Used for adding strand ambiguous reads.
        ''' If "key" is not in table, add it with "value" of 1.
        ''' If "key" is in table, increment its "value".
        ''' </summary>
        Public Overridable Sub add(key As Long, key_RC As Long)
            Dim index As Integer = hash(key)
            While values.[Get](index) <> 0
                If keys.[Get](index) = CLng(key) Then
                    values.incrementAndGet(index)
                    Return
                End If
                index = (index + 1) Mod capacity
            End While
            Dim index_RC As Integer = hash(key_RC)
            While values.[Get](index_RC) <> 0
                If keys.[Get](index_RC) = CLng(key_RC) Then
                    values.incrementAndGet(index_RC)
                    Return
                End If
                index_RC = (index_RC + 1) Mod capacity
            End While
            keys.[Set](index, CLng(key))
            values.incrementAndGet(index)
            _size.incrementAndGet()
        End Sub

        Public Overridable ReadOnly Property loadFactor() As Double
            Get
                Return (_size.[Get]() / CDbl(capacity))
            End Get
        End Property

        Public Overridable Function exceedsLoadFactor() As Boolean
            Return ((_size.[Get]() / CDbl(capacity)) >= _loadFactor)
        End Function

        Public Overridable Function containsKey(key As Long) As Boolean
            Dim index As Integer = hash(key)
            While values.[Get](index) <> 0
                If keys.[Get](index) = CLng(key) Then
                    Return True
                End If
                index = (index + 1) Mod capacity
            End While
            Return False
        End Function

        Public Overridable Function size() As Integer
            Return _size.[Get]()
        End Function

        Public Overridable Function [get](key As Long) As Integer
            Dim index As Integer = hash(key)
            While values.[Get](index) <> 0
                If keys.[Get](index) = CLng(key) Then
                    Return values.[Get](index)
                End If
                index = (index + 1) Mod capacity
            End While
            Return -1
        End Function

        Public Overridable Function getKeyAtIndex(i As Integer) As Long
            Return keys.[Get](i)
        End Function

        Public Overridable Function getValueAtIndex(i As Integer) As Integer
            Return values.[Get](i)
        End Function

        Public Overridable Sub remove(key As Long)
            Dim index As Integer = hash(key)
            While values.[Get](index) <> 0
                If keys.[Get](index) = CLng(key) Then
                    keys.[Set](index, 0)
                    values.[Set](index, 0)
                    _size.decrementAndGet()
                    index = (index + 1) Mod capacity
                    While values.[Get](index) <> 0
                        ' Re-hash
                        Dim index2 As Integer = hash(keys.[Get](index))
                        While (index2 <> index) AndAlso (values.[Get](index2) <> 0)
                            index2 = (index2 + 1) Mod capacity
                        End While
                        If (index2 <> index) AndAlso (values.[Get](index2) = 0) Then
                            keys.[Set](index2, keys.[Get](index))
                            values.[Set](index2, values.[Get](index))
                            keys.[Set](index, 0)
                            values.[Set](index, 0)
                        End If
                        index = (index + 1) Mod capacity
                    End While
                    Return
                End If
                index = (index + 1) Mod capacity
            End While
        End Sub

        Private Function hash(key As Long) As Integer

            ' FNV-1a
            Dim _hash As Long = CLng(offset)
            Dim num As Long = CLng(key)
            For i As Integer = 0 To 1
                _hash = _hash Xor (num And hashPower)
                _hash *= prime
                num = CLng(CULng(num) >> Assembler.k)
            Next
            Dim result As Integer = CInt(_hash) Mod capacity
            If result < 0 Then
                result += capacity
            End If
            Return result
        End Function

        '
        '	private synchronized void rehash() {
        '	double REHASH_FACTOR = 1.5;
        '	if (!exceedsLoadFactor()) return;  // Another thread already rehashed
        '	int capacity2 = (int)(REHASH_FACTOR*capacity + 1.0);
        '	AtomicLongArray keys2 = new AtomicLongArray(capacity2);
        '	AtomicIntegerArray values2 = new AtomicIntegerArray(capacity2);
        '	for (int i=0; i<capacity; i++) {
        '		if (values.get(i) != 0) {
        '		//int i2 = (new Long(keys.get(i))).hashCode() % capacity2;
        '		//if (i2 < 0) i2 += capacity2;
        '		int i2 = hash(new Long(keys.get(i)));
        '		while (values2.get(i2) != 0) i2 = (i2+1) % capacity2;
        '		keys2.set(i2, keys.get(i));
        '		values2.set(i2, values.get(i));
        '		}
        '	}
        '	capacity = capacity2;
        '	keys = keys2;
        '	values = values2;
        '	System.gc();
        '	}
        '	


        Private Shared Sub Main(args As String())

            Dim t As New Table()
            t.add(40)
            t.add(0)
            t.add(10)
            t.add(40)
            t.add(21)
            t.add(30)
            t.add(30)
            t.add(40)
            t.add(30)
            t.add(21)
            t.add(40)
            t.remove(30)

            For i As Integer = 0 To t.capacity - 1
                Console.WriteLine(t.keys.[Get](i) & vbTab & t.values.[Get](i))
            Next
        End Sub

    End Class

End Namespace
