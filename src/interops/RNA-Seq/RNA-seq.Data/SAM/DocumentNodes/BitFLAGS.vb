#Region "Microsoft.VisualBasic::4710b0601c001824233350a9a1255413, RNA-Seq\RNA-seq.Data\SAM\DocumentNodes\BitFLAGS.vb"

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

    '     Enum BitFlags
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Module BitFLAGS_API
    ' 
    '         Properties: BitFLAG, Descriptions
    ' 
    '         Function: bitFLAGDescriptions, ComputeBitFLAGS, GetBitFLAGDescriptions
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SAM

    ''' <summary>
    ''' Bitwise flags in .bam/.sam files
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' Concept: Bitwise flags in .bam/.sam files(如何理解 Bitwise flags 的含义)
    ''' 
    ''' Anyone who deals with .sam mapping files has seen the bitwise flag column. It is a single number that can somehow 
    ''' indicate settings for a bunch of different parameters. As per the sam file specifications:
    ''' 
    '''  Bit    Description
    '''  0x1    template having multiple segments in sequencing
    '''  0x2    each segment properly aligned according to the aligner
    '''  0x4    segment unmapped
    '''  0x8    next segment in the template unmapped
    '''  0x10   SEQ being reverse complemented
    '''  0x20   SEQ of the next segment in the template being reversed
    '''  0x40   the first segment in the template
    '''  0x80   the last segment in the template
    '''  0x100  secondary alignment
    '''  0x200  Not passing quality controls
    '''  0x400  PCR Or optical duplicate
    ''' 
    ''' What does all that mean And how does bitwise flags work? This post will probably go into more detail than you need To For understand bitwise flags.
    ''' 
    ''' Radix And numeral systems
    ''' 
    ''' Most of the maths we've learned in school are in the decimal system or a base 10 system. In the decimal system, 
    ''' the number 1,324 is equal to: 
    ''' 
    ''' 1324 = 1 * 103 + 3 * 102 + 2 * 101 + 4 * 100
    ''' 
    ''' *remember from your old math classes that 101 = 10 And 100= 1
    ''' 
    ''' The base Of a numeral system Is also called a radix. A Decimal system has a radix Of 10.
    ''' 
    ''' One of the reasons for a numeral system Is to reduce the amount of unique symbols we have to come up with to represent numbers. 
    ''' It Is important to realize the difference between numbers as an abstract idea And numbers as a physical symbol.
    ''' 
    ''' In the decimal system, we have symbols (0-9) representing the numbers zero through nine. Using these ten number symbols (0-9), 
    ''' we can represent any other number, ie the number ten Is the symbol '1' and the symbol '0' together. If we didn't   
    ''' use a number system, we would need a unique symbol for 10, 11, 12, 13.. and so on, resulting in an unmanagable amount of symbols. 
    ''' 
    ''' However, there Is an advantage for having more unique symbols to represent numbers; And that Is less amount of symbols 
    ''' would be needed for larger numbers. Imagine if we have a unique symbol for every number up to 100. Instead of writing two   
    ''' symbols in the decimal system for '10', we would only have to write the one unique symbol for ten.
    ''' 
    ''' Binary
    ''' 
    ''' To understand bitwise flags, we must first understand what binary encoding Is And how it applies to computing. 
    ''' Binary Is a number system with a radix of 2. Only two symbols, 0 And 1 are used to represent numbers. The decimal number, 1324 in   
    ''' binary would be 10100101100
    ''' 
    ''' 1324 = 1 * 210 + 0 * 29 + 1 * 28 + 0 * 27 + 0 * 26 + 1 * 25 + 0 * 24 + 1 * 23 + 1 * 22 + 0 * 21 + 0 * 20
    ''' 
    ''' With only two symbols describing numbers, binary requires a longer String Of symbols To represent larger numbers. 
    ''' So why Do we use only two symbols? Because computers operate Using two states based On the physical hardware. 
    ''' A high voltage state And a low voltage state In Each Of the transistors On a computer processer represents the binary system.
    ''' 
    ''' Since computers only work In a binary system, all data would need To be encoded In binary For operations To be performed On it. 
    ''' Each fundamental binary data Of 0 Or 1 Is called a 'bit'. The number 1324 would contain 11 bits.
    ''' 
    ''' 
    ''' Bitwise flag
    ''' 
    ''' Notice how Each flag In the .sam specification has a yes Or no setting. Each flag Is a bit, containg a 0 Or a 1, 
    ''' representing no And yes respectively. If the 5th bit (0x10) Is 1 Then the sequence Is reverse complemented; If it Is 0 than it Is 
    ''' Not reverse complemented. A bitwise flag where only the 5th bit Is Set would have a binary String Of:
    ''' 
    ''' binary 10000 = 1 * 24 + 0 * 23 + 0 * 22 + 0 * 21 + 0 * 20 = decimal 16
    ''' 
    ''' The '0x10' in the .sam specification flag table is the convention for writing hexadecimal, which has a radix of 16. 
    ''' Hexadecimal of 10 would be decimal 16:
    ''' 
    ''' hexadecimal 10 = 1 * 161 + 0 * 160 = decimal 16
    ''' 
    ''' 
    ''' Summary
    ''' 
    ''' The number that you see In the bitwise flag column Of your .sam files can be converted To binary encoding which consists 
    ''' Of series Of 0S And 1S. These 0S And 1S indicate whether the flag In the position Is Set (1), meaning True Or Not Set (0), 
    ''' meaning False. The smallest number possible would be a 0, indicating every flag Not Set. The largest number would be 2047, 
    ''' With a binary String Of '11111111111' indicating every flag set. 
    ''' 
    ''' For example, the number 528 in binary would be 1000010000
    ''' 
    ''' 528 = 1 * 29 + 0 * 28 + 0 * 27 + 0 * 26 + 0 * 25 + 1 * 24 + 0 * 23 + 0 * 22 + 0 * 21 + 0 * 20
    ''' 
    ''' The 5th bit (1 * 24) Is set as 1, meaning the read Is reverse complemented. The 10th bit (1 * 29) Is set as 1, meaning the read does Not pass quality controls.
    ''' 
    ''' 
    ''' Decoding bitwise flag
    ''' 
    ''' To decode the bitwise flags in .sam files, you can use this online tool at the picard tools website. 
    ''' Alternatively, to decode in python Or perl, you can use bitwise opeartors. To see whether a bit Is set, 
    ''' you do a 'bitwise AND' operation with the '&amp;' operator. 
    ''' 
    ''' For example To check If the 5th bit Is Set, you would check If the flag has a bitwise And With 16 (2^4 power, 
    ''' 5th bit Is actually 4th power because numeral systems start With 0th power)
    ''' 
    ''' #In python
    '''     If flag &amp; 16
    '''         Return 'negative strand'
    '''     Else
    '''         Return 'positive strand'
    ''' 
    ''' #In perl
    '''     If ($flag &amp; 16) { 
    '''         Return 'negative strand';
    '''     } else {
    '''         Return 'positive strand';
    '''     } 
    ''' 
    ''' </remarks>
    Public Enum BitFlags As Integer

        ''' <summary>
        ''' template having multiple segments in sequencing
        ''' If 0x1 is unset, no assumptions can be made about 0x2, 0x8, 0x20, 0x40 and 0x80.
        ''' (假若1标注没有被设置，则不会进行对2,8,32,64,128的标注进行假设???????)
        ''' </summary>
        <Description("MultipleSegments")> Bit0x1 = 1
        ''' <summary>
        ''' Each segment properly aligned according To the aligner
        ''' </summary>
        <Description("ProperlyAligned")> Bit0x2 = 2
        ''' <summary>
        ''' segment unmapped
        ''' 
        ''' Bit 0x4 is the only reliable place to tell whether the segment is unmapped. If 0x4 is set, no
        ''' assumptions can be made about RNAME, POS, CIGAR, MAPQ, bits 0x2, 0x10 And 0x100
        ''' And the bit 0x20 of the next segment in the template.
        ''' </summary>
        <Description("UnMapped")> Bit0x4 = 4
        ''' <summary>
        ''' Next segment In the template unmapped
        ''' </summary>
        <Description("NextUnMapped")> Bit0x8 = 8
        ''' <summary>
        ''' SEQ being reverse complemented
        ''' </summary>
        <Description("Reversed")> Bit0x10 = 16
        ''' <summary>
        ''' SEQ Of the Next segment In the template being reversed
        ''' </summary>
        <Description("NextReversed")> Bit0x20 = 32
        ''' <summary>
        ''' the first segment In the template
        ''' </summary>
        <Description("First")> Bit0x40 = 64
        ''' <summary>
        ''' the last segment In the template
        ''' </summary>
        <Description("Last")> Bit0x80 = 128
        ''' <summary>
        ''' secondary alignment
        ''' 
        ''' Bit 0x100 marks the alignment not to be used in certain analyses when the tools in use are
        ''' aware Of this bit.
        ''' (256标注表示这个比对不会被使用于特定的分析之中当工具)
        ''' </summary>
        <Description("Secondary")> Bit0x100 = 256
        ''' <summary>
        ''' Not passing quality controls
        ''' </summary>
        <Description("LowQuality")> Bit0x200 = 512
        ''' <summary>
        ''' PCR Or optical duplicate
        ''' </summary>
        <Description("Duplicated")> Bit0x400 = 1024
    End Enum

    <Package("SAM.BitFLAGS", Category:=APICategories.UtilityTools)>
    Public Module BitFLAGS_API

        ''' <summary>
        ''' 标记静态缓存
        ''' </summary>
        Public ReadOnly Property BitFLAG As BitFlags() = {
 _
            BitFlags.Bit0x1,     ' template having multiple segments in sequencing
            BitFlags.Bit0x2,     ' each segment properly aligned according to the aligner
            BitFlags.Bit0x4,     ' segment unmapped
            BitFlags.Bit0x8,     ' next segment in the template unmapped
            BitFlags.Bit0x10,    ' SEQ being reverse complemented
            BitFlags.Bit0x20,    ' SEQ of the next segment in the template being reversed
            BitFlags.Bit0x40,    ' the first segment in the template
            BitFlags.Bit0x80,    ' the last segment in the template
            BitFlags.Bit0x100,   ' secondary alignment
            BitFlags.Bit0x200,   ' Not passing quality controls
            BitFlags.Bit0x400    ' PCR Or optical duplicate
        }

        ''' <summary>
        ''' 标记描述的静态缓存
        ''' </summary>
        Public ReadOnly Property Descriptions As IReadOnlyDictionary(Of BitFlags, String) = bitFLAGDescriptions()

        Private Function bitFLAGDescriptions() As Dictionary(Of BitFlags, String)
            Dim LQuery = From FLAG As BitFlags In _BitFLAG Select FLAG, FLAG.Description '
            Dim table As Dictionary(Of BitFlags, String) =
                LQuery.ToDictionary(Function(obj) obj.FLAG,
                                    Function(obj) obj.Description)
            Return table
        End Function

        ''' <summary>
        ''' | Bit   |Description                                           |
        ''' |-------|------------------------------------------------------|
        ''' | 0x1   |template having multiple segments in sequencing       |
        ''' | 0x2   |each segment properly aligned according to the aligner|
        ''' | 0x4   |segment unmapped                                      |
        ''' | 0x8   |next segment in the template unmapped                 |
        ''' | 0x10  |SEQ being reverse complemented                        |
        ''' | 0x20  |SEQ of the next segment in the template being reversed|
        ''' | 0x40  |the first segment in the template                     |
        ''' | 0x80  |the last segment in the template                      |
        ''' | 0x100 |secondary alignment                                   |
        ''' | 0x200 |Not passing quality controls                          |
        ''' | 0x400 |PCR Or optical duplicate                              |
        ''' 
        ''' 先将数值从十进制转换为2进制，然后再从后面往前面取标记
        ''' </summary>
        ''' <param name="Flag"></param>
        ''' <returns></returns>
        <ExportAPI("Compute.BitFLAGS")>
        Public Function ComputeBitFLAGS(Flag As Integer) As BitFlags()
            ' 将比对结果之中的标记转换为比特值，然后按位取出符号标记
            Dim Bits As Integer() = Convert _
                .ToString(Flag, toBase:=2) _
                .Select(Function(c) CInt(Val(c))) _
                .ToArray
            Dim i As Integer = 0
            Dim BitFlags As New List(Of BitFlags)

            For idx As Integer = Bits.Length - 1 To 0 Step -1
                If Bits(idx) = 1 Then
                    Call BitFlags.Add(_BitFLAG(i))
                End If

                i += 1
            Next

            Return BitFlags.ToArray
        End Function

        <ExportAPI("GET.Descriptions")>
        Public Function GetBitFLAGDescriptions(FLAGS As BitFlags()) As String
            Dim LQuery = (From FLAG As BitFlags In FLAGS Select Descriptions(FLAG)).ToArray
            Dim s As String = LQuery.GetJson
            Return s
        End Function
    End Module
End Namespace
