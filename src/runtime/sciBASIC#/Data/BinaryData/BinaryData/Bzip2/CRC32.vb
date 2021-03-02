' Bzip2 library for .net
' By Jaime Olivares
' Location: http://github.com/jaime-olivares/bzip2
' Ported from the Java implementation by Matthew Francis: https://github.com/MateuszBartosiewicz/bzip2

Namespace Bzip2
    ''' <summary>
    ''' A CRC32 calculator
    ''' </summary>
    Friend Class CRC32
#Region "Private fields"
        ''' <summary>The CRC lookup table</summary> 
        Private Shared ReadOnly Crc32Lookup As UInteger() = {&H00000000, &H04c11db7, &H09823b6e, &H0d4326d9, &H130476dc, &H17c56b6b, &H1a864db2, &H1e475005, &H2608edb8, &H22c9f00F, &H2f8ad6d6, &H2b4bcb61, &H350c9b64, &H31cd86d3, &H3c8ea00a, &H384fbdbD, &H4c11db70, &H48d0c6c7, &H4593e01e, &H4152fda9, &H5f15adac, &H5bd4b01b, &H569796c2, &H52568b75, &H6a1936c8, &H6ed82b7F, &H639b0da6, &H675a1011, &H791d4014, &H7ddc5da3, &H709f7b7a, &H745e66cD, &H9823b6e0, &H9ce2ab57, &H91a18d8e, &H95609039, &H8b27c03c, &H8fe6dd8b, &H82a5fb52, &H8664e6e5, &Hbe2b5b58, &Hbaea46eF, &Hb7a96036, &Hb3687d81, &Had2f2d84, &Ha9ee3033, &Ha4ad16ea, &Ha06c0b5D, &Hd4326d90, &Hd0f37027, &Hddb056fe, &Hd9714b49, &Hc7361b4c, &Hc3f706fb, &Hceb42022, &Hca753d95, &Hf23a8028, &Hf6fb9d9F, &Hfbb8bb46, &Hff79a6f1, &He13ef6f4, &He5ffeb43, &He8bccd9a, &Hec7dd02D, &H34867077, &H30476dc0, &H3d044b19, &H39c556ae, &H278206ab, &H23431b1c, &H2e003dc5, &H2ac12072, &H128e9dcF, &H164f8078, &H1b0ca6a1, &H1fcdbb16, &H018aeb13, &H054bf6a4, &H0808d07D, &H0cc9cdca, &H7897ab07, &H7c56b6b0, &H71159069, &H75d48dde, &H6b93dddb, &H6f52c06c, &H6211e6b5, &H66d0fb02, &H5e9f46bF, &H5a5e5b08, &H571d7dd1, &H53dc6066, &H4d9b3063, &H495a2dd4, &H44190b0D, &H40d816ba, &Haca5c697, &Ha864db20, &Ha527fdf9, &Ha1e6e04e, &Hbfa1b04b, &Hbb60adfc, &Hb6238b25, &Hb2e29692, &H8aad2b2F, &H8e6c3698, &H832f1041, &H87ee0df6, &H99a95df3, &H9d684044, &H902b669D, &H94ea7b2a, &He0b41de7, &He4750050, &He9362689, &Hedf73b3e, &Hf3b06b3b, &Hf771768c, &Hfa325055, &Hfef34de2, &Hc6bcf05F, &Hc27dede8, &Hcf3ecb31, &Hcbffd686, &Hd5b88683, &Hd1799b34, &Hdc3abdeD, &Hd8fba05a, &H690ce0ee, &H6dcdfd59, &H608edb80, &H644fc637, &H7a089632, &H7ec98b85, &H738aad5c, &H774bb0eb, &H4f040d56, &H4bc510e1, &H46863638, &H42472b8F, &H5c007b8a, &H58c1663D, &H558240e4, &H51435d53, &H251d3b9e, &H21dc2629, &H2c9f00f0, &H285e1d47, &H36194d42, &H32d850f5, &H3f9b762c, &H3b5a6b9b, &H0315d626, &H07d4cb91, &H0a97ed48, &H0e56f0fF, &H1011a0fa, &H14d0bd4D, &H19939b94, &H1d528623, &Hf12f560e, &Hf5ee4bb9, &Hf8ad6d60, &Hfc6c70d7, &He22b20d2, &He6ea3d65, &Heba91bbc, &Hef68060b, &Hd727bbb6, &Hd3e6a601, &Hdea580d8, &Hda649d6F, &Hc423cd6a, &Hc0e2d0dD, &Hcda1f604, &Hc960ebb3, &Hbd3e8d7e, &Hb9ff90c9, &Hb4bcb610, &Hb07daba7, &Hae3afba2, &Haafbe615, &Ha7b8c0cc, &Ha379dd7b, &H9b3660c6, &H9ff77d71, &H92b45ba8, &H9675461F, &H8832161a, &H8cf30baD, &H81b02d74, &H857130c3, &H5d8a9099, &H594b8d2e, &H5408abf7, &H50c9b640, &H4e8ee645, &H4a4ffbf2, &H470cdd2b, &H43cdc09c, &H7b827d21, &H7f436096, &H7200464F, &H76c15bf8, &H68860bfD, &H6c47164a, &H61043093, &H65c52d24, &H119b4be9, &H155a565e, &H18197087, &H1cd86d30, &H029f3d35, &H065e2082, &H0b1d065b, &H0fdc1bec, &H3793a651, &H3352bbe6, &H3e119d3F, &H3ad08088, &H2497d08D, &H2056cd3a, &H2d15ebe3, &H29d4f654, &Hc5a92679, &Hc1683bce, &Hcc2b1d17, &Hc8ea00a0, &Hd6ad50a5, &Hd26c4d12, &Hdf2f6bcb, &Hdbee767c, &He3a1cbc1, &He760d676, &Hea23f0aF, &Heee2ed18, &Hf0a5bd1D, &Hf464a0aa, &Hf9278673, &Hfde69bc4, &H89b8fd09, &H8d79e0be, &H803ac667, &H84fbdbd0, &H9abc8bd5, &H9e7d9662, &H933eb0bb, &H97ffad0c, &Hafb010b1, &Hab710d06, &Ha6322bdF, &Ha2f33668, &Hbcb4666D, &Hb8757bda, &Hb5365d03, &Hb1f740b4}

        ''' <summary>The current CRC</summary>
        Private crcField As UInteger = &HfffffffF
#End Region

#Region "Public properties"
        ''' <summary>Gets the current CRC</summary> 
        Public ReadOnly Property CRC As UInteger
            Get
                Return Not crcField
            End Get
        End Property
#End Region

#Region "Public methods"
        ''' <summary>Updates the CRC with a single byte</summary>
        ''' <paramname="value">The value to update the CRC with</param>
        Public Sub UpdateCrc(ByVal value As Integer)
            crcField = crcField << 8 Xor Crc32Lookup((crcField >> 24 Xor value) And &HfF)
        End Sub

        ''' <summary>Update the CRC with a sequence of identical bytes</summary>	
        ''' <paramname="value">The value to update the CRC with</param>
        ''' <paramname="count">The number of bytes</param>
        Public Sub UpdateCrc(ByVal value As Integer, ByVal count As Integer)
            While Math.Max(Threading.Interlocked.Decrement(count), count + 1) > 0
                crcField = crcField << 8 Xor Crc32Lookup((crcField >> 24 Xor value) And &HfF)
            End While
        End Sub
#End Region
    End Class
End Namespace
