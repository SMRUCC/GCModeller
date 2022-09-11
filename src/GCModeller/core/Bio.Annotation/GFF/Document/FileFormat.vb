#Region "Microsoft.VisualBasic::a77f1036c87d0753dd6550a3198348af, GCModeller\core\Bio.Annotation\GFF\Document\FileFormat.vb"

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

    '   Total Lines: 93
    '    Code Lines: 73
    ' Comment Lines: 7
    '   Blank Lines: 13
    '     File Size: 4.10 KB


    '     Module FileFormat
    ' 
    '         Function: TryGetFreaturesData, TryGetMetaData
    ' 
    '         Sub: TrySetMetaData
    '         Structure parserHelper
    ' 
    '             Function: IsMetaDataLine, parse
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.NCBI.GenBank.TabularFormat.GFF.Document

    Friend Module FileFormat

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="Gff"></param>
        ''' <param name="defaultVer%">默认的文件格式版本号缺省值</param>
        Public Sub TrySetMetaData(data$(), ByRef Gff As GFFTable, defaultVer%)
            data = TryGetMetaData(data)

            Dim LQuery = From t As String
                         In data
                         Where Not t.IndexOf(" "c) = -1  ' ### 这种情况下mid函数会出错
                         Let p As Integer = InStr(t, " ")
                         Let Name As String = Mid(t, 1, p - 1)
                         Let Value As String = Mid(t, p + 1)
                         Select Name,
                             Value
                         Group By Name Into Group '
            Dim attrs As Dictionary(Of String, String) = LQuery _
                .ToDictionary(Function(obj)
                                  Return obj.Name.ToLower
                              End Function,
                              Function(obj)
                                  Return obj.Group _
                                      .Select(Function(x) x.Value) _
                                      .JoinBy("; ")
                              End Function)

            Call $"There are {attrs.Count} meta data was parsed from the gff file.".__DEBUG_ECHO

            Gff.GffVersion = CInt(Val(TryGetValue(attrs, "##gff-version")))
            Gff.date = TryGetValue(attrs, "##date")
            Gff.SrcVersion = TryGetValue(attrs, "##source-version")
            Gff.type = TryGetValue(attrs, "##type")
            Gff.SeqRegion = SeqRegion.Parser(TryGetValue(attrs, "##sequence-region"))

            ' 为零，则表示文本字符串为空值，则会使用默认的版本号
            If Gff.GffVersion = 0 Then
                Gff.GffVersion = defaultVer
            End If

            Call $"The parser version of the gff file is version {Gff.GffVersion}...".__DEBUG_ECHO

            If {1, 2, 3}.IndexOf(Gff.GffVersion) = -1 Then
                Call $"{NameOf(Version)}={Gff.GffVersion} is currently not supported yet, ignored!".Warning
            End If
        End Sub

        Public Function TryGetFreaturesData(data$(), version%) As Feature()
            Dim loadBuffer As String() = (From s As String
                                          In data
                                          Where Not String.IsNullOrWhiteSpace(s) AndAlso
                                              Not s.First = "#"c
                                          Select s).ToArray
            Dim helper As New parserHelper With {
                .version = version
            }
            Dim features As Feature() = loadBuffer _
                .Select(AddressOf helper.parse) _
                .ToArray
            Return features
        End Function

        Private Structure parserHelper
            Public version As Integer

            Public Function parse(s As String) As Feature
                Return FeatureParser.CreateObject(s, version)
            End Function

            Public Shared Function IsMetaDataLine(line As String) As Boolean
                Return Not String.IsNullOrEmpty(line) AndAlso Len(line) > 2 AndAlso String.Equals(Mid(line, 1, 2), "##")
            End Function
        End Structure

        Private Function TryGetMetaData(data As String()) As String()
            Try
                Dim LQuery = (From sLine As String
                              In data
                              Where parserHelper.IsMetaDataLine(sLine)
                              Select sLine).ToArray
                Return LQuery
            Catch ex As Exception
                Call App.LogException(New Exception(data.JoinBy(vbCrLf), ex))
                Return New String() {}
            End Try
        End Function
    End Module
End Namespace
