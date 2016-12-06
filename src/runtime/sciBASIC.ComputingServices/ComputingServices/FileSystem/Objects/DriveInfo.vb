#Region "Microsoft.VisualBasic::ff69d83bb9d262a23a3d4624be08116f, ..\ComputingServices\FileSystem\Objects\DriveInfo.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic.Language

Namespace FileSystem

    ''' <summary>
    ''' Provides access to information on a drive.
    ''' </summary>
    <ComVisible(True)> Public NotInheritable Class DriveInfo : Inherits ClassObject

        Sub New(info As System.IO.DriveInfo)
            Me.AvailableFreeSpace = info.AvailableFreeSpace
            Me.DriveFormat = info.DriveFormat
            Me.DriveType = info.DriveType
            Me.IsReady = info.IsReady
            Me.Name = info.Name
            Me.RootDirectory = info.RootDirectory
            Me.TotalFreeSpace = info.TotalFreeSpace
            Me.TotalSize = info.TotalSize
            Me.VolumeLabel = info.VolumeLabel
        End Sub

        Sub New()
        End Sub

        ' Exceptions:
        '   T:System.UnauthorizedAccessException:
        '     Access to the drive information is denied.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurred (for example, a disk error or a drive was not ready).
        ''' <summary>
        ''' Indicates the amount of available free space on a drive, in bytes.
        ''' </summary>
        ''' <returns>The amount of free space available on the drive, in bytes.</returns>
        Public Property AvailableFreeSpace As Long

        ' Exceptions:
        '   T:System.UnauthorizedAccessException:
        '     Access to the drive information is denied.
        '
        '   T:System.IO.DriveNotFoundException:
        '     The drive does not exist or is not mapped.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurred (for example, a disk error or a drive was not ready).
        ''' <summary>
        ''' Gets the name of the file system, such as NTFS or FAT32.
        ''' </summary>
        ''' <returns>The name of the file system on the specified drive.</returns>
        Public Property DriveFormat As String

        ''' <summary>
        ''' Gets the drive type, such as CD-ROM, removable, network, or fixed.
        ''' </summary>
        ''' <returns>One of the enumeration values that specifies a drive type.</returns>
        Public Property DriveType As DriveType

        ''' <summary>
        ''' Gets a value that indicates whether a drive is ready.
        ''' </summary>
        ''' <returns>true if the drive is ready; false if the drive is not ready.</returns>
        Public Property IsReady As Boolean

        ''' <summary>
        ''' Gets the name of a drive, such as C:\.
        ''' </summary>
        ''' <returns>The name of the drive.</returns>
        Public Property Name As String

        ''' <summary>
        ''' Gets the root directory of a drive.
        ''' </summary>
        ''' <returns>An object that contains the root directory of the drive.</returns>
        Public Property RootDirectory As System.IO.DirectoryInfo

        ' Exceptions:
        '   T:System.UnauthorizedAccessException:
        '     Access to the drive information is denied.
        '
        '   T:System.IO.DriveNotFoundException:
        '     The drive is not mapped or does not exist.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurred (for example, a disk error or a drive was not ready).
        ''' <summary>
        ''' Gets the total amount of free space available on a drive, in bytes.
        ''' </summary>
        ''' <returns>The total free space available on a drive, in bytes.</returns>
        Public Property TotalFreeSpace As Long

        ' Exceptions:
        '   T:System.UnauthorizedAccessException:
        '     Access to the drive information is denied.
        '
        '   T:System.IO.DriveNotFoundException:
        '     The drive is not mapped or does not exist.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurred (for example, a disk error or a drive was not ready).
        ''' <summary>
        ''' Gets the total size of storage space on a drive, in bytes.
        ''' </summary>
        ''' <returns>The total size of the drive, in bytes.</returns>
        Public Property TotalSize As Long

        ' Exceptions:
        '   T:System.IO.IOException:
        '     An I/O error occurred (for example, a disk error or a drive was not ready).
        '
        '   T:System.IO.DriveNotFoundException:
        '     The drive is not mapped or does not exist.
        '
        '   T:System.Security.SecurityException:
        '     The caller does not have the required permission.
        '
        '   T:System.UnauthorizedAccessException:
        '     The volume label is being set on a network or CD-ROM drive.-or-Access to the
        '     drive information is denied.
        ''' <summary>
        ''' Gets or sets the volume label of a drive.
        ''' </summary>
        ''' <returns>The volume label.</returns>
        Public Property VolumeLabel As String
    End Class
End Namespace
