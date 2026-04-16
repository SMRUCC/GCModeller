#Region "Microsoft.VisualBasic::11634cd54e4090cc34732648c06695d7, WebCloud\SMRUCC.WebCloud.QRCode\QRCodeCreator.vb"

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

    ' Class QRCodeCreator
    ' 
    '     Properties: Description, ErrorCorrection, Type, Version
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: [Get], Add, AddErrorCorrection, ChooseParameters, CreateCodeWords
    '               EncodeCharacterCount, EncodeMode, EvaluateMask, EvaluateMicroMask, EvaluateNormalMask
    '               GetAlignmentPatternLocations, GetAvailableErrorCorrectionLevels, GetAvailableModes, GetCharacterCountBits, GetMaxCharacters
    '               GetSymbolDimension, IsFree, Mask, Mul, ToBitmap
    ' 
    '     Sub: [Set], AddFormatInformation, AddVersionInformation, Apply, CreateFreeMask
    '          DrawAlignmentPattern, DrawFinderPattern, DrawHLine, DrawRect, DrawTimingHLine
    '          DrawTimingVLine, DrawVLine, Fill, FillRect, Render
    '          Reserve, Save
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Text
Imports Microsoft.VisualBasic.Text

''' <summary>
''' A QR symbol creator
''' 
''' > https://github.com/jtdubs/QRCode/blob/master/QRCode/QRCode.cs
''' </summary>
Public Class QRCodeCreator

    Dim modules As ModuleType(,)
    Dim accessCount As Integer(,)
    Dim freeMask As Boolean(,)
    Dim [dim] As Integer

#Region "Construction"
    ''' <summary>
    ''' Create a QR symbol that represents the supplied `data'.
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub New(data As String)
        Me.New(data, ErrorCorrection.M, False)
    End Sub

    ''' <summary>
    ''' Create a QR symbol that represents the supplied `data' with the indicated minimum level of error correction.
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub New(data As String, minimumErrorCorrection As ErrorCorrection)
        Me.New(data, minimumErrorCorrection, False)
    End Sub

    ''' <summary>
    ''' Create a QR symbol that represents the supplied `data' with the indicated minimum level of error correction.
    ''' </summary>
    Public Sub New(data As String, minimumErrorCorrection As ErrorCorrection, allowMicroCodes As Boolean)
        Dim mode = ChooseParameters(data, minimumErrorCorrection, allowMicroCodes)
        Dim codeWords = CreateCodeWords(data, mode)
        Dim bits = AddErrorCorrection(codeWords)
        Reserve()
        Fill(bits)
        Dim mask__1 = Mask()
        AddFormatInformation(mask__1)
        AddVersionInformation()
    End Sub
#End Region

#Region "External Interface"
    ''' <summary>
    ''' Type of QR symbol (normal or micro)
    ''' </summary>
    Public Property Type() As SymbolType
    ''' <summary>
    ''' Version of QR symbol (1-5 or 1-40, depending on type)
    ''' </summary>
    Public Property Version() As Integer
    ''' <summary>
    ''' Level of error correction in this symbol
    ''' </summary>
    Public Property ErrorCorrection() As ErrorCorrection
    ''' <summary>
    ''' A textual description of a QR code's metadata
    ''' </summary>
    Public ReadOnly Property Description() As String
        Get
            Select Case Type
                Case SymbolType.Micro
                    If Version = 1 Then
                        Return [String].Format("QR M{0}", Version)
                    Else
                        Return [String].Format("QR M{0}-{1}", Version, ErrorCorrection)
                    End If
                Case SymbolType.Normal
                    Return [String].Format("QR {0}-{1}", Version, ErrorCorrection)
            End Select

            Throw New InvalidOperationException()
        End Get
    End Property

    ''' <summary>
    ''' Save the QR code as an image at the following scale.
    ''' </summary>
    ''' <param name="imagepath">Path of image file to create.</param>
    ''' <param name="scale">Size of a module, in pixels.</param>
    Public Sub Save(imagePath As String, scale As Integer)
        Using b As Bitmap = ToBitmap(scale)
            b.Save(imagePath, ImageFormat.Png)
        End Using
    End Sub

    ''' <summary>
    ''' Generate a bitmap of this QR code at the following scale.
    ''' </summary>
    ''' <param name="scale"></param>
    ''' <returns></returns>
    Public Function ToBitmap(scale As Integer) As Bitmap
        Dim b As New Bitmap([dim] * scale, [dim] * scale)

        Using g As Graphics = Graphics.FromImage(b)
            Render(g, scale)
        End Using

        Return b
    End Function

    ''' <summary>
    ''' Render this bitmap to the supplied Graphics object at the indicated scale.
    ''' </summary>
    ''' <param name="scale"></param>
    Public Sub Render(g As Graphics, scale As Integer)
        Dim brush = New SolidBrush(Color.Black)

        g.Clear(Color.White)

        For x As Integer = 0 To [dim] - 1
            For y As Integer = 0 To [dim] - 1
                If [Get](x, y) = ModuleType.Dark Then
                    g.FillRectangle(brush, x * scale, y * scale, scale, scale)
                End If
            Next
        Next
    End Sub
#End Region

