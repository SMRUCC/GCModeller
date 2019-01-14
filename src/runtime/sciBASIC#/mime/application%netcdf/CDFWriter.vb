Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.MIME.application.netCDF.Components

Public Module CDFWriter

    Public Function CreateWriter(path As String,
                                 dimensions As Dimension(),
                                 attrs As attribute(),
                                 variables As variable(),
                                 elements%)

        Dim output As New BinaryDataWriter(path.Open) With {
            .ByteOrder = ByteOrder.BigEndian
        }

        ' Magic and version
        Call output.Write(netCDFReader.Magic, BinaryStringFormat.NoPrefixOrTermination)
        Call output.Write(CByte(2))

        ' >>>>>>> header
        Call output.Write(CUInt(elements))
        ' -------------------------dimensionsList----------------------------
        ' List of dimensions
        Call output.Write(CUInt(Header.NC_DIMENSION))
        ' dimensionSize
        Call output.Write(CUInt(dimensions.Length))

        For Each dimension In dimensions
            Call output.Write(dimension.name, BinaryStringFormat.UInt32LengthPrefix)
            Call output.writePadding
            Call output.Write(CUInt(dimension.size))
        Next

        ' ------------------------attributesList-----------------------------
        Call output.writeAttributes(attrs)

        ' -----------------------variablesList--------------------------
        ' List of variables
        Call output.Write(CUInt(Header.NC_VARIABLE))
        ' variableSize 
        Call output.Write(CUInt(variables.Length))

        For Each var In variables
            Call output.Write(var.name, BinaryStringFormat.UInt32LengthPrefix)
            Call output.writePadding
            ' dimensionality 
            Call output.Write(CUInt(var.dimensions.Length))
            ' dimensionsIds
            Call output.Write(var.dimensions)
            ' attributes of this variable
            Call output.writeAttributes(var.attributes)
            Call output.Write(CUInt(str2num(var.type)))
            ' varSize
            Call output.Write(CUInt(var.size))
            ' version = 2, write 8 bytes
            Call output.Write(CUInt(var.offset))
            Call output.Write(CUInt(var.offset))
        Next

        ' <<<<<<<< header
    End Function

    <Extension>
    Private Sub writeAttributes(output As BinaryDataWriter, attrs As attribute())
        ' List of global attributes
        Call output.Write(CUInt(Header.NC_ATTRIBUTE))
        ' attributeSize
        Call output.Write(CUInt(attrs.Length))

        For Each attr In attrs
            Call output.Write(attr.name, BinaryStringFormat.UInt32LengthPrefix)
            Call output.writePadding
            Call output.Write(CUInt(str2num(attr.type)))
            ' one string, size = 1
            Call output.Write(CUInt(1))
            Call output.Write(attr.value, BinaryStringFormat.NoPrefixOrTermination)
            Call output.writePadding
        Next
    End Sub
End Module
