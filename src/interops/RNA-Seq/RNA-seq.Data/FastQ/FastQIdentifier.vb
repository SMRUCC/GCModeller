
Namespace Fastaq

    ''' <summary>
    ''' Illumina sequence identifiers
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FastQIdentifier

        'Sequences from the Illumina software use a systematic identifier:

        '@HWUSI-EAS100R:6:73:941:1973#0/1

        'HWUSI-EAS100R	the unique instrument name
        '          6    flowcell lane
        '         73	tile number within the flowcell lane
        '        941	'x'-coordinate of the cluster within the tile
        '       1973    'y'-coordinate of the cluster within the tile
        '         #0	index number for a multiplexed sample (0 for no indexing)
        '         /1	the member of a pair, /1 or /2 (paired-end or mate-pair reads only)

        'Versions of the Illumina pipeline since 1.4 appear to use #NNNNNN instead of #0 for the multiplex ID, 
        'where NNNNNN is the sequence of the multiplex tag.





        'With Casava 1.8 the format of the '@' line has changed:

        '@EAS139:136:FC706VJ:2:2104:15343:197393 1:Y:18:ATCACG

        '       EAS139	the unique instrument name
        '        136	the run id
        '    FC706VJ	the flowcell id
        '          2	flowcell lane
        '       2104	tile number within the flowcell lane
        '      15343	'x'-coordinate of the cluster within the tile
        '     197393	'y'-coordinate of the cluster within the tile
        '          1	the member of a pair, 1 or 2 (paired-end or mate-pair reads only)
        '          Y	Y if the read is filtered, N otherwise
        '         18	0 when none of the control bits are on, otherwise it is an even number
        '     ATCACG	index sequence

        ''' <summary>
        ''' The unique instrument name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Identifier As String
        ''' <summary>
        ''' Flowcell lane
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FlowCellLane As Integer
        ''' <summary>
        ''' Tile number within the flowcell lane
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Tiles As Integer
        ''' <summary>
        ''' 'x'-coordinate of the cluster within the tile
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property X As Integer
        ''' <summary>
        ''' 'y'-coordinate of the cluster within the tile
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Y As Integer
        ''' <summary>
        ''' Index number for a multiplexed sample (0 for no indexing)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MsIndex As String
        ''' <summary>
        ''' The member of a pair, /1 or /2 (paired-end or mate-pair reads only)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PairMember As String

        ''' <summary>
        ''' @HWUSI-EAS100R:6:73:941:1973#0/1
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        ''' <example>@FCC00ACABXX:8:1101:1333:1980#TTAGGCAT/1</example>
        Public Shared Function IDParser(str As String) As FastQIdentifier
            If Len(str) = 1 AndAlso str(0) = "+"c Then '可能是第三行数据
                Return New FastQIdentifier
            End If

            Dim Tokens As String() = str.Split(":"c)
            Dim Identifier As FastQIdentifier = New FastQIdentifier
            Identifier.Identifier = Tokens(0)
            Identifier.FlowCellLane = CInt(Val(Tokens(1)))
            Identifier.Tiles = CInt(Val(Tokens(2)))
            Identifier.X = CInt(Val(Tokens(3)))
            Identifier.Y = CInt(Val(Tokens(4)))

            Tokens = Tokens(4).Split("#"c).Last.Split("/"c)

            Identifier.MsIndex = Tokens(0)
            Identifier.PairMember = Tokens.ElementAtOrDefault(1)

            Return Identifier
        End Function

        Public Overrides Function ToString() As String
            Return $"{Identifier}:{FlowCellLane}:{Tiles}:{X}:{Y}#{MsIndex}/{PairMember}"
        End Function
    End Class
End Namespace