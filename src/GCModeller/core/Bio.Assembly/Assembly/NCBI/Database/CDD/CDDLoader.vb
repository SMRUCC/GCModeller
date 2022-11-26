#Region "Microsoft.VisualBasic::113639b48b44f16149979d0c2b4ad579, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\CDD\CDDLoader.vb"

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

    '   Total Lines: 153
    '    Code Lines: 114
    ' Comment Lines: 12
    '   Blank Lines: 27
    '     File Size: 5.08 KB


    '     Class CDDLoader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetCdd, GetCog, GetFastaUrl, GetItem, GetKog
    '                   GetPfam, GetPrk, GetSmart, GetTigr, Load
    '                   LoadFASTA, ToString
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Dir = System.String

Namespace Assembly.NCBI.CDD

    Public NotInheritable Class CDDLoader : Implements IDisposable

        Protected Friend Cdd, Cog, Kog, Pfam, Prk, Smart, Tigr As DbFile

        Dim DIR As String

        Protected _getDBMethods As Func(Of DbFile)() = New Func(Of DbFile)() {
            AddressOf GetCdd,
            AddressOf GetCog,
            AddressOf GetKog,
            AddressOf GetPfam,
            AddressOf GetPrk,
            AddressOf GetSmart,
            AddressOf GetTigr
        }

        Public Function GetItem(Id As String) As CDD.SmpFile
            Dim LQuery = LinqAPI.DefaultFirst(Of CDD.SmpFile) _
 _
                () <= From Db As Func(Of DbFile)
                      In _getDBMethods.AsParallel
                      Let DbFile As DbFile = Db()
                      Let SmpItem As CDD.SmpFile = DbFile.ContainsId(Id)
                      Where Not SmpItem Is Nothing
                      Select SmpItem

            Return LQuery
        End Function

        Default Public ReadOnly Property Item(id As String) As CDD.SmpFile
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return GetItem(id)
            End Get
        End Property

        Sub New(CDDDir As Dir)
            DIR = CDDDir
        End Sub

        Public Function GetPrk() As CDD.DbFile
            If Prk Is Nothing Then
                Prk = Load("Prk")
            End If
            Return Prk
        End Function

        Public Function GetSmart() As CDD.DbFile
            If Smart Is Nothing Then
                Smart = Load("Smart")
            End If
            Return Smart
        End Function

        Public Function GetTigr() As CDD.DbFile
            If Tigr Is Nothing Then
                Tigr = Load("Tigr")
            End If
            Return Tigr
        End Function

        Public Function GetCdd() As CDD.DbFile
            If Cdd Is Nothing Then
                Cdd = Load("Cdd")
            End If
            Return Cdd
        End Function

        Public Function GetCog() As CDD.DbFile
            If Cog Is Nothing Then
                Cog = Load("Cog")
            End If
            Return Cog
        End Function

        Public Function GetKog() As CDD.DbFile
            If Kog Is Nothing Then
                Kog = Load("Kog")
            End If
            Return Kog
        End Function

        Public Function GetPfam() As CDD.DbFile
            If Pfam Is Nothing Then
                Pfam = Load("Pfam")
            End If
            Return Pfam
        End Function

        Public Function Load(DbName As String) As DbFile
            Call $"> Loading database ""{DbName}""...".__DEBUG_ECHO

            Dim FilePath As String = $"{DIR}/{DbName}.xml"
            Dim DbFile As DbFile = FilePath.LoadTextDoc(Of DbFile)(ThrowEx:=False)
            Return DbFile
        End Function

        Public Function LoadFASTA(DbName As String) As FASTA.FastaFile
            Return FastaFile.Read(String.Format("{0}/{1}.fasta", DIR, DbName))
        End Function

        Public Function GetFastaUrl(DbName As String) As String
            Return String.Format("{0}/{1}.fasta", DIR, DbName)
        End Function

        Public Overrides Function ToString() As String
            Return DIR
        End Function

        Public Shared Widening Operator CType(cddDIR As String) As CDDLoader
            Return New CDDLoader(cddDIR)
        End Operator

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
