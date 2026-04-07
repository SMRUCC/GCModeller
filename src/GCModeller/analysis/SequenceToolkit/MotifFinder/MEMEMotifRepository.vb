#Region "Microsoft.VisualBasic::e7c6581be221a8f56f507dadf7d472a3, analysis\SequenceToolkit\MotifFinder\MEMEMotifRepository.vb"

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

    '   Total Lines: 38
    '    Code Lines: 29 (76.32%)
    ' Comment Lines: 3 (7.89%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (15.79%)
    '     File Size: 1.51 KB


    ' Class MEMEMotifRepository
    ' 
    '     Properties: FamilyList
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: LoadFamilyMotifs
    ' 
    '     Sub: AddPWM
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.Interops.NBCR
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
Imports MotifSet = SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.PWMDatabase

''' <summary>
''' save meme motif model data inside a local directory with multiple meme text file
''' </summary>
Public Class MEMEMotifRepository : Inherits MotifSet

    Public Overrides ReadOnly Property FamilyList As String()
        Get
            Return fs.EnumerateFiles("/", "*.meme") _
                .Select(Function(file) file.BaseName) _
                .ToArray
        End Get
    End Property

    Public Sub New(dir As String)
        MyBase.New(Microsoft.VisualBasic.FileIO.Directory.FromLocalFileSystem(dir))
    End Sub

    Public Overrides Sub AddPWM(family As String, pwm As IEnumerable(Of Probability))
        Dim writer As New MemeWriter(pwm)
        Dim file As Stream = fs.OpenFile($"/{family.NormalizePathString(False)}.meme", FileMode.OpenOrCreate, FileAccess.Write)

        Using doc As New StreamWriter(file)
            Call writer.WriteDocument(doc)
            Call doc.Flush()
        End Using
    End Sub

    Public Overrides Function LoadFamilyMotifs(family As String) As IEnumerable(Of Probability)
        Return From pwm As MotifPWM
               In MEME_Suite.ParsePWMFile(DirectCast(fs, Microsoft.VisualBasic.FileIO.Directory).GetFullPath($"/{family}.meme"))
               Select CType(pwm, Probability)
    End Function
End Class

