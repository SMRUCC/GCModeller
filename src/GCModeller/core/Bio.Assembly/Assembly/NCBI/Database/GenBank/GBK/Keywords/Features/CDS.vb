#Region "Microsoft.VisualBasic::5feb6bc8c207550724ccce32610c8367, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\Features\CDS.vb"

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

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' 为CDS字段记录所特化的对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CDS : Inherits Feature

        Public Property db_xref_GI As String
        Public Property db_xref_GO As String
        Public Property db_xref_InterPro As String()
        Public Property db_xref_UniprotKBSwissProt As String
        Public Property db_xref_UniprotKBTrEMBL As String

        Public Property DBLinks As MetaCyc.Schema.DBLinkManager

        Sub New(CDS_Feature As Feature)
            Me.Location = CDS_Feature.Location
            Me.KeyName = CDS_Feature.KeyName

            Call CDS_Feature.CopyTo(Me.__innerList)

            Dim db_xref = (From str As String In Me.QueryDuplicated("db_xref")
                           Let Tokens As String() = str.Split(":"c)
                           Select Key = Tokens.First, Value = Tokens.Last
                           Group By Key Into Group).ToArray.ToDictionary(keySelector:=Function(item) item.Key, elementSelector:=Function(item) (From obj In item.Group Select obj.Value).ToArray)

            Dim TempChunk As String() = Nothing

            Call db_xref.TryGetValue("GI", TempChunk) : If Not TempChunk.IsNullOrEmpty Then db_xref_GI = TempChunk.First
            Call db_xref.TryGetValue("GOA", TempChunk) : If Not TempChunk.IsNullOrEmpty Then db_xref_GO = TempChunk.First
            Call db_xref.TryGetValue("UniProtKB/Swiss-Prot", TempChunk) : If Not TempChunk.IsNullOrEmpty Then db_xref_UniprotKBSwissProt = TempChunk.First
            Call db_xref.TryGetValue("UniProtKB/TrEMBL", TempChunk) : If Not TempChunk.IsNullOrEmpty Then db_xref_UniprotKBTrEMBL = TempChunk.First
            Call db_xref.TryGetValue("InterPro", db_xref_InterPro)

            Me.gbRaw = CDS_Feature.gbRaw
        End Sub
    End Class
End Namespace