#Region "Steps"
    ''' <summary>
    ''' Choose suitable values for Type, Version, ErrorCorrection and Mode.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    Private Function ChooseParameters(data As String, minimumErrorCorrection As ErrorCorrection, allowMicroCodes As Boolean) As Mode
        ' get list of error correction modes at least as good as the user-specified one
        Dim allowedErrorCorrectionModes = {
            ErrorCorrection.None,
            ErrorCorrection.L,
            ErrorCorrection.M,
            ErrorCorrection.Q,
            ErrorCorrection.H
        }.SkipWhile(Function(e) e <> minimumErrorCorrection) _
         .ToList()

        ' get the tightest-fit encoding mode
        Dim tightestMode As Mode

        If data.All(Function(c) [Char].IsDigit(c)) Then
            tightestMode = Mode.Numeric
        ElseIf data.All(Function(c) ASCII.AlphaNumericTable.ContainsKey(c)) Then
            tightestMode = Mode.AlphaNumeric
        Else
            tightestMode = Mode.[Byte]
        End If

        ' get list of allowed encoding modes
        Dim allowedModes = New Mode() {Mode.Numeric, Mode.AlphaNumeric, Mode.[Byte]}.SkipWhile(Function(m) m <> tightestMode).ToList()
        ' get list of possible types
        Dim possibleTypes As List(Of (SymbolType, Byte))

        If allowMicroCodes Then
            possibleTypes = Enumerable.Concat(Enumerable.Range(1, 4).[Select](Function(i) (SymbolType.Micro, CByte(i))), Enumerable.Range(1, 40).[Select](Function(i) (SymbolType.Normal, CByte(i)))).ToList()
        Else
            possibleTypes = Enumerable.Range(1, 40).[Select](Function(i) (SymbolType.Normal, CByte(i))).ToList()
        End If

        ' for each type in ascending order of size
        For Each p As (SymbolType, Byte) In possibleTypes
            ' for each error correction level from most to least
            For Each e In allowedErrorCorrectionModes.Intersect(GetAvailableErrorCorrectionLevels(p.Item1, p.Item2)).Reverse()
                ' lookup the data capacity
                Dim capacityEntry = DataCapacityTable.First(Function(f) f.Item1 = p.Item1 AndAlso f.Item2 = p.Item2 AndAlso f.Item3 = e).Item4

                ' for each encoding mode from tightest to loosest
                For Each m In allowedModes.Intersect(GetAvailableModes(p.Item1, p.Item2))
                    Dim capacity As Integer = 0

                    Select Case m
                        Case Mode.Numeric
                            capacity = capacityEntry.Item2
                            Exit Select
                        Case Mode.AlphaNumeric
                            capacity = capacityEntry.Item3
                            Exit Select
                        Case Mode.[Byte]
                            capacity = capacityEntry.Item4
                            Exit Select
                        Case Else
                            capacity = 0
                            Exit Select
                    End Select

                    ' if there is enough room, we've found our solution
                    If capacity >= data.Length Then
                        Type = p.Item1
                        Version = p.Item2
                        ErrorCorrection = e
                        Return m
                    End If
                Next
            Next
        Next

        Throw New InvalidOperationException("no suitable parameters found")
    End Function

    ''' <summary>
    ''' Encode the data in the following mode, pad, and return final code words.
    ''' </summary>
    ''' <param name="data">The data to encode.</param>
    ''' <returns>The fully-encoded data.</returns>
    Private Function CreateCodeWords(data As String, mode__1 As Mode) As Byte()
        '#Region "Code word creation"
        ' encode data as series of bit arrays
        Dim bits As New List(Of BitArray)()

        ' add mode indicator
        bits.Add(EncodeMode(mode__1))

        ' add character count
        bits.Add(EncodeCharacterCount(mode__1, data.Length))

        ' perform mode-specific data encoding
        Select Case mode__1
            Case Mode.[Byte]
                If True Then
                    ' retrieve UTF8 encoding of data
                    bits.Add(Encoding.UTF8.GetBytes(data).ToBitArray())
                End If
                Exit Select

            Case Mode.Numeric
                If True Then
                    Dim idx As Integer

                    ' for every triple of digits
                    For idx = 0 To data.Length - 3 Step 3
                        ' encode them as a 3-digit decimal number
                        Dim x As Integer = ASCII.AlphaNumericTable(data(idx)) * 100 + ASCII.AlphaNumericTable(data(idx + 1)) * 10 + ASCII.AlphaNumericTable(data(idx + 2))
                        bits.Add(x.ToBitArray(10))
                    Next

                    ' if there is a remaining pair of digits
                    If idx < data.Length - 1 Then
                        ' encode them as a 2-digit decimal number
                        Dim x As Integer = ASCII.AlphaNumericTable(data(idx)) * 10 + ASCII.AlphaNumericTable(data(idx + 1))
                        idx += 2
                        bits.Add(x.ToBitArray(7))
                    End If

                    ' if there is a remaining digit
                    If idx < data.Length Then
                        ' encode it as a decimal number
                        Dim x As Integer = ASCII.AlphaNumericTable(data(idx))
                        idx += 1
                        bits.Add(x.ToBitArray(4))
                    End If
                End If
                Exit Select

            Case Mode.AlphaNumeric
                If True Then
                    Dim idx As Integer

                    ' for every pair of characters
                    For idx = 0 To data.Length - 2 Step 2
                        ' encode them as a single number
                        Dim x As Integer = ASCII.AlphaNumericTable(data(idx)) * 45 + ASCII.AlphaNumericTable(data(idx + 1))
                        bits.Add(x.ToBitArray(11))
                    Next

                    ' if there is a remaining character
                    If idx < data.Length Then
                        ' encode it as a number
                        Dim x As Integer = ASCII.AlphaNumericTable(data(idx))
                        bits.Add(x.ToBitArray(6))
                    End If
                End If
                Exit Select
        End Select

        ' add the terminator mode marker
        bits.Add(EncodeMode(Mode.Terminator))

        ' calculate the bitstream's total length, in bits
        Dim bitstreamLength As Integer = bits.Sum(Function(b) b.Length)

        ' check the full capacity of symbol, in bits
        Dim capacity As Integer = DataCapacityTable.First(Function(f) f.Item1 = Type AndAlso f.Item2 = Version AndAlso f.Item3 = ErrorCorrection).Item4.Item1 * 8

        ' M1 and M3 are actually shorter by 1 nibble
        If Type = SymbolType.Micro AndAlso (Version = 3 OrElse Version = 1) Then
            capacity -= 4
        End If

        ' pad the bitstream to the nearest octet boundary with zeroes
        If bitstreamLength < capacity AndAlso bitstreamLength Mod 8 <> 0 Then
            Dim paddingLength As Integer = Math.Min(8 - (bitstreamLength Mod 8), capacity - bitstreamLength)
            bits.Add(New BitArray(paddingLength))
            bitstreamLength += paddingLength
        End If

        ' fill the bitstream with pad codewords
        Dim padCodewords As Byte() = New Byte() {&H37, &H88}
        Dim padIndex As Integer = 0
        While bitstreamLength < (capacity - 4)
            bits.Add(New BitArray(New Byte() {padCodewords(padIndex)}))
            bitstreamLength += 8
            padIndex = (padIndex + 1) Mod 2
        End While

        ' fill the last nibble with zeroes (only necessary for M1 and M3)
        If bitstreamLength < capacity Then
            bits.Add(New BitArray(4))
            bitstreamLength += 4
        End If

        ' flatten list of bitarrays into a single bool[]
        Dim flattenedBits As Boolean() = New Boolean(bitstreamLength - 1) {}
        Dim bitIndex As Integer = 0
        For Each b In bits
            b.CopyTo(flattenedBits, bitIndex)
            bitIndex += b.Length
        Next

        Return New BitArray(flattenedBits).ToByteArray()
    End Function

    ''' <summary>
    ''' Generate error correction words and interleave with code words.
    ''' </summary>
    ''' <param name="codeWords"></param>
    ''' <returns></returns>
    Private Function AddErrorCorrection(codeWords As Byte()) As BitArray
        Dim dataBlocks As New List(Of Byte())()
        Dim eccBlocks As New List(Of Byte())()

        ' generate error correction words
        Dim ecc = ErrorCorrectionTable.First(Function(f) f.Item1 = Type AndAlso f.Item2 = Version AndAlso f.Item3 = ErrorCorrection).Item4
        Dim dataIndex As Integer = 0

        ' for each collection of blocks that are needed
        For Each e In ecc
            ' lookup number of data words and error words in this block
            Dim dataWords As Integer = e.Item3
            Dim errorWords As Integer = e.Item2 - e.Item3

            ' retrieve the appropriate polynomial for the desired error word count
            Dim poly = Polynomials(errorWords).ToArray()

            ' for each needed block
            For b As Integer = 0 To e.Item1 - 1
                ' add the block's data to the final list
                dataBlocks.Add(codeWords.Skip(dataIndex).Take(dataWords).ToArray())
                dataIndex += dataWords

                ' pad the block with zeroes
                Dim temp = Enumerable.Concat(dataBlocks.Last(), Enumerable.Repeat(CByte(0), errorWords)).ToArray()

                ' perform polynomial division to calculate error block
                For start As Integer = 0 To dataWords - 1
                    Dim pow As Byte = LogTable(temp(start))
                    For i As Integer = 0 To poly.Length - 1
                        temp(i + start) = temp(i + start) Xor ExponentTable(Mul(poly(i), pow))
                    Next
                Next

                ' add error block to the final list
                eccBlocks.Add(temp.Skip(dataWords).ToArray())
            Next
        Next
        '#End Region

        ' generate final data sequence
        Dim sequence As Byte() = New Byte(dataBlocks.Sum(Function(b) b.Length) + (eccBlocks.Sum(Function(b) b.Length) - 1)) {}
        Dim finalIndex As Integer = 0

        ' interleave the data blocks
        For i As Integer = 0 To dataBlocks.Max(Function(b) b.Length) - 1
            Dim index% = i

            For Each b In dataBlocks.Where(Function(block) block.Length > index)
                sequence(finalIndex) = b(i)
                finalIndex += 1
            Next
        Next

        ' interleave the error blocks
        For i As Integer = 0 To eccBlocks.Max(Function(b) b.Length) - 1
            Dim index% = i

            For Each b In eccBlocks.Where(Function(block) block.Length > index)
                sequence(finalIndex) = b(i)
                finalIndex += 1
            Next
        Next

        Return sequence.ToBitArray()
    End Function

    ''' <summary>
    ''' Perform the following steps
    ''' - Draw finder patterns
    ''' - Draw alignment patterns
    ''' - Draw timing lines
    ''' - Reserve space for version and format information
    ''' - Mark remaining space as "free" for data
    ''' </summary>
    Private Sub Reserve()
        [dim] = GetSymbolDimension()

        ' initialize to a full symbol of unaccessed, light modules
        freeMask = New Boolean([dim] - 1, [dim] - 1) {}
        accessCount = New Integer([dim] - 1, [dim] - 1) {}
        modules = New ModuleType([dim] - 1, [dim] - 1) {}
        For x As Integer = 0 To [dim] - 1
            For y As Integer = 0 To [dim] - 1
                modules(x, y) = ModuleType.Light
                accessCount(x, y) = 0
                freeMask(x, y) = True
            Next
        Next

        ' draw alignment patterns
        For Each location In GetAlignmentPatternLocations()
            ' check for overlap with top-left finder pattern
            If location.Item1 < 10 AndAlso location.Item2 < 10 Then
                Continue For
            End If

            ' check for overlap with bottom-left finder pattern
            If location.Item1 < 10 AndAlso location.Item2 > ([dim] - 10) Then
                Continue For
            End If

            ' check for overlap with top-right finder pattern
            If location.Item1 > ([dim] - 10) AndAlso location.Item2 < 10 Then
                Continue For
            End If

            DrawAlignmentPattern(location.Item1, location.Item2)
        Next

        ' draw top-left finder pattern
        DrawFinderPattern(3, 3)
        ' and border
        DrawHLine(0, 7, 8, ModuleType.Light)
        DrawVLine(7, 0, 7, ModuleType.Light)

        Select Case Type
            Case SymbolType.Micro
                ' draw top-left finder pattern's format area
                DrawHLine(1, 8, 8, ModuleType.Light)
                DrawVLine(8, 1, 7, ModuleType.Light)

                ' draw timing lines
                DrawTimingHLine(8, 0, [dim] - 8)
                DrawTimingVLine(0, 8, [dim] - 8)
                Exit Select

            Case SymbolType.Normal
                ' draw top-left finder pattern's format area
                DrawHLine(0, 8, 9, ModuleType.Light)
                DrawVLine(8, 0, 8, ModuleType.Light)

                ' draw top-right finder pattern
                DrawFinderPattern([dim] - 4, 3)
                ' and border
                DrawHLine([dim] - 8, 7, 8, ModuleType.Light)
                DrawVLine([dim] - 8, 0, 7, ModuleType.Light)
                ' and format area
                DrawHLine([dim] - 8, 8, 8, ModuleType.Light)

                ' draw bottom-left finder pattern
                DrawFinderPattern(3, [dim] - 4)
                ' and border
                DrawHLine(0, [dim] - 8, 8, ModuleType.Light)
                DrawVLine(7, [dim] - 7, 7, ModuleType.Light)
                ' and format area
                DrawVLine(8, [dim] - 7, 7, ModuleType.Light)
                ' and dark module
                [Set](8, [dim] - 8, ModuleType.Dark)

                ' draw timing lines
                DrawTimingHLine(8, 6, [dim] - 8 - 8)
                DrawTimingVLine(6, 8, [dim] - 8 - 8)

                If Version >= 7 Then
                    ' reserve version information areas
                    FillRect(0, [dim] - 11, 6, 3, ModuleType.Light)
                    FillRect([dim] - 11, 0, 3, 6, ModuleType.Light)
                End If
                Exit Select
        End Select

        ' mark non-accessed cells as free, accessed cells as reserved
        CreateFreeMask()
    End Sub

    ''' <summary>
    ''' Populate the "free" modules with data.
    ''' </summary>
    ''' <param name="bits"></param>
    Private Sub Fill(bits As BitArray)
        ' start with bit 0, moving up
        Dim idx As Integer = 0
        Dim up As Boolean = True

        Dim minX As Integer = If(Type = SymbolType.Normal, 0, 1)
        Dim minY As Integer = If(Type = SymbolType.Normal, 0, 1)

        Dim timingX As Integer = If(Type = SymbolType.Normal, 6, 0)
        Dim timingY As Integer = If(Type = SymbolType.Normal, 6, 0)

        ' from right-to-left
        For x As Integer = [dim] - 1 To minX Step -2
            ' skip over the vertical timing line
            If x = timingX Then
                x -= 1
            End If

            ' in the indicated direction
            Dim y As Integer = (If(up, [dim] - 1, minY))
            While y >= minY AndAlso y < [dim]
                ' for each horizontal pair of modules
                For dx As Integer = 0 To -2 + 1 Step -1
                    ' if the module is free (not reserved)
                    If IsFree(x + dx, y) Then
                        ' if data remains to be written
                        If idx < bits.Length Then
                            ' write the next bit
                            [Set](x + dx, y, If(bits(idx), ModuleType.Dark, ModuleType.Light))
                        Else
                            ' pad with light cells
                            [Set](x + dx, y, ModuleType.Light)
                        End If

                        ' advance to the next bit
                        idx += 1
                    End If
                Next
                y += (If(up, -1, 1))
            End While

            ' reverse directions
            up = Not up
        Next
    End Sub

    ''' <summary>
    ''' Identify and apply the best mask
    ''' </summary>
    ''' <returns></returns>
    Private Function Mask() As Byte
        Dim masks As List(Of Tuple(Of Byte, Byte, Func(Of Integer, Integer, Boolean))) = Nothing

        ' determine which mask types are applicable
        Select Case Type
            Case SymbolType.Micro
                masks = DataMaskTable.Where(Function(m) m.Item2 <> 255).ToList()
                Exit Select

            Case SymbolType.Normal
                masks = DataMaskTable.ToList()
                Exit Select
        End Select

        ' evaluate all the maks
        Dim results = masks.[Select](Function(m) (m, EvaluateMask(m.Item3)))

        ' choose a winner
        Dim winner As Tuple(Of Byte, Byte, Func(Of Integer, Integer, Boolean))
        If Type = SymbolType.Normal Then
            winner = results.OrderBy(Function(t) t.Item2).First().Item1
        Else
            ' lowest penalty wins
            winner = results.OrderBy(Function(t) t.Item2).Last().Item1
        End If
        ' highest score wins
        ' apply the winner
        Call Apply(winner.Item3)

        ' return the winner's ID
        Return If(Type = SymbolType.Normal, winner.Item1, winner.Item2)
    End Function

    ''' <summary>
    ''' Write the format information (version and mask id)
    ''' </summary>
    ''' <param name="maskID"></param>
    Private Sub AddFormatInformation(maskID As Byte)
        If Type = SymbolType.Normal Then
            Dim bits = NormalFormatStrings.First(Function(f) f.Item1 = ErrorCorrection AndAlso f.Item2 = maskID).Item3

            ' add format information around top-left finder pattern
            [Set](8, 0, If(bits(14), ModuleType.Dark, ModuleType.Light))
            [Set](8, 1, If(bits(13), ModuleType.Dark, ModuleType.Light))
            [Set](8, 2, If(bits(12), ModuleType.Dark, ModuleType.Light))
            [Set](8, 3, If(bits(11), ModuleType.Dark, ModuleType.Light))
            [Set](8, 4, If(bits(10), ModuleType.Dark, ModuleType.Light))
            [Set](8, 5, If(bits(9), ModuleType.Dark, ModuleType.Light))
            [Set](8, 7, If(bits(8), ModuleType.Dark, ModuleType.Light))
            [Set](8, 8, If(bits(7), ModuleType.Dark, ModuleType.Light))
            [Set](7, 8, If(bits(6), ModuleType.Dark, ModuleType.Light))
            [Set](5, 8, If(bits(5), ModuleType.Dark, ModuleType.Light))
            [Set](4, 8, If(bits(4), ModuleType.Dark, ModuleType.Light))
            [Set](3, 8, If(bits(3), ModuleType.Dark, ModuleType.Light))
            [Set](2, 8, If(bits(2), ModuleType.Dark, ModuleType.Light))
            [Set](1, 8, If(bits(1), ModuleType.Dark, ModuleType.Light))
            [Set](0, 8, If(bits(0), ModuleType.Dark, ModuleType.Light))

            ' add format information around top-right finder pattern
            [Set]([dim] - 1, 8, If(bits(14), ModuleType.Dark, ModuleType.Light))
            [Set]([dim] - 2, 8, If(bits(13), ModuleType.Dark, ModuleType.Light))
            [Set]([dim] - 3, 8, If(bits(12), ModuleType.Dark, ModuleType.Light))
            [Set]([dim] - 4, 8, If(bits(11), ModuleType.Dark, ModuleType.Light))
            [Set]([dim] - 5, 8, If(bits(10), ModuleType.Dark, ModuleType.Light))
            [Set]([dim] - 6, 8, If(bits(9), ModuleType.Dark, ModuleType.Light))
            [Set]([dim] - 7, 8, If(bits(8), ModuleType.Dark, ModuleType.Light))
            [Set]([dim] - 8, 8, If(bits(7), ModuleType.Dark, ModuleType.Light))

            ' add format information around bottom-left finder pattern
            [Set](8, [dim] - 7, If(bits(6), ModuleType.Dark, ModuleType.Light))
            [Set](8, [dim] - 6, If(bits(5), ModuleType.Dark, ModuleType.Light))
            [Set](8, [dim] - 5, If(bits(4), ModuleType.Dark, ModuleType.Light))
            [Set](8, [dim] - 4, If(bits(3), ModuleType.Dark, ModuleType.Light))
            [Set](8, [dim] - 3, If(bits(2), ModuleType.Dark, ModuleType.Light))
            [Set](8, [dim] - 2, If(bits(1), ModuleType.Dark, ModuleType.Light))
            [Set](8, [dim] - 1, If(bits(0), ModuleType.Dark, ModuleType.Light))
        Else
            Dim bits = MicroFormatStrings.First(Function(f) f.Item1 = Version AndAlso f.Item2 = ErrorCorrection AndAlso f.Item3 = maskID).Item4

            ' add format information around top-left finder pattern
            [Set](8, 1, If(bits(14), ModuleType.Dark, ModuleType.Light))
            [Set](8, 2, If(bits(13), ModuleType.Dark, ModuleType.Light))
            [Set](8, 3, If(bits(12), ModuleType.Dark, ModuleType.Light))
            [Set](8, 4, If(bits(11), ModuleType.Dark, ModuleType.Light))
            [Set](8, 5, If(bits(10), ModuleType.Dark, ModuleType.Light))
            [Set](8, 6, If(bits(9), ModuleType.Dark, ModuleType.Light))
            [Set](8, 7, If(bits(8), ModuleType.Dark, ModuleType.Light))
            [Set](8, 8, If(bits(7), ModuleType.Dark, ModuleType.Light))
            [Set](7, 8, If(bits(6), ModuleType.Dark, ModuleType.Light))
            [Set](6, 8, If(bits(5), ModuleType.Dark, ModuleType.Light))
            [Set](5, 8, If(bits(4), ModuleType.Dark, ModuleType.Light))
            [Set](4, 8, If(bits(3), ModuleType.Dark, ModuleType.Light))
            [Set](3, 8, If(bits(2), ModuleType.Dark, ModuleType.Light))
            [Set](2, 8, If(bits(1), ModuleType.Dark, ModuleType.Light))
            [Set](1, 8, If(bits(0), ModuleType.Dark, ModuleType.Light))
        End If
    End Sub

    ''' <summary>
    ''' Write the version information
    ''' </summary>
    Private Sub AddVersionInformation()
        If Type = SymbolType.Micro OrElse Version < 7 Then
            Return
        End If

        Dim bits = VersionStrings(Version)

        ' write top-right block
        Dim idx = 17
        For y As Integer = 0 To 5
            For x As Integer = [dim] - 11 To [dim] - 9
                [Set](x, y, If(bits(idx), ModuleType.Dark, ModuleType.Light))
                idx -= 1
            Next
        Next

        ' write bottom-left block
        idx = 17
        For x As Integer = 0 To 5
            For y As Integer = [dim] - 11 To [dim] - 9
                [Set](x, y, If(bits(idx), ModuleType.Dark, ModuleType.Light))
                idx -= 1
            Next
        Next
    End Sub
#End Region

#Region "Drawing Helpers"
    Private Sub DrawFinderPattern(centerX As Integer, centerY As Integer)
        DrawRect(centerX - 3, centerY - 3, 7, 7, ModuleType.Dark)
        DrawRect(centerX - 2, centerY - 2, 5, 5, ModuleType.Light)
        FillRect(centerX - 1, centerY - 1, 3, 3, ModuleType.Dark)
    End Sub

    Private Sub DrawAlignmentPattern(centerX As Integer, centerY As Integer)
        DrawRect(centerX - 2, centerY - 2, 5, 5, ModuleType.Dark)
        DrawRect(centerX - 1, centerY - 1, 3, 3, ModuleType.Light)
        [Set](centerX, centerY, ModuleType.Dark)
    End Sub

    Private Sub FillRect(left As Integer, top As Integer, width As Integer, height As Integer, type As ModuleType)
        For dx As Integer = 0 To width - 1
            For dy As Integer = 0 To height - 1
                [Set](left + dx, top + dy, type)
            Next
        Next
    End Sub

    Private Sub DrawRect(left As Integer, top As Integer, width As Integer, height As Integer, type As ModuleType)
        DrawHLine(left, top, width, type)
        DrawHLine(left, top + height - 1, width, type)
        DrawVLine(left, top + 1, height - 2, type)
        DrawVLine(left + width - 1, top + 1, height - 2, type)
    End Sub

    Private Sub DrawHLine(x As Integer, y As Integer, length As Integer, type As ModuleType)
        For dx As Integer = 0 To length - 1
            [Set](x + dx, y, type)
        Next
    End Sub

    Private Sub DrawVLine(x As Integer, y As Integer, length As Integer, type As ModuleType)
        For dy As Integer = 0 To length - 1
            [Set](x, y + dy, type)
        Next
    End Sub

    Private Sub DrawTimingHLine(x As Integer, y As Integer, length As Integer)
        For dx As Integer = 0 To length - 1
            [Set](x + dx, y, If(((x + dx) Mod 2 = 0), ModuleType.Dark, ModuleType.Light))
        Next
    End Sub

    Private Sub DrawTimingVLine(x As Integer, y As Integer, length As Integer)
        For dy As Integer = 0 To length - 1
            [Set](x, y + dy, If(((y + dy) Mod 2 = 0), ModuleType.Dark, ModuleType.Light))
        Next
    End Sub

    Private Sub [Set](x As Integer, y As Integer, type As ModuleType)
        modules(x, y) = type
        accessCount(x, y) += 1
    End Sub

    Private Function [Get](x As Integer, y As Integer) As ModuleType
        Return modules(x, y)
    End Function

    Private Sub CreateFreeMask()
        For x As Integer = 0 To [dim] - 1
            For y As Integer = 0 To [dim] - 1
                freeMask(x, y) = accessCount(x, y) = 0
            Next
        Next
    End Sub

    Private Function IsFree(x As Integer, y As Integer) As Boolean
        Return freeMask(x, y)
    End Function
#End Region

#Region "Masking Helpers"
    Private Function EvaluateMask(mask As Func(Of Integer, Integer, Boolean)) As Integer
        ' apply the mask
        Apply(mask)

        Try
            If Type = SymbolType.Normal Then
                Return EvaluateNormalMask()
            Else
                Return EvaluateMicroMask()
            End If
        Finally
            ' undo the mask
            Apply(mask)
        End Try
    End Function

    Private Sub Apply(mask As Func(Of Integer, Integer, Boolean))
        For x As Integer = 0 To [dim] - 1
            For y As Integer = 0 To [dim] - 1
                If IsFree(x, y) AndAlso mask(y, x) Then
                    [Set](x, y, If([Get](x, y) = ModuleType.Dark, ModuleType.Light, ModuleType.Dark))
                End If
            Next
        Next
    End Sub

    Private Function EvaluateMicroMask() As Integer
        Dim darkCount1 As Integer = Enumerable.Range(1, [dim] - 2).Count(Function(x) [Get](x, [dim] - 1) = ModuleType.Dark)
        Dim darkCount2 As Integer = Enumerable.Range(1, [dim] - 2).Count(Function(y) [Get]([dim] - 1, y) = ModuleType.Dark)

        Return Math.Min(darkCount1, darkCount2) * 16 + Math.Max(darkCount1, darkCount2)
    End Function

    Private Function EvaluateNormalMask() As Integer
        Dim penalty As Integer = 0

        ' horizontal adjacency penalties
        For y As Integer = 0 To [dim] - 1
            Dim last As ModuleType = [Get](0, y)
            Dim count As Integer = 1

            For x As Integer = 1 To [dim] - 1
                Dim m = [Get](x, y)
                If m = last Then
                    count += 1
                Else
                    If count >= 5 Then
                        penalty += count - 2
                    End If

                    last = m
                    count = 1
                End If
            Next

            If count >= 5 Then
                penalty += count - 2
            End If
        Next

        ' vertical adjacency penalties
        For x As Integer = 0 To [dim] - 1
            Dim last As ModuleType = [Get](x, 0)
            Dim count As Integer = 1

            For y As Integer = 1 To [dim] - 1
                Dim m = [Get](x, y)
                If m = last Then
                    count += 1
                Else
                    If count >= 5 Then
                        penalty += count - 2
                    End If

                    last = m
                    count = 1
                End If
            Next

            If count >= 5 Then
                penalty += count - 2
            End If
        Next

        ' block penalties
        For x As Integer = 0 To [dim] - 2
            For y As Integer = 0 To [dim] - 2
                Dim m = [Get](x, y)

                If m = [Get](x + 1, y) AndAlso m = [Get](x, y + 1) AndAlso m = [Get](x + 1, y + 1) Then
                    penalty += 3
                End If
            Next
        Next

        ' horizontal finder pattern penalties
        For y As Integer = 0 To [dim] - 1
            For x As Integer = 0 To [dim] - 12
                If [Get](x + 0, y) = ModuleType.Dark AndAlso [Get](x + 1, y) = ModuleType.Light AndAlso [Get](x + 2, y) = ModuleType.Dark AndAlso [Get](x + 3, y) = ModuleType.Dark AndAlso [Get](x + 4, y) = ModuleType.Dark AndAlso [Get](x + 5, y) = ModuleType.Light AndAlso [Get](x + 6, y) = ModuleType.Dark AndAlso [Get](x + 7, y) = ModuleType.Light AndAlso [Get](x + 8, y) = ModuleType.Light AndAlso [Get](x + 9, y) = ModuleType.Light AndAlso [Get](x + 10, y) = ModuleType.Light Then
                    penalty += 40
                End If

                If [Get](x + 0, y) = ModuleType.Light AndAlso [Get](x + 1, y) = ModuleType.Light AndAlso [Get](x + 2, y) = ModuleType.Light AndAlso [Get](x + 3, y) = ModuleType.Light AndAlso [Get](x + 4, y) = ModuleType.Dark AndAlso [Get](x + 5, y) = ModuleType.Light AndAlso [Get](x + 6, y) = ModuleType.Dark AndAlso [Get](x + 7, y) = ModuleType.Dark AndAlso [Get](x + 8, y) = ModuleType.Dark AndAlso [Get](x + 9, y) = ModuleType.Light AndAlso [Get](x + 10, y) = ModuleType.Dark Then
                    penalty += 40
                End If
            Next
        Next

        ' vertical finder pattern penalties
        For x As Integer = 0 To [dim] - 1
            For y As Integer = 0 To [dim] - 12
                If [Get](x, y + 0) = ModuleType.Dark AndAlso [Get](x, y + 1) = ModuleType.Light AndAlso [Get](x, y + 2) = ModuleType.Dark AndAlso [Get](x, y + 3) = ModuleType.Dark AndAlso [Get](x, y + 4) = ModuleType.Dark AndAlso [Get](x, y + 5) = ModuleType.Light AndAlso [Get](x, y + 6) = ModuleType.Dark AndAlso [Get](x, y + 7) = ModuleType.Light AndAlso [Get](x, y + 8) = ModuleType.Light AndAlso [Get](x, y + 9) = ModuleType.Light AndAlso [Get](x, y + 10) = ModuleType.Light Then
                    penalty += 40
                End If

                If [Get](x, y + 0) = ModuleType.Light AndAlso [Get](x, y + 1) = ModuleType.Light AndAlso [Get](x, y + 2) = ModuleType.Light AndAlso [Get](x, y + 3) = ModuleType.Light AndAlso [Get](x, y + 4) = ModuleType.Dark AndAlso [Get](x, y + 5) = ModuleType.Light AndAlso [Get](x, y + 6) = ModuleType.Dark AndAlso [Get](x, y + 7) = ModuleType.Dark AndAlso [Get](x, y + 8) = ModuleType.Dark AndAlso [Get](x, y + 9) = ModuleType.Light AndAlso [Get](x, y + 10) = ModuleType.Dark Then
                    penalty += 40
                End If
            Next
        Next

        ' ratio penalties
        Dim total As Integer = [dim] * [dim]
        Dim darkCount As Integer = 0

        For x As Integer = 0 To [dim] - 1
            For y As Integer = 0 To [dim] - 1
                If [Get](x, y) = ModuleType.Dark Then
                    darkCount += 1
                End If
            Next
        Next

        Dim percentDark As Integer = darkCount * 100 \ total
        Dim up As Integer = If((percentDark Mod 5 = 0), percentDark, percentDark + (5 - (percentDark Mod 5)))
        Dim down As Integer = If((percentDark Mod 5 = 0), percentDark, percentDark - (percentDark Mod 5))
        up = Math.Abs(up - 50)
        down = Math.Abs(down - 50)
        up /= 5
        down /= 5
        penalty += Math.Min(up, down) * 10

        Return penalty
    End Function
#End Region

#Region "Calculation Helpers"
    Private Function GetSymbolDimension() As Integer
        Select Case Type
            Case SymbolType.Micro
                Return 9 + (2 * Version)
            Case SymbolType.Normal
                Return 17 + (4 * Version)
        End Select

        Throw New InvalidOperationException()
    End Function

    Private Iterator Function GetAlignmentPatternLocations() As IEnumerable(Of (Integer, Integer))
        Select Case Type
            Case SymbolType.Micro
                Exit Select

            Case SymbolType.Normal
                Dim locations = AlignmentPatternLocations(Version)
                For i As Integer = 0 To locations.Length - 1
                    For j As Integer = i To locations.Length - 1
                        Yield (locations(i), locations(j))

                        If i <> j Then
                            Yield (locations(j), locations(i))
                        End If
                    Next
                Next
                Exit Select
            Case Else

                Throw New InvalidOperationException()
        End Select
    End Function

    Private Function GetAvailableModes(type As SymbolType, version As Integer) As IEnumerable(Of Mode)
        Select Case type
            Case SymbolType.Normal
                Return NormalModes

            Case SymbolType.Micro
                Return MicroModes(version)
            Case Else

                Throw New InvalidOperationException()
        End Select
    End Function

    Private Function GetAvailableErrorCorrectionLevels(type As SymbolType, version As Integer) As IEnumerable(Of ErrorCorrection)
        Select Case type
            Case SymbolType.Normal
                Return NormalErrorCorrectionLevels

            Case SymbolType.Micro
                Return MicroErrorCorrectionLevels(version)
            Case Else

                Throw New InvalidOperationException()
        End Select
    End Function

    Public Function EncodeMode(mode As Mode) As BitArray
        Select Case Type
            Case SymbolType.Normal
                Return NormalModeEncodings(CInt(mode))

            Case SymbolType.Micro
                Return MicroModeEncodings.First(Function(t) t.Item1 = Version AndAlso t.Item2 = mode).Item3
        End Select

        Throw New InvalidOperationException()
    End Function

    Private Function EncodeCharacterCount(mode As Mode, count As Integer) As BitArray
        Dim bits As Integer = GetCharacterCountBits(mode)

        Dim min As Integer = 1
        Dim max As Integer = GetMaxCharacters(mode)

        If count < min OrElse count > max Then
            Throw New ArgumentOutOfRangeException("count", [String].Format("QR {0} character counts must be in the range {1} <= n <= {2}", Description, min, max))
        End If

        Return count.ToBitArray(bits)
    End Function

    Private Function GetCharacterCountBits(mode As Mode) As Integer
        Return CharacterWidthTable.First(Function(f) f.Item1 = Type AndAlso f.Item2 = Version AndAlso f.Item3 = mode).Item4
    End Function

    Private Function GetMaxCharacters(mode As Mode) As Integer
        Return (1 << GetCharacterCountBits(mode)) - 1
    End Function

    Private Shared Function Mul(a1 As Byte, a2 As UShort) As Byte
        Return CByte((a1 + a2) Mod 255)
    End Function

    Private Shared Function Add(a1 As Byte, a2 As Byte) As Byte
        Return LogTable(ExponentTable(a1) Xor ExponentTable(a2))
    End Function
#End Region
End Class
