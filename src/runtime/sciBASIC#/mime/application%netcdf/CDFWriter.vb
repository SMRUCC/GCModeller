Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.MIME.application.netCDF.Components

Public Class CDFWriter

    Public Function CreateWriter(path As String, h As Header)
        Dim output As New BinaryDataWriter(path.Open) With {
            .ByteOrder = ByteOrder.BigEndian
        }

        ' Magic and version
        Call output.Write(netCDFReader.Magic, BinaryStringFormat.NoPrefixOrTermination)
        Call output.Write(CByte(2))

        ' >>>>>>> header
        Call output.Write(CUInt(h.recordDimension.length))
        ' -------------------------dimensionsList----------------------------
        ' List of dimensions
        Call output.Write(CUInt(Header.NC_DIMENSION))
        ' dimensionSize
        Call output.Write(CUInt(h.dimensions.Length))

        For Each dimension In h.dimensions
            Call output.Write(dimension.name, BinaryStringFormat.UInt32LengthPrefix)
            Call output.writePadding
            Call output.Write(CUInt(dimension.size))
        Next

        ' ------------------------attributesList-----------------------------
        ' Call output.writeAttributes(h.globalAttributes)

        ' -----------------------variablesList--------------------------
        ' List of variables
        Call output.Write(CUInt(Header.NC_VARIABLE))
        ' variableSize 
        Call output.Write(CUInt(h.variables.Length))

        For Each var In h.variables
            Call output.Write(var.name, BinaryStringFormat.UInt32LengthPrefix)
            Call output.writePadding
            ' dimensionality 
            Call output.Write(CUInt(var.dimensions.Length))
            ' dimensionsIds
            Call output.Write(var.dimensions)
            ' attributes of this variable
            ' Call output.writeAttributes(var.attributes)
            Call output.Write(CUInt(str2num(var.type)))
            ' varSize
            Call output.Write(CUInt(var.size))
            ' version = 2, write 8 bytes
            Call output.Write(CUInt(var.offset))
            Call output.Write(CUInt(var.offset))
        Next

        ' <<<<<<<< header
    End Function

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
End Class
