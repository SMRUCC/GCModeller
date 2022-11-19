#Region "Microsoft.VisualBasic::45646dc4da82e8cb4ff363713af3eeca, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\CDD\Database.vb"

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

    '   Total Lines: 241
    '    Code Lines: 157
    ' Comment Lines: 51
    '   Blank Lines: 33
    '     File Size: 9.28 KB


    '     Class Database
    ' 
    '         Properties: Cdd, Cog, DbPaths, DomainInfo, Kog
    '                     Paths, Pfam, Prk, Smart, Tigr
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetDomainFasta, GetDomainInfo, ToString
    ' 
    '         Sub: (+2 Overloads) Dispose
    '         Class FastaLoader
    ' 
    '             Function: GetCdd, GetCog, GetKog, GetPfam, GetPrk
    '                       GetSmart, GetTigr, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.NCBI.CDD

    Public Class Database : Implements System.IDisposable

        ReadOnly _DIR As String, _FastaLoader As FastaLoader

        Public ReadOnly Property DomainInfo As DomainInfo

        Sub New(DbDIR As String)
            _DomainInfo = DomainInfo.PreLoad(DbDIR)
            _DIR = DbDIR
            _FastaLoader = New FastaLoader With {
                .Database = Me
            }
        End Sub

        ''' <summary>
        ''' 返回指定名称的数据库的文件路径
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public ReadOnly Property Db(Name As String) As String
            Get
                Name = Mid(Name, 1, 1).ToUpper & Mid(Name, 2)  '首字母大写
                Return Me.DbPaths(Name).Key()()
            End Get
        End Property

        ''' <summary>
        ''' cdd.fasta
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Cdd As String
            Get
                Return String.Format("{0}/Cdd.fasta", _DIR)
            End Get
        End Property

        ''' <summary>
        ''' cog.fasta
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Cog As String
            Get
                Return String.Format("{0}/Cog.fasta", _DIR)
            End Get
        End Property

        ''' <summary>
        ''' kog.fasta
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Kog As String
            Get
                Return String.Format("{0}/Kog.fasta", _DIR)
            End Get
        End Property

        ''' <summary>
        ''' pfam.fasta
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Pfam As String
            Get
                Return String.Format("{0}/Pfam.fasta", _DIR)
            End Get
        End Property

        ''' <summary>
        ''' prk.fasta
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Prk As String
            Get
                Return String.Format("{0}/Prk.fasta", _DIR)
            End Get
        End Property

        ''' <summary>
        ''' smart.fasta
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Smart As String
            Get
                Return String.Format("{0}/Smart.fasta", _DIR)
            End Get
        End Property

        ''' <summary>
        ''' tigr.fasta
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Tigr As String
            Get
                Return String.Format("{0}/Tigr.fasta", _DIR)
            End Get
        End Property

        Public Function GetDomainInfo(Id As String) As CDD.SmpFile
            For Each DbName As String In DbPaths.Keys
                Dim Db = DomainInfo(Name:=DbName)
                Dim LQuery = From Item In Db.SmpData.AsParallel Where String.Equals(Item.Name, Id) Select Item '
                Dim Result = LQuery.FirstOrDefault
                Return Result
            Next
            Return Nothing '没有查询到任何记录
        End Function

        Public Function GetDomainFasta(Id As String) As FASTA.FastaSeq
            For Each DbPath In DbPaths.Values
                Dim Db As SequenceModel.FASTA.FastaFile = DbPath.Value()()
                Dim LQuery = From Fsa In Db Where String.Equals(Fsa.Headers(0), Id) Select Fsa '
                Dim Result = LQuery.FirstOrDefault
                Return Result
            Next
            Return Nothing
        End Function

        Public ReadOnly Property DbPaths As Dictionary(Of String, KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile))) =
            New Dictionary(Of String, KeyValuePair(Of Func(Of String), Func(Of SequenceModel.FASTA.FastaFile))) From {
                {"Cdd", New KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile))(Function() Me.Cdd, Function() _FastaLoader.GetCdd)},
                {"Cog", New KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile))(Function() Me.Cog, Function() _FastaLoader.GetCog)},
                {"kog", New KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile))(Function() Me.Kog, Function() _FastaLoader.GetKog)},
                {"Pfam", New KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile))(Function() Me.Pfam, Function() _FastaLoader.GetPfam)},
                {"Prk", New KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile))(Function() Me.Prk, Function() _FastaLoader.GetPrk)},
                {"Smart", New KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile))(Function() Me.Smart, Function() _FastaLoader.GetSmart)},
                {"Tigr", New KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile))(Function() Me.Tigr, Function() _FastaLoader.GetTigr)}}

        Public ReadOnly Property Paths As String()
            Get
                Dim Query = From path In DbPaths.Values Select path.Key()() '
                Return Query.ToArray
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return _DIR
        End Function

        Public Shared Widening Operator CType(DbPath As String) As Database
            Return New Database(DbPath)
        End Operator

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
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

        ''' <summary>
        ''' 本对象会将读取的数据缓存与内存之中，以加速下一次查询的速度
        ''' </summary>
        ''' <remarks></remarks>
        Friend NotInheritable Class FastaLoader
            Friend Cdd, Cog, Kog, Pfam, Prk, Smart, Tigr As SequenceModel.FASTA.FastaFile
            Friend Database As Database

            Public Function GetPrk() As SequenceModel.FASTA.FastaFile
                If Prk Is Nothing Then
                    Prk = SequenceModel.FASTA.FastaFile.Read(Database.Prk)
                End If
                Return Prk
            End Function

            Public Function GetSmart() As SequenceModel.FASTA.FastaFile
                If Smart Is Nothing Then
                    Smart = SequenceModel.FASTA.FastaFile.Read(Database.Smart)
                End If
                Return Smart
            End Function

            Public Function GetTigr() As SequenceModel.FASTA.FastaFile
                If Tigr Is Nothing Then
                    Tigr = SequenceModel.FASTA.FastaFile.Read(Database.Tigr)
                End If
                Return Tigr
            End Function

            Public Function GetCdd() As SequenceModel.FASTA.FastaFile
                If Cdd Is Nothing Then
                    Cdd = SequenceModel.FASTA.FastaFile.Read(Database.Cdd)
                End If
                Return Cdd
            End Function

            Public Function GetCog() As SequenceModel.FASTA.FastaFile
                If Cog Is Nothing Then
                    Cog = SequenceModel.FASTA.FastaFile.Read(Database.Cog)
                End If
                Return Cog
            End Function

            Public Function GetKog() As SequenceModel.FASTA.FastaFile
                If Kog Is Nothing Then
                    Kog = SequenceModel.FASTA.FastaFile.Read(Database.Kog)
                End If
                Return Kog
            End Function

            Public Function GetPfam() As SequenceModel.FASTA.FastaFile
                If Pfam Is Nothing Then
                    Pfam = SequenceModel.FASTA.FastaFile.Read(Database.Pfam)
                End If
                Return Pfam
            End Function

            Public Overrides Function ToString() As String
                Return Database._DIR
            End Function
        End Class
    End Class
End Namespace
