#Region "Microsoft.VisualBasic::32238c04da786167512c62b8bedbd143, analysis\SequenceToolkit\MotifScanner\PWMDatabase.vb"

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


    ' Code Statistics:

    '   Total Lines: 115
    '    Code Lines: 73 (63.48%)
    ' Comment Lines: 22 (19.13%)
    '    - Xml Docs: 54.55%
    ' 
    '   Blank Lines: 20 (17.39%)
    '     File Size: 4.29 KB


    ' Class PWMDatabase
    ' 
    '     Properties: FamilyList
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: LoadFamilyMotifs, (+2 Overloads) LoadMotifs
    ' 
    '     Sub: AddPWM, (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

''' <summary>
''' PWM database with internal virtual file system
''' </summary>
Public Class PWMDatabase : Implements IDisposable

    Protected ReadOnly fs As IFileSystemEnvironment

    Dim disposedValue As Boolean

    Public Overridable ReadOnly Property FamilyList As String()
        Get
            Dim folder As FileSystemTree = FileSystemTree.BuildTree(fs.GetFiles("/motifs/"))
            Dim subtree = folder.GetFile("motifs").Files
            Dim dirs As FileSystemTree() = subtree.Values _
                .Where(Function(a) a.IsDirectory) _
                .ToArray

            Return dirs _
                .Select(Function(dir) dir.Name) _
                .ToArray
        End Get
    End Property

    Sub New(fs As IFileSystemEnvironment)
        Me.fs = fs
    End Sub

    Public Overridable Sub AddPWM(family As String, pwm As IEnumerable(Of Probability))
        For Each model As Probability In pwm.SafeQuery
            Dim json As JsonElement = model.CreateJSONElement
            Dim bson As MemoryStream = BSONFormat.SafeGetBuffer(json)
            Dim file As String = $"/motifs/{family}/{model.name}.motif"

            Call fs.DeleteFile(file)

            Using block As Stream = fs.OpenFile(file, FileMode.OpenOrCreate, FileAccess.Write)
                Call block.Write(bson.ToArray, Scan0, bson.Length)
                Call block.Flush()
            End Using
        Next
    End Sub

    Public Overridable Iterator Function LoadFamilyMotifs(family As String) As IEnumerable(Of Probability)
        Dim dir As String = $"/motifs/{family}/"

        For Each file As String In fs.EnumerateFiles(dir, "*.motif")
            Dim block As Stream = fs.OpenFile(file, FileMode.Open, FileAccess.Read)
            Dim json As JsonObject = BSONFormat.Load(block, leaveOpen:=True)
            Dim motif As Probability = json.CreateObject(Of Probability)(decodeMetachar:=False)

            Yield motif
        Next
    End Function

    ''' <summary>
    ''' Load all motif data into memory
    ''' </summary>
    ''' <returns></returns>
    Public Function LoadMotifs() As Dictionary(Of String, Probability())
        Dim dbset As New Dictionary(Of String, Probability())

        For Each name As String In FamilyList
            Call dbset.Add(name, LoadFamilyMotifs(name).ToArray)
        Next

        Return dbset
    End Function

    ''' <summary>
    ''' Load all motif data into memory
    ''' </summary>
    ''' <param name="fs"></param>
    ''' <returns></returns>
    Public Shared Function LoadMotifs(fs As IFileSystemEnvironment) As Dictionary(Of String, Probability())
        Using db As New PWMDatabase(fs)
            Return db.LoadMotifs
        End Using
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                If CObj(fs).GetType.ImplementInterface(Of IDisposable) Then
                    Call DirectCast(fs, IDisposable).Dispose()
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

