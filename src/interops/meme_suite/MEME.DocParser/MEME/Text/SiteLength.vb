#Region "Microsoft.VisualBasic::f9beda8137da2018c632fdbf67b53d9d, meme_suite\MEME.DocParser\MEME\Text\SiteLength.vb"

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

    '     Module SiteLength
    ' 
    '         Function: GetSize, SetSize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace DocumentFormat.MEME.Text

    ''' <summary>
    ''' 解析输入的位点的原始序列的长度可能会比较麻烦
    ''' </summary>
    Module SiteLength

        Const sp As String = "(-{2,}\s*)+"
        Const sp2 As String = "\*+"

        'TRAINING SET
        '********************************************************************************
        'DATAFILE= C:\GCModellerRepositoryRoot\1.30\diffexpr-results.logFold=2.DEGs.UpDowns\MMX-TO-NY.Down.fa\250bp.fasta
        'ALPHABET= ACGT
        'Sequence name            Weight Length  Sequence name            Weight Length  
        '-------------            ------ ------  -------------            ------ ------  
        'XC_0165                  1.0000    251  XC_0362                  1.0000    251  
        'XC_0371                  1.0000    251  XC_0370                  1.0000    251  
        'XC_0372                  1.0000    251  XC_0444                  1.0000    251  
        'XC_0441                  1.0000    251  XC_0442                  1.0000    251  
        'XC_0443                  1.0000    251  XC_0450                  1.0000    251  
        'XC_0451                  1.0000    251  XC_0603                  1.0000    251  
        'XC_0602                  1.0000    251  XC_0637                  1.0000    251  
        'XC_0638                  1.0000    251  XC_0848                  1.0000    251  
        'XC_0849                  1.0000    251  XC_1119                  1.0000    251  
        'XC_1120                  1.0000    251  XC_1300                  1.0000    251  
        'XC_1301                  1.0000    251  XC_1413                  1.0000    251  
        'XC_1409                  1.0000    251  XC_1411                  1.0000    251  
        'XC_1432                  1.0000    251  XC_1685                  1.0000    251  
        'XC_1689                  1.0000    251  XC_2126                  1.0000    251  
        'XC_2301                  1.0000    251  XC_2303                  1.0000    251  
        'XC_2305                  1.0000    251  XC_2319                  1.0000    251  
        'XC_2320                  1.0000    251  XC_2596                  1.0000    251  
        'XC_2651                  1.0000    251  XC_2690                  1.0000    251  
        'XC_2720                  1.0000    251  XC_2830                  1.0000    251  
        'XC_2981                  1.0000    251  XC_2977                  1.0000    251  
        'XC_3206                  1.0000    251  XC_0069                  1.0000    251  
        'XC_1991                  1.0000    251  XC_2037                  1.0000    251  
        'XC_2036                  1.0000    251  XC_2107                  1.0000    251  
        'XC_2311                  1.0000    251  XC_2312                  1.0000    251  
        'XC_2321                  1.0000    251  XC_2721                  1.0000    251  
        'XC_1415                  1.0000    251  XC_1414                  1.0000    251  
        'XC_1476                  1.0000    251  XC_1683                  1.0000    251  
        'XC_1733                  1.0000    251  XC_1766                  1.0000    251  
        'XC_2223                  1.0000    251  XC_2306                  1.0000    251  
        'XC_2309                  1.0000    251  XC_2459                  1.0000    251  
        'XC_2471                  1.0000    251  XC_2472                  1.0000    251  
        'XC_2774                  1.0000    251  XC_2115                  1.0000    251  
        'XC_2124                  1.0000    251  XC_2136                  1.0000    251  
        'XC_2147                  1.0000    251  XC_4079                  1.0000    251  
        'XC_4121                  1.0000    251  XC_4125                  1.0000    251  
        'XC_4128                  1.0000    251  XC_4126                  1.0000    251  
        'XC_2282                  1.0000    251  XC_2284                  1.0000    251  
        'XC_1202                  1.0000    251  XC_1435                  1.0000    251  
        'XC_1882                  1.0000    251  XC_2224                  1.0000    251  
        'XC_2302                  1.0000    251  XC_2315                  1.0000    251  
        'XC_2316                  1.0000    251  XC_2318                  1.0000    251  
        'XC_2483                  1.0000    251  XC_2484                  1.0000    251  
        'XC_2504                  1.0000    251  XC_2835                  1.0000    251  
        'XC_2934                  1.0000    251  XC_2976                  1.0000    251  
        'XC_2245                  1.0000    251  XC_2785                  1.0000    251  
        'XC_2787                  1.0000    251  XC_2922                  1.0000    251  
        'XC_2932                  1.0000    251  XC_3140                  1.0000    251  
        'XC_3968                  1.0000    251  XC_3969                  1.0000    251  
        'XC_3970                  1.0000    251  XC_4029                  1.0000    251  
        'XC_4028                  1.0000    251  
        '********************************************************************************

        '********************************************************************************
        'COMMAND LINE SUMMARY
        '********************************************************************************
        'This information can also be useful In the Event you wish To report a
        'problem with the MEME software.

        'command: meme C : \GCModellerRepositoryRoot\1.30\diffexpr-results.logFold=2.DEGs.UpDowns\MMX-TO-NY.Down.fa\250bp.fasta -dna -Mod zoops -evt 1 -nmotifs 30 -maxsize 1000000000 -maxw 52 

        'model:  Mod=         zoops    nmotifs=        30    evt=             1
        'Object Function()= E - value of product of p-values
        'width:  minw=            6    maxw=           52    minic=        0.00
        'width:  wg=             11    ws=              1    endgaps=       yes
        'nsites: minsites=        2    maxsites=       99    wnsites=       0.8
        'theta:  prob=            1    spmap=         uni    spfuzz=        0.5
        'em:     prior=   dirichlet    b=            0.01    maxiter=        50
        '        distance=    1e-05
        'data:   n=           24849    N=              99
        'strands: +
        'sample: seed=            0    seqfrac=         1
        'Letter frequencies In dataset:
        'A 0.189 C 0.319 G 0.309 T 0.183 
        'Background letter frequencies (from dataset With add-one prior applied):
        'A 0.189 C 0.319 G 0.309 T 0.183 

        Public Function GetSize(s As String) As Dictionary(Of String, Integer)
            If String.IsNullOrEmpty(s) Then
                Return New Dictionary(Of String, Integer)
            End If

            Dim Tokens As String() = Regex.Split(s, sp2)
            s = Tokens(1)
            Tokens = Regex.Split(s, sp)
            s = Tokens.Last
            Tokens = s.LineTokens
            Tokens = Tokens.Select(Function(x) Regex.Split(x, "\s+")).ToVector
            Tokens = (From x As String In Tokens Where Not String.IsNullOrWhiteSpace(x) Select x).ToArray

            Dim data = Tokens.Split(3)
            Dim hash As Dictionary(Of String, Integer) = data.ToDictionary(Function(x) x.First, Function(x) Scripting.CTypeDynamic(Of Integer)(x.Last))
            Return hash
        End Function

        <Extension>
        Public Function SetSize(motif As Motif, size As Dictionary(Of String, Integer)) As Motif
            For Each site As Site In motif.Sites
                site.Size = size(site.Name)
            Next

            Return motif
        End Function
    End Module
End Namespace
