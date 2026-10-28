Imports System

Module Program

    ''' <summary>
    ''' The entry point of the SGrid gene regulatory network demo.
    ''' (SGrid 基因调控网络演示程序的入口点)
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns>进程退出码，零表示全部断言通过</returns>
    Function Main(args As String()) As Integer
        Dim options As GRNDemo.DemoOptions = GRNDemo.DemoOptions.Parse(args)

        Select Case options.Command.ToLowerInvariant()
            Case "help", "-h", "--help"
                Call Console.WriteLine(GRNDemo.DemoOptions.Usage)

                Return 0
            Case "selftest"
                Return SelfTest.GrnSelfTest.Run(verbose:=Not options.Quiet)
            Case "simd-check", "simdcheck"
                Return SelfTest.SimdCheck.Run(verbose:=Not options.Quiet)
            Case Else
                Return GRNDemo.DemoRunner.Run(options)
        End Select
    End Function

End Module
