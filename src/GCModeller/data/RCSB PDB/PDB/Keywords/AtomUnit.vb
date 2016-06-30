Namespace Keywords

    Public Structure Point3D
        Dim X, Y, Z As Double

        Public Overrides Function ToString() As String
            Return String.Format("<{0},{1},{2}>", X, Y, Z)
        End Function
    End Structure

    Public Class AtomUnit

        ''' <summary>
        ''' 氨基酸的名称简写
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AA_ID As String
        ''' <summary>
        ''' 当前的氨基酸分子在Fasta序列之中的残基位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AA_IDX As Integer
        Public Property Index As Integer
        Public Property Atom As String
        Public Property Location As Point3D

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  ---> ({1}) {2}   {3}", Index, AA_ID, Location.ToString, Atom)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s">
        ''' -----------------------------------------------|-----------------
        ''' N   ILE     7      25.289   6.282   7.602  1.00121.47           N
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Shared Function InternalParser(s As String, InternalIndex As Integer) As AtomUnit
            Dim Tokens As String() = (From strToken As String
                                      In s.Split
                                      Where Not String.IsNullOrEmpty(strToken)
                                      Select strToken).ToArray
            Dim Location As Point3D = New Point3D With {
                .X = Val(Tokens(3)),
                .Y = Val(Tokens(4)),
                .Z = Val(Tokens(5))
            }
            Return New AtomUnit With {
                .Index = InternalIndex,
                .Location = Location,
                .Atom = Tokens(0),
                .AA_ID = Tokens(1),
                .AA_IDX = Val(Tokens(2))
            }
        End Function
    End Class
End Namespace