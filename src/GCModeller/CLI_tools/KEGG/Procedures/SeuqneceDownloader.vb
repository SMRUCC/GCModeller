#Region "Microsoft.VisualBasic::60ff8092a858c1074538d6a0a06968d1, ..\GCModeller\CLI_tools\KEGG\Procedures\SeuqneceDownloader.vb"

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

Public Class SeuqneceDownloader : Inherits jp_kegg2

    Sub New(uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
        Call MyBase.New(uri)
    End Sub

    Public Sub RunTask()
        KEGG.UriMySQL.TimeOut = 1000 * 60 * 100 * 60
        Call KEGG.ForEach(Of LocalMySQL.gene)("SELECT * FROM jp_kegg2.gene;", AddressOf __fillSeq)
    End Sub

    Private Sub __fillSeq(gene As LocalMySQL.gene)
        Dim update As Boolean = False
        Dim query As SMRUCC.genomics.SequenceModel.FASTA.FastaToken = Nothing

        If String.IsNullOrEmpty(gene.aa_seq) Then
            query = SMRUCC.genomics.Assembly.KEGG.WebServices.FetchSeq(gene.kegg_sp, gene.locus_tag)
            gene.aa_seq = query.SequenceData.ToUpper
            update = True
            Call Threading.Thread.Sleep(500)  ' 需要休眠降低服务器压力以免被封IP
        End If
        If String.IsNullOrEmpty(gene.nt_seq) Then
            query = SMRUCC.genomics.Assembly.KEGG.WebServices.FetchNt(gene.kegg_sp, gene.locus_tag)
            gene.nt_seq = query.SequenceData.ToUpper
            update = True
            Call Threading.Thread.Sleep(500)
        End If

        If update Then
            If String.IsNullOrEmpty(gene.gene_name) Then
                gene.gene_name = gene.definition
            End If
            gene.definition = query.Title.Replace("'", "~")
            'Call KEGG.ExecUpdate(gene)
            Call FileIO.FileSystem.WriteAllText(App.CurrentDirectory & "/genes.sql", gene.GetUpdateSQL & vbCrLf, append:=True)
            Call Threading.Thread.Sleep(250)
        Else
            Call Console.Write(".")
        End If
    End Sub
End Class

