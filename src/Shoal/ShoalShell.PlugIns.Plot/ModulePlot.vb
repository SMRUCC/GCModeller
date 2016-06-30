Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles

<[PackageNamespace]("Plot.Device.Image",
                    Description:="ShoalShell runtime library extensions for Image resource.",
                    Cites:="ShoalShell runtime library extensions.",
                    Publisher:="xie.guigang@live.com",
                    Url:="http://SourceForge.net/projects/shoal")>
Public Module ModulePlot

    <OutputDeviceHandle(GetType(Image))>
    <ExportAPI("image.plot")>
    Public Function Plot(image As Image) As Boolean
        Using Device As FormImagePlotDevice = New FormImagePlotDevice
            Device.PictureBox1.BackgroundImage = image
            Device.Text = String.Format("[{0},{1}] {2}", image.Width, image.Height, image.RawFormat.ToString)
            Device.Size = image.Size
            Device.ShowDialog()
        End Using

        Return True
    End Function

    <OutputDeviceHandle(GetType(Bitmap))>
    <ExportAPI("bitmap.plot")>
    Public Function Plot(image As Bitmap) As Boolean
        Using Device As FormImagePlotDevice = New FormImagePlotDevice
            Device.PictureBox1.BackgroundImage = image
            Device.Text = String.Format("[{0},{1}] {2}", image.Width, image.Height, image.RawFormat.ToString)
            Device.Size = image.Size
            Device.ShowDialog()
        End Using

        Return True
    End Function

    <IO_DeviceHandle(GetType(System.Drawing.Image))>
    <ExportAPI("write.image")>
    Public Function WriteImage(res As Image, saveto As String) As Boolean
        Try
            Call res.Save(saveto)
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try

        Return True
    End Function

    <IO_DeviceHandle(GetType(System.Drawing.Bitmap))>
<ExportAPI("write.image")>
    Public Function WriteImage(res As Bitmap, saveto As String) As Boolean
        Try
            Call res.Save(saveto)
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try

        Return True
    End Function

    <InputDeviceHandle("image")>
    <ExportAPI("read.image")>
    Public Function ReadImage(path As String) As Image
        Return Image.FromFile(path)
    End Function
End Module
