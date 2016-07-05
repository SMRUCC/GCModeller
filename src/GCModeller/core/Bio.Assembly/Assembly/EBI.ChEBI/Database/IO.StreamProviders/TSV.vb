#Region "Microsoft.VisualBasic::401fd8362838b873a496cd4a61d78b6d, ..\GCModeller\core\Bio.Assembly\Assembly\EBI.ChEBI\Database\IO.StreamProviders\TSV.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic

Namespace Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv

    ''' <summary>
    ''' 将文件读取出来然后对每一行数据进行分割
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FileIO : Inherits Microsoft.VisualBasic.ComponentModel.ITextFile

        Public Shared Function Load(Of T As IO.StreamProviders.Tsv.Tables.BaseElements)(Path As String) As T()
            Dim Chunkbuffer = FileIO.LoadFile(Path)
            Dim Heads As String() = Chunkbuffer.First
            Chunkbuffer = Chunkbuffer.Skip(1).ToArray

            Dim SchemaCache As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
            For i As Integer = 0 To Heads.Count - 1
                Call SchemaCache.Add(Heads(i), i)
            Next

            Dim TableSchema = GetSchema(Of T)()
            Dim ChunkList As List(Of T) = New List(Of T)
            For Each LineBuffer In Chunkbuffer
                Dim DataElement As T = Activator.CreateInstance(Of T)()

                For Each Field As KeyValuePair(Of String, System.Reflection.PropertyInfo) In TableSchema
                    Dim index As Integer = SchemaCache(Field.Key)
                    Call Field.Value.SetValue(DataElement, LineBuffer(index))
                Next

                Call ChunkList.Add(DataElement)
            Next

            Return ChunkList.ToArray
        End Function

        Public Shared Function GetSchema(Of T As Tables.BaseElements)() As KeyValuePair(Of String, System.Reflection.PropertyInfo)()
            Dim Type As System.Type = GetType(T)
            Dim LQuery = (From p As System.Reflection.PropertyInfo
                          In Type.GetProperties
                          Where p.CanRead AndAlso p.CanWrite
                          Select New KeyValuePair(Of String, System.Reflection.PropertyInfo)(p.Name, p)).ToArray
            Return LQuery
        End Function

        Public Shared Function LoadFile(path As String) As String()()
            Dim Chunkbuffer As String() = ReadAllLines(path)
            Dim LQuery = (From strLine As String In Chunkbuffer Select Strings.Split(strLine, vbTab)).ToArray '跳过标题行
            Return LQuery
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class TSV : Inherits ComponentModel.TabularLazyLoader

        Dim Names As Tables.Names()
        Dim Accessions As Tables.DatabaseAccession()

        Sub New(DataDir As String)
            Call MyBase.New(DataDir, {"*.tsv"})
        End Sub

        Public Function GetNames(Optional filename As String = "names.tsv") As Tables.Names()
            Return LoadData(Of Tables.Names)(Names, filename)
        End Function

        Private Function LoadData(Of T As Tables.BaseElements)(ByRef ChunkBuffer As T(), FileName As String) As T()
            If ChunkBuffer.IsNullOrEmpty Then
                FileName = String.Format("{0}/{1}", _DIR, FileName)
                Call Console.WriteLine("[{0}] Load data from {1}", GetType(T).FullName, FileName)
                Dim st = Stopwatch.StartNew
                ChunkBuffer = FileIO.Load(Of T)(FileName)
                Call Console.WriteLine("[LOAD_DATA_DONE] Performance {0} objects in {1} ms..." & vbCrLf, ChunkBuffer.Count, st.ElapsedMilliseconds)
            End If
            Return ChunkBuffer
        End Function

        Public Function GetDatabaseAccessions(Optional filename As String = "database_accession.tsv") As Tables.DatabaseAccession()
            Return LoadData(Of Tables.DatabaseAccession)(Accessions, filename)
        End Function
    End Class
End Namespace
